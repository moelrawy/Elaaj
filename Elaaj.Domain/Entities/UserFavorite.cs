using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elaaj.Domain.Entities;

public class UserFavorite
{
    public int Id { get; set; }
    public int PatientId { get; set; }
    public Patient Patient { get; set; } = null!;

    public int PharmacyId { get; set; }
    public Pharmacy Pharmacy { get; set; } = null!;
    public DateTime AddedAt { get; set; } = DateTime.UtcNow;
}
