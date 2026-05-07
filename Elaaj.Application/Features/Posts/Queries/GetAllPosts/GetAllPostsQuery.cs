using Elaaj.Application.Features.Posts.DTOs;
using Elaaj.Application.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elaaj.Application.Features.Posts.Queries.GetAllPosts;

public class GetAllPostsQuery :  IRequest<PagedResult<PostDto>>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
