using Elaaj.Application.Features.Prescriptions.Commands.AcceptReply;
using Elaaj.Application.Features.Prescriptions.Commands.CreatePrescription;
using Elaaj.Application.Features.Prescriptions.Commands.CreateReply;
using Elaaj.Application.Features.Prescriptions.Queries.GetMyPrescriptions;
using Elaaj.Application.Features.Prescriptions.Queries.GetNearbyPrescriptions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Elaaj.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] 
    public class PrescriptionsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PrescriptionsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreatePrescriptionCommand command)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { Message = "برجاء تسجيل الدخول أولاً." });
            }

            command.UserId = userId;

            var prescriptionId = await _mediator.Send(command);

            return Ok(new
            {
                PrescriptionId = prescriptionId,
                Message = "تم إرسال روشتتك للصيدليات القريبة بنجاح، في انتظار الردود."
            });
        }

        [HttpGet("nearby/{pharmacyId}")]
        public async Task<IActionResult> GetNearbyPrescriptions(Guid pharmacyId, [FromQuery] double radius = 5)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var query = new GetNearbyPrescriptionsQuery
            {
                UserId = userId,
                PharmacyId = pharmacyId,
                RadiusInKm = radius
            };

            try
            {
                var result = await _mediator.Send(query);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, new { Message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
        }


        [HttpPost("{id}/replies")]
        public async Task<IActionResult> AddReply(Guid id, [FromBody] CreatePrescriptionReplyCommand command)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            command.PrescriptionId = id;
            command.UserId = userId;

            try
            {
                var replyId = await _mediator.Send(command);
                return Ok(new { ReplyId = replyId, Message = "تم إرسال عرضك للمريض بنجاح." });
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, new { Message = ex.Message });
            }
            catch (Exception ex) when (ex is ArgumentException || ex is InvalidOperationException)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }


        [HttpPut("{prescriptionId}/replies/{replyId}/accept")]
        public async Task<IActionResult> AcceptReply(Guid prescriptionId, Guid replyId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var command = new AcceptPrescriptionReplyCommand
            {
                PrescriptionId = prescriptionId,
                ReplyId = replyId,
                UserId = userId
            };

            try
            {
                await _mediator.Send(command);
                return Ok(new { Message = "تم قبول العرض بنجاح وإغلاق الطلب. سيتم تجهيز طلبك من قبل الصيدلية." });
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, new { Message = ex.Message });
            }
            catch (Exception ex) when (ex is ArgumentException || ex is InvalidOperationException)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpGet("my-prescriptions")]
        public async Task<IActionResult> GetMyPrescriptions()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var query = new GetMyPrescriptionsQuery { UserId = userId };

            var result = await _mediator.Send(query);

            return Ok(result);
        }
    }
}