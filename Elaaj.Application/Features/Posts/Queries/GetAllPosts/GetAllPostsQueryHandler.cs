using AutoMapper;
using Elaaj.Application.Features.Posts.DTOs;
using Elaaj.Application.Models;
using Elaaj.Domain.Entities;
using Elaaj.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elaaj.Application.Features.Posts.Queries.GetAllPosts;

public class GetAllPostsQueryHandler : IRequestHandler<GetAllPostsQuery, PagedResult<PostDto>>
{
    private readonly IGenericRepository<Post> _repository;
    private readonly IMapper _mapper;

    public GetAllPostsQueryHandler(IGenericRepository<Post> repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<PagedResult<PostDto>> Handle(GetAllPostsQuery request, CancellationToken cancellationToken)
    {
        var (items, totalCount) = await _repository.GetPagedAsync(
            request.PageNumber,
            request.PageSize,
            predicate: null, 
            orderBy: q => q.OrderByDescending(p => p.CreatedAt),
            includes: p => p.Replies
        );

        var dtos = _mapper.Map<IEnumerable<PostDto>>(items);

        return new PagedResult<PostDto>
        {
            Items = dtos,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }
}