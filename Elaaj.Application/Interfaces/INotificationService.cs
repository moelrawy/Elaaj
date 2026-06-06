using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Elaaj.Domain.Enums;


namespace Elaaj.Application.Interfaces
{
    public interface INotificationService
    {
        Task SendToUserAsync(string userId, string title, string message, NotificationType type,string? relatedEntityId=null,string? relatedEntityType=null,CancellationToken cancellationToken=default );
        Task SendToAllAsync(string title, string message, NotificationType type,CancellationToken cancellationToken = default);
        Task SendToGroupAsync(string groupName, string title, string message, NotificationType type, CancellationToken cancellationToken = default);
    }
}

