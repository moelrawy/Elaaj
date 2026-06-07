using System;

namespace Elaaj.Application.Features.Prescriptions.DTOs;

public record PrescriptionReplyDto
{
    public Guid Id { get; set; }
    public Guid PharmacyId { get; set; }
    public string PharmacyName { get; set; } = string.Empty; 
    public string PharmacyImageUrl { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public decimal? TotalPrice { get; set; }
    public bool IsAvailable { get; set; }
    public DateTime ReplyTime { get; set; }
}