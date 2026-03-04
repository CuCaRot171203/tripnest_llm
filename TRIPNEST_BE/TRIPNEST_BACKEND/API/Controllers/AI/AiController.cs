using APPLICATION.DTOs.Itineraries;
using APPLICATION.Interfaces.Itinerary;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.AI
{
    [Route("api/[controller]")]
    [ApiController]
    public class AiController : ControllerBase
    {

        private readonly IItineraryService _service;

        public AiController(IItineraryService service)
        {
            _service = service;
        }

        [HttpPost("suggest-itinerary")]
        public async Task<IActionResult> SuggestItinerary(
            [FromBody] SuggestItineraryRequestDto req, 
            CancellationToken ct)
        {
            if (req == null)
            {
                return BadRequest(new ProblemDetails { Title = "Request body is required" });
            }

            try
            {
                Guid? userId = null;
                if (User.Identity?.IsAuthenticated == true)
                {
                    if (Guid.TryParse(User.FindFirst("sub")?.Value, out var uid))
                    {
                        userId = uid;
                    }
                }

                var result = await _service.SuggestItineraryAsync(userId, req, ct);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ProblemDetails { Title = "AI generation error", Detail = ex.Message });
            }
        }

        [HttpPost("embedding-index/property/{propertyId}")]
        [Authorize(Roles = "Admin,Worker")]
        public async Task<IActionResult> PushEmbedding(
            [FromRoute] string propertyId, 
            [FromBody] EmbeddingRequestDto req, 
            CancellationToken ct)
        {
            if (req == null)
            {
                return BadRequest(new ProblemDetails { Title = "Request body is required" });
            }

            try
            {
                var ok = await _service.PushEmbeddingAsync(req.SourceId, req.Payload, ct);
                if (!ok)
                {
                    return StatusCode(
                        502, 
                        new ProblemDetails 
                        { 
                            Title = "Failed to push embedding to vector DB" 
                        }
                     );
                }
                return Accepted();
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch(Exception ex)
            {
                return StatusCode(
                    500, 
                    new ProblemDetails 
                    { 
                        Title = "Server error", 
                        Detail = ex.Message 
                    }
                );
            }
        }
    }
}
