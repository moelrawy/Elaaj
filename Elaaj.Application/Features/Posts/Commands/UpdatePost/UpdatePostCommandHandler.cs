using Elaaj.Application.Interfaces;
using Elaaj.Application.Users;
using Elaaj.Domain.Entities;
using Elaaj.Domain.Interfaces;
using MediatR;

namespace Elaaj.Application.Features.Posts.Commands.UpdatePost;

public class UpdatePostCommandHandler : IRequestHandler<UpdatePostCommand, bool>
{
    private readonly IGenericRepository<Post> _repository;
    private readonly IFileService _fileService;
    private readonly IUserContext _userContext;

    public UpdatePostCommandHandler(IGenericRepository<Post> repository, IFileService fileService, IUserContext userContext)
    {
        _repository = repository;
        _fileService = fileService;
        _userContext = userContext;
    }

    public async Task<bool> Handle(UpdatePostCommand request, CancellationToken cancellationToken)
    {
        var currentUser = _userContext.GetCurrentUser();
        if (currentUser == null)
            throw new UnauthorizedAccessException("íÌÈ ÊÓÌíá ÇáÏÎæá ÃæáÇð");

        var post = await _repository.GetByIdAsync(request.Id);
        if (post == null)
            throw new KeyNotFoundException("ÇáÇÓÊÝÓÇÑ ÛíÑ ãæÌæÏ.");

        if (post.UserId != currentUser.Id)
            throw new UnauthorizedAccessException("áÇ íãßäß ÊÚÏíá åÐÇ ÇáÇÓÊÝÓÇÑ.");

        if (request.File != null && request.File.Length > 0)
        {
            post.ImageUrl = await _fileService.UploadFileAsync(request.File, "generalposts");
        }

        post.Content = request.Content;

        _repository.Update(post);
        await _repository.SaveChangesAsync();

        return true;
    }
}