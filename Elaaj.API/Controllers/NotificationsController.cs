using Elaaj.Application.Features.Notifications.Commands.CreateNotification;
using Elaaj.Application.Features.Notifications.Commands.MarkAsRead;
using Elaaj.Application.Features.Notifications.Queries.GetNotifications;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Elaaj.API.Controllers
{
    [Authorize]
    [ApiController]

    [Route("api/[controller]")]
    public class NotificationsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public NotificationsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        
        public async Task<IActionResult> GetNotifications([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] bool? isRead = null)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var query = new GetNotificationsQuery(userId!, pageNumber, pageSize, isRead);
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpPut("{id}/mark-as-read")]
        public async Task<IActionResult> MarkAsRead(Guid id)
        {
            var command = new MarkNotificationAsReadCommand(id);
            await _mediator.Send(command);
            return NoContent();
        }

        [HttpPut("mark-all-as-read")]
        public async Task<IActionResult> MarkAllAsRead()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            await _mediator.Send(new MarkAllNotificationsAsReadCommand(userId!));
            return NoContent();

        }
    
    [HttpGet("unread-count")]
        public async Task<IActionResult> GetUnreadCount()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var count = await _mediator.Send(new GetUnreadNotificationsCountQuery(userId!));
            return Ok(new { unreadCount = count });
        }

        [HttpPost("send-test")]
        [AllowAnonymous] // عشان تجرب من غير ما تحتاج تعمل Login وتجيب Token
        public async Task<IActionResult> SendTestNotification([FromQuery] string userId, [FromQuery] string title, [FromQuery] string message)
        {
            // هنستدعي السيرفس مباشرة ونمرر لها البيانات
            await _mediator.Send(new CreateNotificationCommand(userId, title, message, Elaaj.Domain.Enums.NotificationType.General));
            return Ok(new { message = "تم إرسال وحفظ الإشعار بنجاح!" });
        }
    } 
}