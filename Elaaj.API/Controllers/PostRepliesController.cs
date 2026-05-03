using MediatR;
using Microsoft.AspNetCore.Mvc;
using Elaaj.Application.Features.PostReplies.Commands.CreatePostReply;
namespace Elaaj.API.Controllers
{
    [ApiController]
    [Route("api/[cintroller")]
    public class PostRepliesController:ControllerBase
    {
        private readonly ISender _mediator;
        public PostRepliesController(ISender mediator)
        {
            _mediator = mediator;
        }
        [HttpPost]
        public async Task<IActionResult> CreateReply([FromBody] CreatePostReplyCommand command)
        {
            var result = await _mediator.Send(command);

            return Ok(result);

        }
    }
}
