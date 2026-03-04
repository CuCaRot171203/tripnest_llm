using APPLICATION.DTOs.Embedding;
using APPLICATION.Interfaces.Embedding;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Embedding
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmbeddingsController : ControllerBase
    {
        private readonly IEmbeddingsService _service;

        public EmbeddingsController(IEmbeddingsService service)
        {
            _service = service;
        }

        [HttpPost("generate")]
        [ProducesResponseType(typeof(EmbeddingDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Generate([FromBody] EmbeddingCreateDto dto, CancellationToken ct)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            // In real system you might restrict this endpoint to Worker/Admin via [Authorize(Roles="Worker")]
            var result = await _service.GenerateEmbeddingAsync(dto, ct);

            // return 201 Created with location header to GET endpoint
            var location = Url.Action(nameof(GetByItem), new { itemType = result.ItemType, itemId = result.ItemId });
            return Created(location ?? $"/api/embeddings/{result.ItemType}/{result.ItemId}", result);
        }

        [HttpGet("{itemType}/{itemId}")]
        [ProducesResponseType(typeof(EmbeddingDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByItem(string itemType, string itemId, CancellationToken ct)
        {
            var dto = await _service.GetByItemAsync(itemType, itemId, ct);
            if (dto == null) return NotFound(new { message = "Embedding not found." });
            return Ok(dto);
        }
    }
}
