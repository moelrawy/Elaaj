using Elaaj.Application.Features.Prescriptions.Commands.AcceptReply;
using Elaaj.Application.Features.Prescriptions.Commands.CreatePrescription;
using Elaaj.Application.Features.Prescriptions.Commands.CreateReply;
using Elaaj.Application.Features.Prescriptions.Commands.UpdatePrescriptionStatus;
using Elaaj.Application.Features.Prescriptions.Queries.GetMyPrescriptions;
using Elaaj.Application.Features.Prescriptions.Queries.GetNearbyPrescriptions;
using Elaaj.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Restaurants.Domain.Constants;
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
        [Authorize(Roles = $"{UserRoles.User}")]
        public async Task<IActionResult> Create([FromForm] CreatePrescriptionCommand command)
        {

            var prescriptionId = await _mediator.Send(command);

            return Ok(new
            {
                PrescriptionId = prescriptionId,
                Message = "تم إرسال روشتتك للصيدليات القريبة بنجاح، في انتظار الردود."
            });
        }
        //this endpoint is for pharmacy staff to get nearby prescriptions to their pharmacy, they can specify a radius in km, default is 5km
        [HttpGet("nearby/{pharmacyId}")]
        public async Task<IActionResult> GetNearbyPrescriptions(Guid pharmacyId, [FromQuery] double radius = 5)
        {
            

            var query = new GetNearbyPrescriptionsQuery
            {
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
        [Authorize(Roles = $"{UserRoles.PharmacyOwner},{UserRoles.PharmacyAdmin}")]
        public async Task<IActionResult> AddReply(Guid id, [FromBody] CreatePrescriptionReplyCommand command)
        {
            command.PrescriptionId = id;
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

            var command = new AcceptPrescriptionReplyCommand
            {
                PrescriptionId = prescriptionId,
                ReplyId = replyId,
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
        [Authorize(Roles = $"{UserRoles.User}")]
        public async Task<IActionResult> GetMyPrescriptions([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {

            var query = new GetMyPrescriptionsQuery
            {
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpPatch("{id}/status")] 
        public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] PrescriptionStatus newStatus)
        {

            var command = new UpdatePrescriptionStatusCommand
            {
                PrescriptionId = id,
                NewStatus = newStatus,
            };

            try
            {
                await _mediator.Send(command);
                return Ok(new { Message = "تم تحديث حالة الطلب بنجاح." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }
    }
}