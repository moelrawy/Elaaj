using Elaaj.Application.Features.PostReplies.Commands.CreatePostReply;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Restaurants.Domain.Constants;
namespace Elaaj.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PostRepliesController:ControllerBase
    {
        private readonly ISender _mediator;
        public PostRepliesController(ISender mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [Authorize(Roles = $"{UserRoles.PharmacyOwner},{UserRoles.PharmacyAdmin}")] //  Only pharmacy staff
        public async Task<IActionResult> CreateReply([FromBody] CreatePostReplyCommand command)
        {
            var result = await _mediator.Send(command);

            return Ok(result);

        }
    }
}
