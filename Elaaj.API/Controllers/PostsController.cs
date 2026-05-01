using Elaaj.Application.Features.Posts.Commands.CreatePost;
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
    public class PostsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PostsController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreatePostCommand command)
        {
            
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { Message = "برجاء تسجيل الدخول أولاً." });
            }

            command.UserId = userId;

            var postId = await _mediator.Send(command);

            return Ok(new { PostId = postId, Message = "تم نشر استفسارك بنجاح." });
        }
    }
}
