using APPLICATION.DTOs.Search;
using APPLICATION.Interfaces.Search;
using INFRASTRUCTURE.DTOs.Search;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Search
{
    [Route("api/[controller]")]
    [ApiController]
    public class SearchController : ControllerBase
    {
        private readonly ISearchService _service;

        public SearchController(ISearchService service)
        {
            _service = service;
        }

        [HttpGet]
        [ProducesResponseType(typeof(SearchResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get([FromQuery] SearchQueryDto query, CancellationToken ct)
        {
            try
            {
                if (query.Page < 0 || query.PageSize < 0)
                    return BadRequest("page and pageSize must be non-negative");

                var result = await _service.SearchAsync(query, ct);
                return Ok(result);
            }
            catch (OperationCanceledException)
            {
                return StatusCode(StatusCodes.Status499ClientClosedRequest);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError, 
                    new { 
                        message = "Internal server error", 
                        detail = ex.Message 
                    }
                );
            }
        }

        [HttpGet("suggestions")]
        [ProducesResponseType(typeof(SuggestionDto[]), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Suggestions([FromQuery] string q, [FromQuery] int limit = 10, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(q))
                return BadRequest("q is required");

            var items = await _service.GetSuggestionsAsync(q, limit, ct);
            return Ok(items);
        }

        [HttpPost("semantic")]
        [ProducesResponseType(typeof(SearchResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Semantic([FromBody] SemanticSearchRequestDto request, CancellationToken ct = default)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Text))
                return BadRequest("text is required");

            var result = await _service.SemanticSearchAsync(request, ct);
            return Ok(result);
        }
    }
}
