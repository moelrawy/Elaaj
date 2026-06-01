using MediatR;

namespace Elaaj.Application.Features.Posts.Commands.DeletePost;

public class DeletePostCommand : IRequest<bool>
{
    public int Id { get; set; }
}