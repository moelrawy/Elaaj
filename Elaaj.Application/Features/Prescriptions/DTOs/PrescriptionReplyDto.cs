using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elaaj.Application.Features.Prescriptions.DTOs;

public record PrescriptionReplyDto
{
    // دي query-specific DTO عشان نقدر نعرض بيانات الردود بشكل مرتب وسهل في الواجهة الخاصة المستخدمين لردود وعروض الصيدليات على الروشتة
    public Guid Id { get; init; }
    public Guid PharmacyId { get; init; }
    public string PharmacyName { get; init; } = string.Empty; 
    public string Message { get; init; } = string.Empty;
    public decimal? TotalPrice { get; init; }
    public bool IsAvailable { get; init; }
    public DateTime ReplyTime { get; init; }
}
