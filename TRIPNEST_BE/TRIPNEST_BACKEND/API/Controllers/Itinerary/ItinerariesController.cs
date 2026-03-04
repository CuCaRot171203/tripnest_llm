using APPLICATION.DTOs.Itineraries;
using APPLICATION.Interfaces.Itinerary;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Itinerary
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItinerariesController : ControllerBase
    {
        private readonly IItineraryService _service;

        public ItinerariesController(IItineraryService service)
        {
            _service = service;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create(
            [FromBody] ItineraryCreateDto dto, 
            CancellationToken ct)
        {
            if (dto == null)
            {
                return BadRequest(new ProblemDetails { Title = "Request body is required" });
            }

            try
            {
                var userId = Guid.Parse(User.FindFirst("sub")?.Value ?? throw new Exception("User id not found"));
                var result = await _service.CreateItineraryAsync(userId, dto, ct);
                return CreatedAtAction(nameof(GetById), new { id = result.ItineraryId }, result);
            }
            catch (FormatException)
            {
                return Unauthorized(new ProblemDetails { Title = "Invalid user id in token" });
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch(Exception ex)
            {
                return StatusCode(500, new ProblemDetails { Title = "Server error", Detail = ex.Message });
            }
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken ct)
        {
            var item = await _service.GetItineraryByIdAsync(id, ct);
            if (item == null)
            {
                return NotFound();
            }
            return Ok(item);
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetMine(CancellationToken ct)
        {
            try
            {
                var userId = Guid.Parse(User.FindFirst("sub")?.Value ?? throw new Exception("User id not found"));
                var items = await _service.GetItinerariesByUserAsync(userId, ct);
                return Ok(items);
            }
            catch (FormatException)
            {
                return Unauthorized(new ProblemDetails { Title = "Invalid user id in token" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ProblemDetails { Title = "Server error", Detail = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int skip = 0, [FromQuery] int take = 50, CancellationToken ct = default)
        {
            if (take <= 0 || take > 100)
            {
                return BadRequest(new ProblemDetails { Title = "take must be between 1 and 100" });
            }

            var list = await _service.GetAllAsync(skip, take, ct);
            return Ok(list);
        }

        [HttpPut("{id:guid}")]
        [Authorize]
        public async Task<IActionResult> Update(
            [FromRoute] Guid id, 
            [FromBody] ItineraryUpdateDto dto, 
            CancellationToken ct)
        {
            if (dto == null)
            {
                return BadRequest(new ProblemDetails { Title = "Request body is required" });
            }

            try
            {
                var userId = Guid.Parse(User.FindFirst("sub")?.Value ?? throw new Exception("User id not found"));
                var success = await _service.UpdateItineraryAsync(userId, id, dto, ct);
                if (!success)
                {
                    return NotFound();
                }
                return NoContent();
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (FormatException)
            {
                return Unauthorized(new ProblemDetails { Title = "Invalid user id in token" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ProblemDetails { Title = "Server error", Detail = ex.Message });
            }
        }

        [HttpDelete("{id:guid}")]
        [Authorize]
        public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken ct)
        {
            try
            {
                var userId = Guid.Parse(User.FindFirst("sub")?.Value ?? throw new Exception("User id not found"));
                var deleted = await _service.DeleteItineraryAsync(userId, id, ct);
                if (!deleted)
                {
                    return NotFound();
                }
                return NoContent();
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (FormatException)
            {
                return Unauthorized(new ProblemDetails { Title = "Invalid user id in token" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ProblemDetails { Title = "Server error", Detail = ex.Message });
            }
        }
    }
}
