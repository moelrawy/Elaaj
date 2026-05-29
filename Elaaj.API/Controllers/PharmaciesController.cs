using Elaaj.Application.Features.Pharmacies.Commands.CreatePharmacy;
using Elaaj.Application.Features.Pharmacies.Commands.DeletePharmacy;
using Elaaj.Application.Features.Pharmacies.Commands.ToggleFavorite;
using Elaaj.Application.Features.Pharmacies.Commands.UpdatePharmacy;
using Elaaj.Application.Features.Pharmacies.Queries.GetMyPharmacies;
using Elaaj.Application.Features.Pharmacies.Queries.GetNearbyPharmacies;
using Elaaj.Application.Features.Pharmacies.Queries.GetPharmacy;
using Elaaj.Application.Features.Pharmacies.Queries.GetPharmacyById;
using Elaaj.Application.Users;
using Elaaj.Domain.Constants;
using Elaaj.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace Elaaj.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PharmaciesController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IUserContext _userContext;

        public PharmaciesController(IMediator mediator,IUserContext userContext)
        {
            _mediator = mediator;
            _userContext = userContext;
        }
        [HttpGet]
        //[AllowAnonymous]
        public async Task<IActionResult> GetAll()
        {
            var result = await _mediator.Send(new GetAllPharmaciesQuery());
            return Ok(result);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _mediator.Send(new GetPharmacyByIdQuery { Id = id });
            if (result == null) return NotFound(new { Message = "الصيدلية غير موجودة" });
            return Ok(result);
        }

        [HttpGet("nearby")]
        [AllowAnonymous]
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
        [HttpGet("my-pharmacies")]
        public async Task<IActionResult> GetMyPharmacies()
        {
            var result = await _mediator.Send(new GetMyPharmaciesQuery());

            var response = result.Select(p => new {
                id = p.Id,
                name = p.Name,
                address = p.Address,
                imageUrl = p.ImageUrl,
                role = p.Role
            });

            return Ok(response);
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Create([FromBody] CreatePharmacyCommand command)
        {
            var pharmacyId = await _mediator.Send(command);
            var pharmacy = await _mediator.Send(new GetPharmacyByIdQuery { Id = pharmacyId });

            return CreatedAtAction(nameof(GetById), new { id = pharmacyId }, new {
                id = pharmacy.Id,
                name = pharmacy.Name,
                address = pharmacy.Address,
                workingHours = pharmacy.WorkingHours,
                hasDelivery = pharmacy.HasDelivery,
                contactNumber = pharmacy.ContactNumber,
                latitude = pharmacy.Latitude,
                longitude = pharmacy.Longitude,
                createdAt = DateTime.UtcNow
            });
        }

        [HttpPut("{id}")]
        [Authorize(Roles = $"{UserRoles.Owner},{UserRoles.PharmacyOwner}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdatePharmacyCommand command)
        {
            //if (id != command.Id) return BadRequest(new { Message = "الـ ID غير متطابق" });
            command.Id = id;

            var success = await _mediator.Send(command);

            if (!success) return NotFound(new { success = false, message = "الصيدلية غير موجودة" });

            return Ok(new { success = true, message = "Pharmacy updated successfully." });
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = $"{UserRoles.Owner},{UserRoles.PharmacyOwner}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var success = await _mediator.Send(new DeletePharmacyCommand { Id = id });

            if (!success) return NotFound(new { Message = "الصيدلية غير موجودة" });

            return Ok(new { Message = "تم حذف الصيدلية بنجاح" });
        }

        [HttpPost("toggle-favorite")]
        public async Task<IActionResult> ToggleFavorite([FromBody] ToggleFavoriteCommand command)
        {
            var currentUser = _userContext.GetCurrentUser();

            if (currentUser == null) return Unauthorized();
            await _mediator.Send(command);
            return Ok(new { Message = "تم تحديث قائمة المفضلات" });
        }
    }
}
