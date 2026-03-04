using APPLICATION.DTOs.Users.Request;
using APPLICATION.DTOs.Users.Response;
using APPLICATION.Interfaces.Users;
using INFRASTRUCTURE.Interfaces.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APPLICATION.Services.Users
{
    public class UsersService : IUsersService
    {
        private readonly IUsersRepository _repo;

        public UsersService(IUsersRepository repo) { 
            _repo = repo;
        }

        public async Task<UserResponse> GetCurrentUserAsync(Guid currentUserId)
        {
            var user = await _repo.GetByIdAsync(currentUserId);
            if (user == null)
            {
                throw new KeyNotFoundException("User not found");
            }
            return MapToUserResponse(user);
        }

        public async Task<UserResponse> UpdateCurrentUserAsync(
            Guid currentUserId, UpdateProfileRequest request)
        {
            var entity = await _repo.GetByIdAsync(currentUserId);
            if (entity == null)
            {
                throw new KeyNotFoundException("User not found");
            }
            if (request.FullName != null)
            {
                entity.FullName = request.FullName;
            }
            if (request.Phone != null)
            {
                 entity.Phone = request.Phone;
            }
            if (request.Locale != null)
            {
                entity.Locale = request.Locale;
            }
            if (request.ProfilePhotoUrl != null)
            {
                entity.ProfilePhotoUrl = request.ProfilePhotoUrl;
            }
            entity.UpdatedAt = DateTime.UtcNow;

            await _repo.UpdateProfileAsync(entity);

            var updated = await _repo.GetByIdAsync(currentUserId);
            if(updated == null)
            {
                throw new InvalidOperationException("Failed to fetch updated user");
            }

            return MapToUserResponse(updated);
        }

        public async Task<UserResponse?> GetPublicProfileAsync(Guid id)
        {
            var user = await _repo.GetByIdAsync(id);
            if (user == null)
            {
                return null;
            }
            return MapToUserResponse(user);
        }

        public async Task InviteUserAsync(Guid callerUserId, InviteUserRequest request)
        {
            var existing = await _repo.GetByEmailAsync(request.Email);
            if(existing != null)
            {
                await _repo.AddCompanyEmployeeAsync(
                        request.CompanyId,
                        existing.UserId,
                        request.Role
                    );
                return;
            }

            var newUserId = Guid.NewGuid();
            await _repo.CreateUnverifiedUserAsync(request.Email, newUserId);
            await _repo.AddCompanyEmployeeAsync(request.CompanyId, newUserId, request.Role );

            // TODO: gửi email invite (tách ra IEmailService)
        }

        public async Task<IEnumerable<BookingSummaryResponse>> GetUserBookingsAsync(Guid callerUserId, Guid targetUserId)
        {
            var bookings = await _repo.GetBookingsForUserAsync(targetUserId);
            if(bookings == null)
            {
                return Enumerable.Empty<BookingSummaryResponse>();
            }

            var list = bookings.Select(b => new BookingSummaryResponse
            {
                BookingId = GetBookingGuidSafe(b),
                PropertyId = GetPropertyIdSafe(b),
                PropertyName = GetPropertyNameSafe(b),
                StartAt = GetStartAtSafe(b),
                EndAt = GetEndAtSafe(b),
                TotalAmount = GetTotalAmountSafe(b),
                Status = GetStatusSafe(b)
            }).ToList();

            return list;
        }

        // MAPPING HELPERS
        private static UserResponse MapToUserResponse(DOMAIN.Models.Users u)
        {
            return new UserResponse
            {
                UserId = u.UserId,
                Email = u.Email,
                FullName = u.FullName,
                Phone = u.Phone,
                Locale = u.Locale,
                ProfilePhotoUrl = u.ProfilePhotoUrl,
                IsActive = u.IsActive ?? false,
                CreatedAt = u.CreatedAt
            };
        }

        private static Guid GetBookingGuidSafe(object booking)
        {
            if (booking == null) return Guid.Empty;
            var type = booking.GetType();

            var prop = type.GetProperty("BookingId") ?? type.GetProperty("Id") ?? type.GetProperty("BookingUUID");
            if (prop != null && prop.GetValue(booking) is Guid g) return g;

            return Guid.Empty;
        }

        private static long GetPropertyIdSafe(object booking)
        {
            if (booking == null) return 0;
            var type = booking.GetType();
            var prop = type.GetProperty("PropertyId") ?? type.GetProperty("Property_Id") ?? type.GetProperty("PropId");
            if (prop != null)
            {
                var val = prop.GetValue(booking);
                if (val is long l) return l;
                if (val is int i) return i;
                if (val is short s) return s;
            }
            return 0;
        }

        private static string? GetPropertyNameSafe(object booking)
        {
            if (booking == null) return null;
            var type = booking.GetType();
            var nav = type.GetProperty("Property") ?? type.GetProperty("Properties");
            if (nav != null)
            {
                var navVal = nav.GetValue(booking);
                if (navVal != null)
                {
                    var navType = navVal.GetType();
                    var nameProp = navType.GetProperty("Name") ?? navType.GetProperty("Title") ?? navType.GetProperty("PropertyName");
                    if (nameProp != null)
                    {
                        var nameVal = nameProp.GetValue(navVal);
                        return nameVal?.ToString();
                    }
                }
            }
            var direct = type.GetProperty("PropertyName") ?? type.GetProperty("PropertyTitle");
            if (direct != null) return direct.GetValue(booking)?.ToString();

            return null;
        }

        private static DateTime GetStartAtSafe(object booking)
        {
            if (booking == null) return DateTime.MinValue;
            var type = booking.GetType();
            var prop = type.GetProperty("StartAt") ?? type.GetProperty("Start") ?? type.GetProperty("From");
            if (prop != null)
            {
                var val = prop.GetValue(booking);
                if (val is DateTime dt) return dt;
                if (val is DateTime?) return ((DateTime?)val) ?? DateTime.MinValue;
            }
            return DateTime.MinValue;
        }

        private static DateTime GetEndAtSafe(object booking)
        {
            if (booking == null) return DateTime.MinValue;
            var type = booking.GetType();
            var prop = type.GetProperty("EndAt") ?? type.GetProperty("End") ?? type.GetProperty("To");
            if (prop != null)
            {
                var val = prop.GetValue(booking);
                if (val is DateTime dt) return dt;
                if (val is DateTime?) return ((DateTime?)val) ?? DateTime.MinValue;
            }
            return DateTime.MinValue;
        }

        private static decimal GetTotalAmountSafe(object booking)
        {
            if (booking == null) return 0m;
            var type = booking.GetType();
            var prop = type.GetProperty("TotalAmount") ?? type.GetProperty("Amount") ?? type.GetProperty("Total");
            if (prop != null)
            {
                var val = prop.GetValue(booking);
                if (val is decimal dec) return dec;
                if (val is double d) return Convert.ToDecimal(d);
                if (val is float f) return Convert.ToDecimal(f);
                if (val is int i) return i;
                if (val is long l) return l;
            }
            return 0m;
        }

        private static string? GetStatusSafe(object booking)
        {
            if (booking == null) return null;
            var type = booking.GetType();
            var prop = type.GetProperty("Status") ?? type.GetProperty("BookingStatus") ?? type.GetProperty("State");
            if (prop != null) return prop.GetValue(booking)?.ToString();
            return null;
        }
    }
}
