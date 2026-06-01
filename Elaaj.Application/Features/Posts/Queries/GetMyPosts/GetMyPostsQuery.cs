using Elaaj.Application.Features.Posts.DTOs;
using Elaaj.Application.Models;
using MediatR;

namespace Elaaj.Application.Features.Posts.Queries.GetMyPosts;

public class GetMyPostsQuery : IRequest<PagedResult<PostDto>>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}