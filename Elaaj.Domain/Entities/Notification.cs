using Elaaj.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elaaj.Domain.Entities
{
   public class Notification
    {
    public Guid Id { get;private set; }
        public string UserId { get;private set; }
        public string Title { get;private set; }
        public string Message { get;private set; }
        public NotificationType Type { get;private set; }
        public bool IsRead { get;private set; }
        public DateTime CreatedAt { get;private set; }
        public DateTime? ReadAt { get;private set; }

        public string? RelatedEntityId { get;private set; }
        public string? RelatedEntityType { get;private set; }

        private Notification() { }
        public Notification(string userId, string title, string message, NotificationType type, string? relatedEntityId = null, string? relatedEntityType = null)
        {
            Id = Guid.NewGuid();
            UserId = userId;
            Title = title;
            Message = message;
            Type = type;
            IsRead = false;
            CreatedAt = DateTime.UtcNow;
            RelatedEntityId = relatedEntityId;
            RelatedEntityType = relatedEntityType;
        }

        public void MarkAsRead()
        {
            if (!IsRead)
            {
                IsRead = true;
                ReadAt = DateTime.UtcNow;
            }
        }
    }
}
