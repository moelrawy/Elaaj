using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Elaaj.Application.Interfaces;
using Elaaj.Domain.Enums;
using MediatR;

namespace Elaaj.Application.Features.Notifications.Commands.CreateNotification
{
    public record CreateNotificationCommand(
        string UserId,
        string Title,
        string Message,
        NotificationType Type,
        string? RelatedEntityId = null,
        string? RelatedEntityType = null) : IRequest<Guid>;
    public class CreateNotificationCommandHandler : IRequestHandler<CreateNotificationCommand, Guid>
    {
        private readonly INotificationService _notificationService;
        public CreateNotificationCommandHandler(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public async Task<Guid> Handle(CreateNotificationCommand request, CancellationToken cancellationToken)
        {
            await _notificationService.SendToUserAsync(
                request.UserId,
                request.Title,
                request.Message,
                request.Type,
                request.RelatedEntityId,
                request.RelatedEntityType,
                cancellationToken
                );
            return Guid.NewGuid();
        }
    }
}
