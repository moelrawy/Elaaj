using Elaaj.Application.Features.Posts.Commands.CreatePost;
using Elaaj.Application.Features.Posts.Queries.GetAllPosts;
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
        public async Task<IActionResult> Create([FromForm] CreatePostCommand command)
        {

            // No need to get userId here, Handler gets it from Token
            var postId = await _mediator.Send(command);
            return Ok(new { PostId = postId, Message = "تم نشر استفسارك بنجاح." });
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPosts()
        {
            var query = new GetAllPostsQuery();
            var result = await _mediator.Send(query);

            return Ok(result);
        }
    }
}
