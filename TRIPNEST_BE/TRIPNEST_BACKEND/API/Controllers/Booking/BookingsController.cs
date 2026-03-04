using APPLICATION.DTOs.Booking.Request;
using APPLICATION.Interfaces.Booking;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Booking
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingsController : ControllerBase
    {
        private readonly IBookingService _service;
        public BookingsController(IBookingService service)
        {
            _service = service;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] CreateBookingRequest dto)
        {
            var userId = Guid.Parse(User.FindFirst("sub")?.Value ?? throw new Exception("no sub"));
            var res = await _service.CreateBookingAsync(userId, dto);
            return CreatedAtAction(nameof(GetById), new { bookingId = res.BookingId }, res);
        }

        [HttpGet("{bookingId:guid}")]
        [Authorize]
        public async Task<IActionResult> GetById(Guid bookingId)
        {
            var userId = Guid.Parse(User.FindFirst("sub")?.Value ?? throw new Exception("no sub"));
            var isAdminOrHost = User.IsInRole("Admin") || User.IsInRole("Host");
            var res = await _service.GetBookingAsync(bookingId, userId, isAdminOrHost);
            if (res == null) return NotFound();
            return Ok(res);
        }

        [HttpPut("{bookingId:guid}/cancel")]
        [Authorize]
        public async Task<IActionResult> Cancel(Guid bookingId)
        {
            var userId = Guid.Parse(User.FindFirst("sub")?.Value ?? throw new Exception("no sub"));
            var isAdminOrHost = User.IsInRole("Admin") || User.IsInRole("Host");
            var res = await _service.CancelBookingAsync(bookingId, userId, isAdminOrHost);
            return Ok(res);
        }

        [HttpPut("{bookingId:guid}/confirm")]
        [Authorize(Roles = "Host,Admin")]
        public async Task<IActionResult> Confirm(Guid bookingId)
        {
            // Minimal: host can confirm => update status
            var booking = await _service.GetBookingAsync(bookingId, Guid.Empty, true);
            if (booking == null) return NotFound();
            // you may add method to service to Confirm; for brevity:
            // TODO: implement ConfirmBookingAsync in service. For now illustrate:
            return Ok(new { message = "Implement ConfirmBookingAsync in service" });
        }

        [HttpPost("{bookingId:guid}/items/{itemId:long}/modify")]
        [Authorize]
        public async Task<IActionResult> ModifyItem(Guid bookingId, long itemId, [FromBody] ModifyBookingItemRequest dto)
        {
            var userId = Guid.Parse(User.FindFirst("sub")?.Value ?? throw new Exception("no sub"));
            var isAdminOrHost = User.IsInRole("Admin") || User.IsInRole("Host");
            var res = await _service.ModifyBookingItemAsync(bookingId, itemId, dto, userId, isAdminOrHost);
            return Ok(res);
        }

        [HttpGet]
        [Authorize]
        public IActionResult Query([FromQuery] Guid? userId, [FromQuery] long? propertyId, [FromQuery] string? status, [FromQuery] DateOnly? from, [FromQuery] DateOnly? to)
        {
            // Implement filtering in service/repo as needed. For brevity return 501
            return StatusCode(501, "Implement filtering endpoint in service/repository");
        }
    }
}
