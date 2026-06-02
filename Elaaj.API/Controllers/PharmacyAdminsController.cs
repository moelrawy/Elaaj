using Elaaj.Application.Features.PharmacyAdmins.Commands.AssignAdmin;
using Elaaj.Application.Features.PharmacyAdmins.Commands.DeleteAdmin;
using Elaaj.Application.Features.PharmacyAdmins.Queries.GetMyPharmacyAdmins;
using Elaaj.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Elaaj.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize] // Must be logged in
public class PharmacyAdminsController : ControllerBase
{
    private readonly IMediator _mediator;

    public PharmacyAdminsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // Only PharmacyOwner or platform Owner can assign admins
    [HttpPost("assign")]
    [Authorize(Roles = $"{UserRoles.PharmacyOwner},{UserRoles.Owner}")]
    public async Task<IActionResult> AssignAdmin([FromBody] AssignPharmacyAdminCommand command)
    {
        try
        {
            await _mediator.Send(command);
            return Ok(new { Message = "تم إضافة الأدمن بنجاح" });
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(403, new { Message = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { Message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }

    [HttpGet("{pharmacyId}")]
    [Authorize(Roles = $"{UserRoles.PharmacyOwner},{UserRoles.Owner}")]
    public async Task<IActionResult> GetMyPharmacyAdmins(Guid pharmacyId)
    {
        try
        {
            var result = await _mediator.Send(new GetMyPharmacyAdminsQuery { PharmacyId = pharmacyId });
            return Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(403, new { Message = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { Message = ex.Message });
        }
    }

    [HttpDelete("{pharmacyId}/{userId}")]
    [Authorize(Roles = $"{UserRoles.PharmacyOwner},{UserRoles.Owner}")]
    public async Task<IActionResult> DeletePharmacyAdmin(Guid pharmacyId, string userId)
    {
        try
        {
            await _mediator.Send(new DeletePharmacyAdminCommand { PharmacyId = pharmacyId, UserId = userId });
            return Ok(new { Message = "تم حذف الأدمن بنجاح" });
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(403, new { Message = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { Message = ex.Message });
        }
    }
}