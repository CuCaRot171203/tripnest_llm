using APPLICATION.DTOs.Upload;
using APPLICATION.Interfaces.Upload;
using APPLICATION.Services.Upload;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Upload
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadsController : ControllerBase
    {
        private readonly IUploadService _service;
        
        public UploadsController(IUploadService service)
        {
            _service = service;
        }

        [HttpPost]
        [ProducesResponseType(typeof(UploadResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateFileRecord(
            [FromBody] SaveFileRecordRequest body, 
            CancellationToken ct = default)
        {
            if (body == null) 
            {
                return BadRequest(new { message = "body is required" });
            }
            if (string.IsNullOrWhiteSpace(body.Key))
            {
                return BadRequest(new { message = "key is required" });
            }

            // Save DB record and return 201 with location header
            var created = await _service.SaveUploadedFileRecordAsync(body.PropertyId, body.Key, body.Order, body.Meta, ct);

            return CreatedAtAction(
                nameof(GetPhotoById), 
                new { id = created.PhotoId }, 
                created);
        }

        [HttpGet("{id:long}")]
        [ProducesResponseType(typeof(UploadResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetPhotoById([FromRoute] long id, CancellationToken ct = default)
        {
            return await Task.FromResult<IActionResult>(NotFound());
        }

        [HttpDelete("{id:long}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeletePhoto(
            [FromRoute] long id, 
            CancellationToken ct = default)
        {
            var ok = await _service.DeletePhotoAsync(id, ct);
            if (!ok) return NotFound(new { message = "photo not found" });
            return NoContent();
        }

        // REQUEST DTO
        public class SaveFileRecordRequest
        {
            public long PropertyId { get; set; }
            public string Key { get; set; } = null!;
            public int? Order { get; set; }
            public string? Meta { get; set; }
        }
    }
}
