using Elaaj.Application.Users;
using Elaaj.Domain.Entities;
using Elaaj.Domain.Interfaces;
using MediatR;

namespace Elaaj.Application.Features.Posts.Commands.DeletePost;

public class DeletePostCommandHandler : IRequestHandler<DeletePostCommand, bool>
{
    private readonly IGenericRepository<Post> _repository;
    private readonly IUserContext _userContext;

    public DeletePostCommandHandler(IGenericRepository<Post> repository, IUserContext userContext)
    {
        _repository = repository;
        _userContext = userContext;
    }

    public async Task<bool> Handle(DeletePostCommand request, CancellationToken cancellationToken)
    {
        var currentUser = _userContext.GetCurrentUser();
        if (currentUser == null)
            throw new UnauthorizedAccessException("ÌÃ»  ”ÃÌ· «·œŒÊ· √Ê·«");

        var post = await _repository.GetByIdAsync(request.Id);
        if (post == null)
            throw new KeyNotFoundException("«·«” ›”«— €Ì— „ÊÃÊœ.");

        if (post.UserId != currentUser.Id)
            throw new UnauthorizedAccessException("·« Ì„ﬂ‰ﬂ Õ–› Â–« «·«” ›”«—.");

        _repository.Delete(post);
        await _repository.SaveChangesAsync();

        return true;
    }
}