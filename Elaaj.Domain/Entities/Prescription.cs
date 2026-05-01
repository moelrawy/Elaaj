using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elaaj.Domain.Entities;

public class Prescription
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string UserId { get; set; } = string.Empty;

    public string ImageUrl { get; set; } = string.Empty; 
    public string Notes { get; set; } = string.Empty; 

    public double Latitude { get; set; }
    public double Longitude { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // هل المريض وافق على عرض من صيدلية وقفل الطلب؟
    public bool IsResolved { get; set; } = false;

    public ICollection<PrescriptionReply> Replies { get; set; } = new List<PrescriptionReply>();
}