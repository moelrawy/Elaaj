using Elaaj.Application.Interfaces;
using Elaaj.Domain.Entities;
using Elaaj.Domain.Enums;
using FirebaseAdmin.Messaging;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Elaaj.Infrastructure.Hubs;


namespace Elaaj.Infrastructure.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IApplicationDbContext _context;
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(IApplicationDbContext context, IHubContext<NotificationHub> hubContext, ILogger<NotificationService> logger)
        {
            _context = context;
            _hubContext = hubContext;
            _logger = logger;

        }
        public async Task SendToUserAsync(string userId, string title, string message, NotificationType type, string? relatedEntityId = null, string? relatedEntityType = null, CancellationToken cancellationToken = default)
        {
            var notification = new Elaaj.Domain.Entities.Notification(userId, title, message, type, relatedEntityId, relatedEntityType);
            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync(cancellationToken);
            try
            {
                await _hubContext.Clients.User(userId).SendAsync("ReceiveNotification", new
                {
                    id = notification.Id,
                    title = notification.Title,
                    message = notification.Message,
                    type = notification.Type.ToString(),
                    createdAt = notification.CreatedAt,

                }, cancellationToken);
                _logger.LogInformation("SignalR Notification sent to user {UserId}", userId);

                await SendFirebaseFallbackAsync(userId, title, message, type, relatedEntityId, relatedEntityType, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send SignalR notification to user {UserId}", userId);
            }

        }
        private async Task SendFirebaseFallbackAsync(string userId, string title, string message, NotificationType type, string? relatedEntityId, string? relatedEntityType, CancellationToken cancellationToken)
        {
            var deviceTokens = await _context.UserDevices
                .Where(d => d.UserId == userId)
                .Select(d => d.DeviceToken)
                .ToListAsync(cancellationToken);

            if (deviceTokens.Any())
            {
                var fcmMessage = new MulticastMessage()
                {
                    Tokens = deviceTokens,
                    Notification = new FirebaseAdmin.Messaging.Notification()
                    {
                        Title = title,
                        Body = message
                    },
                    Data = new Dictionary<string, string> { { "type", "Notification" } }
                };
                await FirebaseMessaging.DefaultInstance.SendMulticastAsync(fcmMessage, cancellationToken);

            }
        }
        public async Task SendToAllAsync(string title, string message, NotificationType type, CancellationToken cancellationToken = default)
        {
            await _hubContext.Clients.All.SendAsync("ReceiveNotification", new
            {
                title,
                message,
                type = type.ToString(),
            }, cancellationToken);
        }
        public async Task SendToGroupAsync(string groupName, string title, string message, NotificationType type, CancellationToken cancellationToken = default)
        {
            await _hubContext.Clients.Group(groupName).SendAsync("ReceiveNotification", new
            {
                title,
                message,
                type = type.ToString(),
            }, cancellationToken);
        }
    }
}
