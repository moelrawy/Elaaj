using Elaaj.Application.Features.Users.Commands.AssignUserRole;
using Elaaj.Application.Features.Users.Commands.GetUserDetails;
using Elaaj.Application.Features.Users.Commands.UnAssignUserRole;
using Elaaj.Application.Features.Users.Commands.UpdateUserDetails;
using Elaaj.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace Elaaj.API.Controllers
{
    [ApiController]
    [Route("api/identity")]
    public class IdentityController(IMediator mediator) : ControllerBase
    {
        [HttpGet("profile")]
        [Authorize] // لازم يكون معاه Token عشان السيستم يعرف هو مين
        public async Task<IActionResult> GetProfile()
        {
            // بنبعت طلب الـ Query والـ Handler اللي إنت كتبته هيتولى الباقي
            var userDetails = await mediator.Send(new GetUserDetailsQuery());

            if (userDetails == null)
                return NotFound(new { Message = "بيانات المستخدم غير موجودة" });

            return Ok(userDetails);
        }

        [HttpPatch("user")]
        [Authorize]
        public async Task<IActionResult> UpdateUserDetails(UpdateUserDetailsCommand command)
        {
            await mediator.Send(command);
            return NoContent();
        }

        [HttpPost("userRole")]
        [Authorize(Roles = UserRoles.Owner)]
        public async Task<IActionResult> AssignUserRole(AssignUserRoleCommand command)
        {
            await mediator.Send(command);
            return NoContent();
        }

        [HttpDelete("userRole")]
        [Authorize(Roles = UserRoles.Owner)]
        public async Task<IActionResult> UnassignUserRole(UnAssignUserRoleCommand command)
        {
            await mediator.Send(command);
            return NoContent();
        }
    }
}