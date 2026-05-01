using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elaaj.Domain.Entities;

public class PrescriptionReply
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid PrescriptionId { get; set; }
    public Prescription Prescription { get; set; } = null!;

    public Guid PharmacyId { get; set; }
    public Pharmacy Pharmacy { get; set; } = null!;

    public string Message { get; set; } = string.Empty;
    public decimal? TotalPrice { get; set; }
    public bool IsAvailable { get; set; } = true;

    public DateTime ReplyTime { get; set; } = DateTime.UtcNow;
}