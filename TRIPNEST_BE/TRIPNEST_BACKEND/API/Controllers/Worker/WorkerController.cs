using APPLICATION.Interfaces.Worker;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Worker
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkerController : ControllerBase
    {
        private readonly IWorkerService _service;
        private readonly ILogger<WorkerController> _logger;

        public WorkerController(IWorkerService service, ILogger<WorkerController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpPost("index-property/{propertyId:long}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> IndexProperty(
            [FromRoute] long propertyId, 
            [FromQuery] bool force = false, 
            CancellationToken ct = default)
        {
            try
            {
                if (propertyId <= 0) return BadRequest(new { message = "propertyId must be positive" });

                var result = await _service.IndexPropertyAsync(propertyId, force, ct);

                if (!result.Indexed && result.Message == "Property not found")
                    return NotFound(new { message = result.Message });

                return Ok(result);
            }
            catch (OperationCanceledException)
            {
                return BadRequest(new { message = "Request cancelled." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in IndexProperty");
                return StatusCode(500, new { message = "Internal server error." });
            }
        }

        [HttpPost("send-reminders")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SendReminders(
            [FromQuery] int daysAhead = 1, 
            CancellationToken ct = default)
        {
            try
            {
                if (daysAhead < 0 || daysAhead > 30) return BadRequest(new { message = "daysAhead must be 0..30" });

                var result = await _service.SendRemindersAsync(daysAhead, ct);

                if (result.RemindersCreated == 0 && result.EmailsSent == 0)
                {
                    return NoContent();
                }

                return Ok(result);
            }
            catch (OperationCanceledException)
            {
                return BadRequest(new { message = "Request cancelled." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in SendReminders");
                return StatusCode(500, new { message = "Internal server error." });
            }
        }
    }
}
