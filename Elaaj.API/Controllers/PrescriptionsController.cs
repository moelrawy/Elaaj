using Elaaj.Application.Features.Prescriptions.Commands.AcceptReply;
using Elaaj.Application.Features.Prescriptions.Commands.CreatePrescription;
using Elaaj.Application.Features.Prescriptions.Commands.CreateReply;
using Elaaj.Application.Features.Prescriptions.Commands.UpdatePrescriptionStatus;
using Elaaj.Application.Features.Prescriptions.Queries.GetMyPrescriptions;
using Elaaj.Application.Features.Prescriptions.Queries.GetNearbyPrescriptions;
using Elaaj.Domain.Constants;
using Elaaj.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Elaaj.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Must be logged in for all endpoints
    public class PrescriptionsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PrescriptionsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // Only regular users and platform owner can create prescriptions
        [HttpPost]
        [Authorize(Roles = $"{UserRoles.User},{UserRoles.Owner}")]
        public async Task<IActionResult> Create([FromForm] CreatePrescriptionCommand command)
        {
            // Handler gets UserId from Token
            var prescriptionId = await _mediator.Send(command);

            return Ok(new
            {
                PrescriptionId = prescriptionId,
                Message = "تم إرسال روشتتك للصيدليات القريبة بنجاح، في انتظار الردود."
            });
        }

        // Only pharmacy staff can get nearby prescriptions
        // Users cannot see all prescriptions, only pharmacies in their area see them
        [HttpGet("nearby/{pharmacyId}")]
        [Authorize(Roles = $"{UserRoles.PharmacyOwner},{UserRoles.PharmacyAdmin},{UserRoles.Owner}")]
        public async Task<IActionResult> GetNearbyPrescriptions(Guid pharmacyId, [FromQuery] double radius = 5)
        {
            // Handler gets UserId from Token
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

        // Only pharmacy staff can reply to prescriptions
        [HttpPost("{id}/replies")]
        [Authorize(Roles = $"{UserRoles.PharmacyOwner},{UserRoles.PharmacyAdmin},{UserRoles.Owner}")]
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

        // Only the prescription owner (User or Owner) can accept replies
        [HttpPut("{prescriptionId}/replies/{replyId}/accept")]
        [Authorize(Roles = $"{UserRoles.User},{UserRoles.Owner}")]
        public async Task<IActionResult> AcceptReply(Guid prescriptionId, Guid replyId)
        {
            // Handler gets UserId from Token
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

        // Only the prescription owner can see their own prescriptions
        [HttpGet("my-prescriptions")]
        [Authorize(Roles = $"{UserRoles.User},{UserRoles.Owner}")]
        public async Task<IActionResult> GetMyPrescriptions([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            // Handler gets UserId from Token
            var query = new GetMyPrescriptionsQuery
            {
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            var result = await _mediator.Send(query);
            return Ok(result);
        }

        // Only pharmacy staff or owner can update status
        [HttpPatch("{id}/status")]
        [Authorize(Roles = $"{UserRoles.PharmacyOwner},{UserRoles.PharmacyAdmin},{UserRoles.Owner}")]
        public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] PrescriptionStatus newStatus)
        {
            // Handler gets UserId from Token and verifies they are pharmacy admin for the reply
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
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }
<<<<<<< Updated upstream
=======

        // Only the prescription owner can update their prescriptions
        [HttpPut("{id}")]
        [Authorize(Roles = $"{UserRoles.User},{UserRoles.Owner}")]
        public async Task<IActionResult> UpdatePrescription(Guid id, [FromBody] string notes)
        {
            // Handler gets UserId from Token and verifies ownership
            var command = new UpdatePrescriptionCommand
            {
                Id = id,
                Notes = notes
            };

            try
            {
                await _mediator.Send(command);
                return Ok(new { Message = "تم تعديل الروشتة بنجاح." });
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        // Only the prescription owner can delete their prescriptions
        [HttpDelete("{id}")]
        [Authorize(Roles = $"{UserRoles.User},{UserRoles.Owner}")]
        public async Task<IActionResult> DeletePrescription(Guid id)
        {
            // Handler gets UserId from Token and verifies ownership
            var command = new DeletePrescriptionCommand
            {
                Id = id
            };

            try
            {
                await _mediator.Send(command);
                return Ok(new { Message = "تم حذف الروشتة بنجاح." });
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }
>>>>>>> Stashed changes
    }
}