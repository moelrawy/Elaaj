using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elaaj.Domain.Entities;

public class UserFavorite
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public User User { get; set; } = null!;

    public Guid PharmacyId { get; set; }
    public Pharmacy Pharmacy { get; set; } = null!;
    public DateTime AddedAt { get; set; } = DateTime.UtcNow;
}
