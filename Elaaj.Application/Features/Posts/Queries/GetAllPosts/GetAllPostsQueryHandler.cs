using AutoMapper;
using Elaaj.Application.Features.Posts.DTOs;
using Elaaj.Domain.Entities;
using Elaaj.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elaaj.Application.Features.Posts.Queries.GetAllPosts;

public class GetAllPostsQueryHandler : IRequestHandler<GetAllPostsQuery, IEnumerable<PostDto>>
{
    private readonly IGenericRepository<Post> _repository;
    private readonly IMapper _mapper;

    public GetAllPostsQueryHandler(IGenericRepository<Post> repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<PostDto>> Handle(GetAllPostsQuery request, CancellationToken cancellationToken)
    {
        var posts = await _repository.GetWhereAsync(x => true, p => p.Replies);

        var orderedPosts = posts.OrderByDescending(p => p.CreatedAt);

        return _mapper.Map<IEnumerable<PostDto>>(orderedPosts);
    }
}
