using Elaaj.Application.Features.Chat.Commands.SendMessage;
using Elaaj.Application.Features.Chat.Queries.GetChatHistory;
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
    public async Task<IActionResult> SendMessage([FromBody] SendMessageCommand command)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(currentUserId)) return Unauthorized();

        command.SenderId = currentUserId; // بناخد الـ Sender من التوكن للأمان

        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// جلب المحادثة السابقة بين المريض والصيدلي لروشتة معينة
    /// </summary>
    [HttpGet("history/{prescriptionId}/{otherUserId}")]
    public async Task<IActionResult> GetChatHistory(Guid prescriptionId, string otherUserId)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(currentUserId)) return Unauthorized();

        var query = new GetChatHistoryQuery
        {
            PrescriptionId = prescriptionId,
            CurrentUserId = currentUserId,
            OtherUserId = otherUserId
        };

        var result = await _mediator.Send(query);
        return Ok(result);
    }
}