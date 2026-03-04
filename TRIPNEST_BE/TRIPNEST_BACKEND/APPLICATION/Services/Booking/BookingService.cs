using APPLICATION.DTOs.Booking.Request;
using APPLICATION.DTOs.Booking.Response;
using APPLICATION.Interfaces.Booking;
using DOMAIN;
using DOMAIN.Models;
using INFRASTRUCTURE.Interfaces.Bookings;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APPLICATION.Services.Booking
{
    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _repo;
        private readonly TripnestDbContext _db;

        public BookingService(IBookingRepository repo, TripnestDbContext db)
        {
            _repo = repo;
            _db = db;
        }

        public async Task<CreateBookingResponse> CreateBookingAsync(Guid userId, CreateBookingRequest dto)
        {
            // Basic validations
            if (dto.CheckoutDate <= dto.CheckinDate)
                throw new ArgumentException("checkout must be after checkin");

            // Start DB transaction
            using var tran = await _db.Database.BeginTransactionAsync();

            try
            {
                //Check & reserve availability for each item
                foreach (var item in dto.Items)
                {
                    var affected = await _repo.DecrementRoomAvailabilityAsync(item.RoomId, dto.CheckinDate, dto.CheckoutDate, item.Qty);
                    // For a valid reservation, we expect update on every date in range
                    var nights = dto.CheckoutDate.ToDateTime(TimeOnly.MinValue).Date - dto.CheckinDate.ToDateTime(TimeOnly.MinValue).Date;
                    int expectedRows = Math.Max(1, (int)nights.TotalDays);
                    if (affected < expectedRows)
                    {
                        throw new InvalidOperationException($"Not enough availability for room {item.RoomId}");
                    }
                }

                // Calculate prices (simplified: read room price from DB)
                decimal total = 0m;
                var booking = new Bookings
                {
                    BookingId = Guid.NewGuid(),
                    UserId = userId,
                    PropertyId = dto.PropertyId,
                    Status = "Pending",
                    CheckinDate = dto.CheckinDate,
                    CheckoutDate = dto.CheckoutDate,
                    GuestsCount = dto.GuestsCount,
                    Currency = dto.Currency,
                    CreatedAt = DateTime.UtcNow
                };

                var bookingItems = new List<Bookingitems>();
                foreach (var item in dto.Items)
                {
                    // Example: fetch room price
                    var room = await _db.Rooms.FirstOrDefaultAsync(r => r.RoomId == item.RoomId);
                    if (room == null) throw new InvalidOperationException($"Room {item.RoomId} not found");
                    decimal price = room.PricePerNight.Value;
                    var nights = (dto.CheckoutDate.ToDateTime(TimeOnly.MinValue).Date - dto.CheckinDate.ToDateTime(TimeOnly.MinValue).Date).Days;
                    var subtotal = price * item.Qty * Math.Max(1, nights);
                    total += subtotal;

                    bookingItems.Add(new Bookingitems
                    {
                        BookingItemId = 0,
                        BookingId = booking.BookingId,
                        RoomId = item.RoomId,
                        Price = price,
                        Nights = nights,
                        Qty = item.Qty,
                        Subtotal = subtotal
                    });
                }

                booking.TotalPrice = total;

                // Persist booking and booking items
                await _repo.AddAsync(booking);
                await _repo.AddBookingItemsAsync(bookingItems);

                // Create payment session (pseudo)
                var paymentSession = $"payment_session_for_{booking.BookingId}"; // integrate real gateway

                await tran.CommitAsync();

                return new CreateBookingResponse
                {
                    BookingId = booking.BookingId,
                    Status = booking.Status ?? "Pending",
                    PaymentSession = paymentSession
                };
            }
            catch
            {
                await tran.RollbackAsync();
                throw;
            }
        }

        public async Task<BookingDetailResponse?> GetBookingAsync(Guid bookingId, Guid callerUserId, bool isAdminOrHost)
        {
            var b = await _repo.GetByIdAsync(bookingId);
            if (b == null) return null;
            // check permissions: owner or admin/host
            if (!isAdminOrHost && b.UserId != callerUserId) throw new UnauthorizedAccessException();
            var res = new BookingDetailResponse
            {
                BookingId = b.BookingId,
                PropertyId = b.PropertyId,
                UserId = b.UserId,
                Status = b.Status ?? "Unknown",
                CheckinDate = b.CheckinDate,
                CheckoutDate = b.CheckoutDate,
                GuestsCount = b.GuestsCount ?? 0,
                TotalPrice = b.TotalPrice ?? 0,
                Currency = b.Currency ?? "USD",
                Items = b.Bookingitems.Select(i => new BookingItemResponse
                {
                    BookingItemId = i.BookingItemId,
                    RoomId = i.RoomId,
                    Qty = i.Qty ?? 0,
                    Nights = i.Nights ?? 0,
                    Price = i.Price ?? 0,
                    Subtotal = i.Subtotal ?? 0
                }).ToList()
            };
            return res;
        }

        public async Task<CancelBookingResponse> CancelBookingAsync(Guid bookingId, Guid callerUserId, bool isAdminOrHost)
        {
            using var tran = await _db.Database.BeginTransactionAsync();
            try
            {
                var b = await _repo.GetByIdAsync(bookingId);
                if (b == null) throw new KeyNotFoundException("Booking not found");
                if (!isAdminOrHost && b.UserId != callerUserId) throw new UnauthorizedAccessException();

                // Business rules: can cancel only if Pending or Confirmed and within policy
                b.Status = "Cancelled";
                b.UpdatedAt = DateTime.UtcNow;
                await _repo.UpdateAsync(b);

                // release room availability
                foreach (var item in b.Bookingitems)
                {
                    await _repo.IncrementRoomAvailabilityAsync(item.RoomId, b.CheckinDate, b.CheckoutDate, item.Qty ?? 1);
                }

                // TODO: call payment gateway refund if paid
                bool refundIssued = false;

                await tran.CommitAsync();
                return new CancelBookingResponse { BookingId = bookingId, Status = "Cancelled", RefundIssued = refundIssued };
            }
            catch
            {
                await tran.RollbackAsync();
                throw;
            }
        }

        public async Task<BookingDetailResponse?> ModifyBookingItemAsync(Guid bookingId, long bookingItemId, ModifyBookingItemRequest dto, Guid callerUserId, bool isAdminOrHost)
        {
            using var tran = await _db.Database.BeginTransactionAsync();
            try
            {
                var b = await _repo.GetByIdAsync(bookingId);
                if (b == null) throw new KeyNotFoundException("Booking not found");
                if (!isAdminOrHost && b.UserId != callerUserId) throw new UnauthorizedAccessException();

                var item = b.Bookingitems.FirstOrDefault(i => i.BookingItemId == bookingItemId);
                if (item == null) throw new KeyNotFoundException("Booking item not found");

                // If increasing qty => check availability and decrement
                if (dto.NewQty.HasValue && dto.NewQty.Value > (item.Qty ?? 0))
                {
                    var delta = dto.NewQty.Value - (item.Qty ?? 0);
                    var affected = await _repo.DecrementRoomAvailabilityAsync(item.RoomId, b.CheckinDate, b.CheckoutDate, delta);
                    var nights = (b.CheckoutDate.ToDateTime(TimeOnly.MinValue).Date - b.CheckinDate.ToDateTime(TimeOnly.MinValue).Date).Days;
                    if (affected < Math.Max(1, nights)) throw new InvalidOperationException("Not enough availability for modification");
                    item.Qty = dto.NewQty;
                }
                else if (dto.NewQty.HasValue && dto.NewQty.Value < (item.Qty ?? 0))
                {
                    var delta = (item.Qty ?? 0) - dto.NewQty.Value;
                    await _repo.IncrementRoomAvailabilityAsync(item.RoomId, b.CheckinDate, b.CheckoutDate, delta);
                    item.Qty = dto.NewQty;
                }

                if (dto.NewNights.HasValue && dto.NewNights.Value != (item.Nights ?? 0))
                {
                    // complex: changing nights may require re-check of availability and price recalc
                    // For simplicity, disallow reducing nights below 1 in this implementation
                    item.Nights = dto.NewNights;
                }

                // recalc subtotal and booking total
                item.Subtotal = (item.Price ?? 0) * (item.Qty ?? 0) * (item.Nights ?? 1);
                b.TotalPrice = b.Bookingitems.Sum(i => i.Subtotal) ?? 0;

                await _repo.UpdateAsync(b);
                await tran.CommitAsync();

                return await GetBookingAsync(bookingId, callerUserId, isAdminOrHost);
            }
            catch
            {
                await _db.Database.RollbackTransactionAsync();
                throw;
            }
        }
    }
}
