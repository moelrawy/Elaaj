using AutoMapper;
using Elaaj.Application.Features.Posts.DTOs;
using Elaaj.Application.Users;
using Elaaj.Domain.Entities;
using Elaaj.Domain.Interfaces;
using MediatR;
using Restaurants.Domain.Constants;

namespace Elaaj.Application.Features.Posts.Queries.GetAllPosts;

public class GetAllPostsQueryHandler : IRequestHandler<GetAllPostsQuery, IEnumerable<PostDto>>
{
    private readonly IGenericRepository<Post> _repository;
    private readonly IMapper _mapper;
    private readonly IUserContext _userContext;

    public GetAllPostsQueryHandler(
        IGenericRepository<Post> repository,
        IMapper mapper,
        IUserContext userContext)
    {
        _repository = repository;
        _mapper = mapper;
        _userContext = userContext;
    }

    public async Task<IEnumerable<PostDto>> Handle(GetAllPostsQuery request, CancellationToken cancellationToken)
    {
        // 1. Verify user is authenticated
        var currentUser = _userContext.GetCurrentUser();
        if (currentUser == null)
            throw new UnauthorizedAccessException("يجب تسجيل الدخول أولاً");

        // 2. Get all posts ordered by date - all roles can see all posts
        var posts = await _repository.GetWhereAsync(x => true, p => p.Replies);

        return _mapper.Map<IEnumerable<PostDto>>(posts.OrderByDescending(p => p.CreatedAt));
    }
}