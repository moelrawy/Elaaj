using Elaaj.Application.Features.Prescriptions.Commands.CreatePrescription;
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
    }
}