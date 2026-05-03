using MediatR;
namespace Elaaj.Application.Features.PostReplies.Commands.CreatePostReply
{
    public class CreatePostReplyCommand : IRequest<Guid>
    {
        public string PatientId { get; set; } 
        public string ReplyContent { get; set; }
        public string PostId { get; set; }
    }
}
