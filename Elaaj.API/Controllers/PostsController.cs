using Azure.Core;
using Elaaj.Application.Features.Posts.Commands.CreatePost;
using Elaaj.Application.Features.Posts.Commands.DeletePost;
using Elaaj.Application.Features.Posts.Commands.UpdatePost;
using Elaaj.Application.Features.Posts.DTOs;
using Elaaj.Application.Features.Posts.Queries.GetAllPosts;
using Elaaj.Application.Features.Posts.Queries.GetMyPosts;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
            var postId = await _mediator.Send(command);
            return Ok(new { PostId = postId, Message = "تم نشر استفسارك بنجاح." });
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPosts([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var query = new GetAllPostsQuery
            {
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("my-posts")]
        public async Task<IActionResult> GetMyPosts([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var query = new GetMyPostsQuery
            {
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromForm] UpdatePostDto dto)
        {
            var command = new UpdatePostCommand
            {
                Id = id,
                Content = dto.Content,
                File = dto.File
            };

            await _mediator.Send(command);
            return Ok(new { Message = "تم تعديل استفسارك بنجاح." });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var command = new DeletePostCommand { Id = id };
            await _mediator.Send(command);

            return Ok(new { Message = "تم حذف الاستفسار بنجاح." });
        }
    }
}
