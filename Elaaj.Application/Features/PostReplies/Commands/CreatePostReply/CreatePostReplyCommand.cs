using MediatR;
namespace Elaaj.Application.Features.PostReplies.Commands.CreatePostReply
{
    public class CreatePostReplyCommand : IRequest<Guid>
    {
        public required string ReceiverId { get; set; }
        public required string ReplyContent { get; set; }
        public int PostId { get; set; }
    }
}
