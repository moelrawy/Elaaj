using Elaaj.Application.Features.Pharmacies.Commands.CreatePharmacy;
using Elaaj.Application.Features.Pharmacies.Commands.DeletePharmacy;
using Elaaj.Application.Features.Pharmacies.Commands.ToggleFavorite;
using Elaaj.Application.Features.Pharmacies.Commands.UpdatePharmacy;
using Elaaj.Application.Features.Pharmacies.Queries.GetNearbyPharmacies;
using Elaaj.Application.Features.Pharmacies.Queries.GetPharmacy;
using Elaaj.Application.Features.Pharmacies.Queries.GetPharmacyById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Restaurants.Domain.Constants;

namespace Elaaj.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PharmaciesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PharmaciesController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll()
        {
            var result = await _mediator.Send(new GetAllPharmaciesQuery());
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _mediator.Send(new GetPharmacyByIdQuery { Id = id });
            if (result == null) return NotFound(new { Message = "الصيدلية غير موجودة" });
            return Ok(result);
        }

        [HttpGet("nearby")]
        public async Task<IActionResult> GetNearby([FromQuery] double lat, [FromQuery] double lon, [FromQuery] double radius = 5)
        {
            var query = new GetNearbyPharmaciesQuery
            {
                Latitude = lat,
                Longitude = lon,
                RadiusInKm = radius
            };
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles =UserRoles.PharmacyAdmin)]
        public async Task<IActionResult> Create([FromBody] CreatePharmacyCommand command)
        {
            var PharmacyId = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetById), new { id = PharmacyId }, new { Id = PharmacyId, Message = "تم إنشاء الصيدلية بنجاح" });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdatePharmacyCommand command)
        {
            if (id != command.Id) return BadRequest(new { Message = "الـ ID غير متطابق" });

            var success = await _mediator.Send(command);

            if (!success) return NotFound(new { Message = "الصيدلية غير موجودة" });

            return Ok(new { Message = "تم تعديل بيانات الصيدلية بنجاح" });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var success = await _mediator.Send(new DeletePharmacyCommand { Id = id });

            if (!success) return NotFound(new { Message = "الصيدلية غير موجودة" });

            return Ok(new { Message = "تم حذف الصيدلية بنجاح" });
        }

        [HttpPost("toggle-favorite")]
        public async Task<IActionResult> ToggleFavorite([FromBody] ToggleFavoriteCommand command)
        {
            await _mediator.Send(command);
            return Ok(new { Message = "تم تحديث قائمة المفضلات" });
        }
    }
}
