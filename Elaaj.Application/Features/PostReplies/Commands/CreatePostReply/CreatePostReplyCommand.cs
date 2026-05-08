using MediatR;
namespace Elaaj.Application.Features.PostReplies.Commands.CreatePostReply
{
    public class CreatePostReplyCommand : IRequest<int>
    {
        public required string ReceiverId { get; set; }
        public required string ReplyContent { get; set; }
        public Guid PharmacyId { get; set; }
        public int PostId { get; set; }
    }
}
