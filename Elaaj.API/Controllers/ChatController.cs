using Elaaj.Application.Features.Chat.Commands.SendMessage;
using Elaaj.Application.Features.Chat.Queries.GetChatHistory;
using Elaaj.Domain.Constants;
using Elaaj.Domain.Entities;
using Elaaj.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Elaaj.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class ChatController : ControllerBase
{
    private readonly IMediator _mediator;

    public ChatController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// إرسال رسالة في الشات المخصص لروشتة معينة
    /// </summary>
    [HttpPost("send")]
    public async Task<IActionResult> SendMessage(
       [FromBody] SendMessageCommand command,
       [FromServices] IGenericRepository<Pharmacy> pharmacyRepo)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(currentUserId)) return Unauthorized();

        if (User.IsInRole(UserRoles.Owner) || User.IsInRole(UserRoles.PharmacyAdmin) || User.IsInRole(UserRoles.PharmacyOwner))
        {
            if (string.IsNullOrEmpty(command.SenderId))
                return BadRequest(new { Message = "يجب إرسال معرف الصيدلية (SenderId)." });

            if (!Guid.TryParse(command.SenderId, out Guid pharmacyId))
                return BadRequest(new { Message = "صيغة معرف الصيدلية غير صحيحة." });

            // التأكد إن اليوزر ده له صلاحية على الصيدلية
            var authorizedPharmacies = await pharmacyRepo.GetAllAsync(p =>
                p.Id == pharmacyId &&
                (p.OwnerId == currentUserId || p.Admins.Any(a => a.UserId == currentUserId))
            );

            if (!authorizedPharmacies.Any())
                return StatusCode(StatusCodes.Status403Forbidden, new { Message = "غير مصرح لك بإرسال رسائل نيابة عن هذه الصيدلية." });
        }
        else if (User.IsInRole(UserRoles.User))
        {
            command.SenderId = currentUserId;
        }
        else
        {
            return StatusCode(StatusCodes.Status403Forbidden, new { Message = "التوكن الخاص بك لا يحتوي على صلاحيات مقروءة!" });
        }

        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// جلب المحادثة السابقة بين المريض والصيدلي لروشتة معينة
    /// </summary>
    [HttpGet("history/{prescriptionId}/{otherUserId}")]
    public async Task<IActionResult> GetChatHistory(
    Guid prescriptionId,
    string otherUserId,
    [FromQuery] string? pharmacyId, // 👈 ضفنا الباراميتر ده عشان الصيدلية تبعت رقمها
    [FromServices] IGenericRepository<Pharmacy> pharmacyRepo)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(currentUserId)) return Unauthorized();

        string participantId = currentUserId; // الديفولت هو المريض

        // لو اللي بيطلب الهيستوري صيدلية (أدمن أو مالك)
        if (User.IsInRole(UserRoles.Owner) || User.IsInRole(UserRoles.PharmacyAdmin) || User.IsInRole(UserRoles.PharmacyOwner))
        {
            if (string.IsNullOrEmpty(pharmacyId))
            {
                return BadRequest(new { Message = "يجب إرسال معرف الصيدلية (pharmacyId) في الـ Query Parameters." });
            }

            if (!Guid.TryParse(pharmacyId, out Guid parsedPharmacyId))
            {
                return BadRequest(new { Message = "صيغة معرف الصيدلية غير صحيحة." });
            }

            // خطوة الأمان: نتأكد إن اليوزر ده فعلاً مدير في الصيدلية دي
            var authorizedPharmacies = await pharmacyRepo.GetAllAsync(p =>
                p.Id == parsedPharmacyId &&
                (p.OwnerId == currentUserId || p.Admins.Any(a => a.UserId == currentUserId))
            );

            if (!authorizedPharmacies.Any())
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { Message = "غير مصرح لك بعرض رسائل هذه الصيدلية." });
            }

            // لو تمام، هنخلي الطرف اللي بيبحث في الداتا بيز هو الصيدلية
            participantId = pharmacyId;
        }

        var query = new GetChatHistoryQuery
        {
            PrescriptionId = prescriptionId,
            CurrentUserId = participantId, // 👈 هياخد رقم الصيدلية لو صيدلي، ورقم المريض لو مريض
            OtherUserId = otherUserId
        };

        var result = await _mediator.Send(query);
        return Ok(result);
    }
}