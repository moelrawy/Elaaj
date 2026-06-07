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

        // =================================================================
        // 🚀 التعديل الجوهري: تحديد "نية" المستخدم بناءً على الـ SenderId
        // =================================================================

        // هل المستخدم بيحاول يبعت الرسالة بالنيابة عن صيدلية؟
        bool isActingAsPharmacy = !string.IsNullOrEmpty(command.SenderId) && command.SenderId != currentUserId;

        if (isActingAsPharmacy)
        {
            // 1. نتأكد إن التوكن بتاعه فيه صلاحيات الصيدلة أصلاً
            if (!User.IsInRole(UserRoles.Owner) && !User.IsInRole(UserRoles.PharmacyAdmin) && !User.IsInRole(UserRoles.PharmacyOwner))
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { Message = "ليس لديك صلاحية لإرسال رسائل كصيدلية." });
            }

            // 2. نتأكد من صحة صيغة الـ ID بتاع الصيدلية
            if (!Guid.TryParse(command.SenderId, out Guid pharmacyId))
            {
                return BadRequest(new { Message = "صيغة معرف الصيدلية غير صحيحة." });
            }

            // 3. نتأكد إنه المالك أو الأدمن للصيدلية دي تحديداً
            var authorizedPharmacies = await pharmacyRepo.GetAllAsync(p =>
                p.Id == pharmacyId &&
                (p.OwnerId == currentUserId || p.Admins.Any(a => a.UserId == currentUserId))
            );

            if (!authorizedPharmacies.Any())
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { Message = "غير مصرح لك بإرسال رسائل نيابة عن هذه الصيدلية." });
            }
        }
        else
        {
            // لو الـ SenderId فاضي، أو بيساوي الـ ID بتاع اليوزر، يبقى هو بيلعب دور "المريض" دلوقتي
            command.SenderId = currentUserId;
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
    [FromQuery] string? pharmacyId,
    [FromServices] IGenericRepository<Pharmacy> pharmacyRepo)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(currentUserId)) return Unauthorized();

        string participantId = currentUserId; // الديفولت: اليوزر بيستعرض الشات كمريض

        // =================================================================
        // 🚀 التعديل: تحديد "نية" الاستعراض بناءً على إرسال pharmacyId
        // =================================================================
        bool isActingAsPharmacy = !string.IsNullOrEmpty(pharmacyId);

        if (isActingAsPharmacy)
        {
            // 1. نتأكد إن التوكن فيه صلاحيات الصيدلة
            if (!User.IsInRole(UserRoles.Owner) && !User.IsInRole(UserRoles.PharmacyAdmin) && !User.IsInRole(UserRoles.PharmacyOwner))
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { Message = "ليس لديك صلاحية لعرض رسائل الصيدليات." });
            }

            // 2. التأكد من الصيغة
            if (!Guid.TryParse(pharmacyId, out Guid parsedPharmacyId))
            {
                return BadRequest(new { Message = "صيغة معرف الصيدلية غير صحيحة." });
            }

            // 3. نتأكد إنه المالك أو الأدمن للصيدلية دي
            var authorizedPharmacies = await pharmacyRepo.GetAllAsync(p =>
                p.Id == parsedPharmacyId &&
                (p.OwnerId == currentUserId || p.Admins.Any(a => a.UserId == currentUserId))
            );

            if (!authorizedPharmacies.Any())
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { Message = "غير مصرح لك بعرض رسائل هذه الصيدلية." });
            }

            // لو كل حاجة تمام، نخلي الطرف اللي بيبحث في الداتا بيز هو "الصيدلية"
            participantId = pharmacyId;
        }

        var query = new GetChatHistoryQuery
        {
            PrescriptionId = prescriptionId,
            CurrentUserId = participantId, // هياخد رقم المريض أو رقم الصيدلية حسب النية
            OtherUserId = otherUserId
        };

        var result = await _mediator.Send(query);
        return Ok(result);
    }
}