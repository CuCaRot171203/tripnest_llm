using APPLICATION.DTOs.Reviews;
using APPLICATION.Interfaces.Review;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Review
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewsController : ControllerBase
    {
        private readonly IReviewService _reviewService;

        public ReviewsController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        [HttpPost("properties/{propertyId:long}/reviews")]
        [Authorize]
        public async Task<IActionResult> CreateReview(
            long propertyId, 
            [FromBody] CreateReviewRequestDto dto, 
            CancellationToken ct)
        {
            var userIdClaim = User.FindFirst("sub")?.Value ?? User.FindFirst("uid")?.Value;
            if (!Guid.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized();
            }

            try
            {
                var result = await _reviewService.CreateReviewAsync(propertyId, userId, dto, ct);
                return CreatedAtAction(nameof(GetReviewsByProperty), new { propertyId = propertyId }, result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("properties/{propertyId:long}/reviews")]
        [AllowAnonymous]
        public async Task<IActionResult> GetReviewsByProperty(long propertyId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10, CancellationToken ct = default)
        {
            var res = await _reviewService.GetReviewsByPropertyAsync(propertyId, page, pageSize, ct);
            return Ok(res);
        }

        [HttpPut("reviews/{reviewId:guid}")]
        [Authorize]
        public async Task<IActionResult> UpdateReview(Guid reviewId, [FromBody] UpdateReviewRequestDto dto, CancellationToken ct)
        {
            var userIdClaim = User.FindFirst("sub")?.Value ?? User.FindFirst("uid")?.Value;
            if (!Guid.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized();
            }


            try
            {
                var updated = await _reviewService.UpdateReviewAsync(reviewId, userId, dto, ct);
                if (updated == null) return NotFound();
                return Ok(updated);
            }
            catch (UnauthorizedAccessException ua)
            {
                return Forbid();
            }
        }

        [HttpDelete("reviews/{reviewId:guid}")]
        [Authorize]
        public async Task<IActionResult> DeleteReview(Guid reviewId, CancellationToken ct)
        {
            var userIdClaim = User.FindFirst("sub")?.Value ?? User.FindFirst("uid")?.Value;
            if (!Guid.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized();
            }


            try
            {
                var ok = await _reviewService.DeleteReviewAsync(reviewId, userId, ct);
                if (!ok) return NotFound();
                return NoContent();
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }
    }
}
