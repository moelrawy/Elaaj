using Elaaj.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elaaj.Application.Features.Prescriptions.DTOs;

public record MyPrescriptionDto
{
    // دي query-specific DTO عشان نقدر نعرض بيانات الروشتة بشكل مرتب وسهل في الواجهة الخاصة المستخدمين لعرض روشتاتهم و الردود والعروض الخاصة بيها
    public Guid Id { get; init; }
    public string ImageUrl { get; init; } = string.Empty;
    public string Notes { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
    public bool IsResolved { get; init; }
    public PrescriptionStatus Status { get; init; }
    public ICollection<PrescriptionReplyDto> Replies { get; set; } = new List<PrescriptionReplyDto>();
}
