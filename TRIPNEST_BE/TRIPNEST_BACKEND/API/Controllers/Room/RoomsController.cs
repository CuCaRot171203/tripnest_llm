using APPLICATION.DTOs.Room.Request;
using APPLICATION.Interfaces.Rooms;
using APPLICATION.Services.Rooms;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Room
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomsController : ControllerBase
    {
        private readonly IRoomsService _service;

        public RoomsController(IRoomsService service)
        {
            _service = service;
        }

        [HttpPost("properties/{propertyId:long}/rooms")]
        // [Authorize(Roles = "Owner,CompanyStaff")]
        public async Task<IActionResult> CreateRoom([FromRoute] long propertyId, [FromBody] CreateRoomRequest req)
        {
            if (propertyId != req.PropertyId) req.PropertyId = propertyId;
            var created = await _service.CreateRoomAsync(req);
            return CreatedAtAction(nameof(GetRoom), new { roomId = created.RoomId }, created);
        }

        [HttpGet("rooms/{roomId:long}")]
        public async Task<IActionResult> GetRoom([FromRoute] long roomId)
        {
            var room = await _service.UpdateRoomAsync(roomId, new UpdateRoomRequest()); // little hack: better implement Get in service/repo
            if (room == null) return NotFound();
            return Ok(room);
        }

        [HttpPut("rooms/{roomId:long}")]
        // [Authorize(Roles = "Owner,CompanyStaff")]
        public async Task<IActionResult> UpdateRoom([FromRoute] long roomId, [FromBody] UpdateRoomRequest req)
        {
            var updated = await _service.UpdateRoomAsync(roomId, req);
            if (updated == null) return NotFound();
            return Ok(updated);
        }

        [HttpDelete("rooms/{roomId:long}")]
        // [Authorize(Roles = "Owner,CompanyStaff")]
        public async Task<IActionResult> DeleteRoom([FromRoute] long roomId)
        {
            var ok = await _service.DeleteRoomAsync(roomId);
            if (!ok) return NotFound();
            return NoContent();
        }

        [HttpGet("rooms/{roomId:long}/availability")]
        public async Task<IActionResult> GetAvailability([FromRoute] long roomId, [FromQuery] string from, [FromQuery] string to)
        {
            if (!DateOnly.TryParse(from, out var fromDate) || !DateOnly.TryParse(to, out var toDate))
                return BadRequest("from/to must be valid dates in YYYY-MM-DD format");

            var result = await _service.GetAvailabilityAsync(roomId, fromDate, toDate);
            return Ok(result);
        }

        [HttpPost("rooms/{roomId:long}/availability/bulk")]
        // [Authorize(Roles = "Owner,CompanyStaff")]
        public async Task<IActionResult> BulkAvailability([FromRoute] long roomId, [FromBody] BulkAvailabilityRequest req)
        {
            if (req?.Dates == null || !req.Dates.Any())
                return BadRequest("Dates required");

            var ok = await _service.BulkSetAvailabilityAsync(roomId, req);
            if (!ok) return NotFound();
            return NoContent();
        }

        [HttpPost("rooms/{roomId:long}/prices")]
        // [Authorize(Roles = "Owner,CompanyStaff")]
        public async Task<IActionResult> CreatePriceSlot([FromRoute] long roomId, [FromBody] CreatePriceSlotRequest req)
        {
            try
            {
                var created = await _service.CreatePriceSlotAsync(roomId, req);
                return CreatedAtAction(nameof(GetPriceSlot), new { roomId = created.RoomId, priceId = created.RoomPriceId }, created);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("rooms/{roomId:long}/prices/{priceId:long}")]
        public async Task<IActionResult> GetPriceSlot(long roomId, long priceId)
        {
            // Implement if needed (left minimal)
            return Ok();
        }
    }
}
