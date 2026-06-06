using Elaaj.Domain.Enums;
using System;


namespace Elaaj.Application.DTOs
{
    public record NotificationDto(
        Guid Id,
        string USerId,
        string Title,
        string Message,
        NotificationType Type,
        bool IsRead,
        DateTime CreateAt,
        DateTime? ReadAt,
        string? RelatedEntityId ,
        string? RelatedEntityType 
    );
}
