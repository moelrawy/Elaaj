using System;

namespace Elaaj.Application.Features.PostReplies.DTOs;

public record PostReplyDto
{
    public int Id { get; init; } // Changed from Guid to int
    public string PharmacyName { get; init; } = string.Empty; 
    public string Message { get; init; } = string.Empty; // Renamed to Message to match entity
    public DateTime CreatedAt { get; init; }
}