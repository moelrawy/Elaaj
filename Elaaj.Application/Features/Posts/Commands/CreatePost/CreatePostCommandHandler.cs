using Elaaj.Application.Interfaces;
using Elaaj.Application.Users;
using Elaaj.Domain.Entities;
using Elaaj.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elaaj.Application.Features.Posts.Commands.CreatePost;

public class CreatePostCommandHandler : IRequestHandler<CreatePostCommand, int>
{
    private readonly IGenericRepository<Post> _repository;
    private readonly IFileService _fileService;
    private readonly IUserContext _userContext;

    public CreatePostCommandHandler(IGenericRepository<Post> repository, IFileService fileService, IUserContext userContext)
    {
        _repository = repository;
        _fileService = fileService;
        _userContext = userContext;
    }

    public async Task<int> Handle(CreatePostCommand request, CancellationToken cancellationToken)
    {
        Users.CurrentUser? currentUser = _userContext.GetCurrentUser();
        if (currentUser == null)
            throw new UnauthorizedAccessException("يجب تسجيل الدخول أولاً");

        //if (currentUser.IsInRole(UserRoles.PharmacyOwner) && !currentUser.IsInRole(UserRoles.User))
        //    throw new UnauthorizedAccessException("الصيدلاني مش المفروض يعمل استشارة");

        string? imageUrl = null;
        if (request.File != null && request.File.Length > 0)
        {
            imageUrl = await _fileService.UploadFileAsync(request.File, "generalposts");
        }

        var Post = new Post
        {
            UserId = currentUser.Id,
            Content = request.Content,
            ImageUrl = imageUrl,
            CreatedAt = DateTime.UtcNow
        };

        await _repository.AddAsync(Post);
        await _repository.SaveChangesAsync();

        return Post.Id;
    }
}