using MediatR;
using Elaaj.Application.Interfaces;
namespace Elaaj.Application.Features.PostReplies.Commands.CreatePostReply
{
    public class CreatePostReplyCommandHandler : IRequestHandler<CreatePostReplyCommand,Guid>
    {
        private readonly INotificationService _notificationService;
        public CreatePostReplyCommandHandler(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }
        public async Task <Guid> Handle(CreatePostReplyCommand request,CancellationToken cancellationToken)
        {
            await _notificationService.SendReplyNotification(request.PatientId, "تم الرد علي استشارتك");
            return Guid.NewGuid();
        }

    }
}
