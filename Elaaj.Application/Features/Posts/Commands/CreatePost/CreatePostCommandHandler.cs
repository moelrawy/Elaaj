using Elaaj.Application.Interfaces;
using Elaaj.Application.Users;
using Elaaj.Domain.Entities;
using Elaaj.Domain.Interfaces;
using MediatR;
using Restaurants.Domain.Constants;

namespace Elaaj.Application.Features.Posts.Commands.CreatePost;

public class CreatePostCommandHandler : IRequestHandler<CreatePostCommand, int>
{
    private readonly IGenericRepository<Post> _repository;
    private readonly IFileService _fileService;
    private readonly IUserContext _userContext;

    public CreatePostCommandHandler(
        IGenericRepository<Post> repository,
        IFileService fileService,
        IUserContext userContext)
    {
        _repository = repository;
        _fileService = fileService;
        _userContext = userContext;
    }

    public async Task<int> Handle(CreatePostCommand request, CancellationToken cancellationToken)
    {
        // 1. Get current user from Token, not from Request
        var currentUser = _userContext.GetCurrentUser();
        if (currentUser == null)
            throw new UnauthorizedAccessException("يجب تسجيل الدخول أولاً");

        // 2. Only regular User can create a Post, not a Pharmacist
        if (currentUser.IsInRole(UserRoles.PharmacyOwner) && !currentUser.IsInRole(UserRoles.User))
            throw new UnauthorizedAccessException("الصيدلاني مش المفروض يعمل استشارة");

        // 3. Upload image if provided
        string? imageUrl = null;
        if (request.File != null && request.File.Length > 0)
        {
            imageUrl = await _fileService.UploadFileAsync(request.File, "generalposts");
        }

        // 4. Save the Post to the Database
        var post = new Post
        {
            UserId = currentUser.Id, // Get UserId from Token, not from Request
            Content = request.Content,
            ImageUrl = imageUrl,
            CreatedAt = DateTime.UtcNow
        };

        await _repository.AddAsync(post);
        await _repository.SaveChangesAsync();

        return post.Id;
    }
}