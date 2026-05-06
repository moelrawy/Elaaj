using Elaaj.Application.Features.Posts.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elaaj.Application.Features.Posts.Queries.GetAllPosts;

public class GetAllPostsQuery :  IRequest<IEnumerable<PostDto>>
{
}
