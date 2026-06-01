using AutoMapper;
using Elaaj.Application.Features.Posts.DTOs;
using Elaaj.Application.Models;
using Elaaj.Application.Users;
using Elaaj.Domain.Entities;
using Elaaj.Domain.Interfaces;
using MediatR;

namespace Elaaj.Application.Features.Posts.Queries.GetMyPosts;

public class GetMyPostsQueryHandler : IRequestHandler<GetMyPostsQuery, PagedResult<PostDto>>
{
    private readonly IGenericRepository<Post> _repository;
    private readonly IMapper _mapper;
    private readonly IUserContext _userContext;

    public GetMyPostsQueryHandler(IGenericRepository<Post> repository, IMapper mapper, IUserContext userContext)
    {
        _repository = repository;
        _mapper = mapper;
        _userContext = userContext;
    }

    public async Task<PagedResult<PostDto>> Handle(GetMyPostsQuery request, CancellationToken cancellationToken)
    {
        var currentUser = _userContext.GetCurrentUser();
        if (currentUser == null)
            throw new UnauthorizedAccessException("íÌÈ ÊÓÌíá ÇáÏÎæá ÃæáÇð");

        var (items, totalCount) = await _repository.GetPagedAsync(
            request.PageNumber,
            request.PageSize,
            predicate: p => p.UserId == currentUser.Id,
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