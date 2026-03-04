using APPLICATION.DTOs.Property;
using APPLICATION.Interfaces.Property;
using DOMAIN;
using INFRASTRUCTURE.Interfaces.Property;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Property
{
    [Route("api/[controller]")]
    [ApiController]
    public class PropertiesController : ControllerBase
    {
        private readonly IPropertiesService _service;

        public PropertiesController(IPropertiesService service)
        {
            _service = service;
        }

        [HttpPost]
        [Authorize(Roles = "Host,CompanyStaff,Admin")]
        public async Task<IActionResult> Create([FromBody] CreatePropertyRequest req)
        {
            // optionally get user id from claims
            var userId = User?.FindFirst("sub")?.Value;
            Guid? owner = null;
            if (Guid.TryParse(userId, out var g))
            {
                owner = g;
            }

            var created = await _service.CreateAsync(req, owner);
            return CreatedAtAction(nameof(GetById), new { id = created!.PropertyId }, created);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetList([FromQuery] string? q, [FromQuery] string? city,
            [FromQuery] double? lat, [FromQuery] double? lng, [FromQuery] double? radius,
            [FromQuery] decimal? minPrice, [FromQuery] decimal? maxPrice,
            [FromQuery] string? amenities, [FromQuery] string? sort,
            [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            List<int>? amenityList = null;
            if (!string.IsNullOrEmpty(amenities))
            {
                var parts = amenities.Split(',', StringSplitOptions.RemoveEmptyEntries);
                amenityList = new List<int>();
                foreach (var p in parts) if (int.TryParse(p, out var x)) amenityList.Add(x);
            }

            var res = await _service.GetListAsync(q, city, lat, lng, radius, minPrice, maxPrice, amenityList, sort, page, pageSize);
            return Ok(res);
        }

        [HttpGet("{id:long}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById([FromRoute] long id)
        {
            var prop = await _service.GetByIdAsync(id);
            if (prop == null)
            {
                return NotFound();
            }
            return Ok(prop);
        }

        [HttpPut("{id:long}")]
        [Authorize]
        public async Task<IActionResult> Update([FromRoute] long id, [FromBody] UpdatePropertyRequest req)
        {
            if (id != req.PropertyId)
            {
                return BadRequest("Property id mismatch");
            }
            var updated = await _service.UpdateAsync(req);
            if (updated == null)
            {
                return NotFound();
            }
            return Ok(updated);
        }

        [HttpDelete("{id:long}")]
        [Authorize(Roles = "Owner,Admin,CompanyStaff")]
        public async Task<IActionResult> Delete([FromRoute] long id)
        {
            var ok = await _service.SoftDeleteAsync(id);
            if (!ok)
            {
                return NotFound();
            }
            return NoContent();
        }

        [HttpPost("{id:long}/photos")]
        [Authorize(Roles = "Owner,CompanyStaff,Admin")]
        [RequestSizeLimit(10_485_760)] // ~10MB
        public async Task<IActionResult> UploadPhoto([FromRoute] long id, IFormFile file, [FromForm] long? order, [FromForm] string? meta)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("file is required");
            }
            var uploaded = await _service.UploadPhotoAsync(id, file, order, meta);
            return CreatedAtAction(nameof(GetPhotos), new { id = id }, uploaded);
        }

        [HttpGet("{id:long}/photos")]
        [AllowAnonymous]
        public async Task<IActionResult> GetPhotos([FromRoute] long id)
        {
            var photos = await _service.GetPhotosAsync(id);
            return Ok(photos);
        }

        [HttpDelete("{id:long}/photos/{photoId:long}")]
        [Authorize(Roles = "Owner,CompanyStaff,Admin")]
        public async Task<IActionResult> DeletePhoto([FromRoute] long id, [FromRoute] long photoId)
        {
            var repo = HttpContext.RequestServices.GetService(typeof(IPropertiesRepository)) as IPropertiesRepository;
            var photo = await repo!.GetPhotoAsync(id, photoId);
            if (photo == null)
            {
                return NotFound();
            }
            // soft delete by setting url = null or meta flag - here we remove
            // better: mark IsDeleted column. We'll just remove record for now
            // but to be safe use repository update:
            // Assume removing record:
            // (Directly remove)
            // Warning: better to implement explicit method in repository; for speed we do:
            var db = HttpContext.RequestServices.GetService(typeof(TripnestDbContext)) as TripnestDbContext;
            if (db == null)
            {
                return StatusCode(500);
            }
            db.Propertyphotos.Remove(photo);
            await db.SaveChangesAsync();
            return NoContent();
        }

        [HttpGet("{id:long}/rooms")]
        [AllowAnonymous]
        public async Task<IActionResult> GetRooms([FromRoute] long id)
        {
            var rooms = await _service.GetRoomsAsync(id);
            return Ok(rooms);
        }
    }
}
