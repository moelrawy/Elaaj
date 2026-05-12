using Elaaj.Domain.Constants;

namespace Elaaj.Domain.Entities;

public class PharmacyAdmin
{
    public string UserId { get; set; } = string.Empty;
    public Guid PharmacyId { get; set; }
    public string Role { get; set; } = UserRoles.PharmacyOwner;
    public DateTime AssignedAt { get; set; } = DateTime.UtcNow;

    public User User { get; set; } = null!;
    public Pharmacy Pharmacy { get; set; } = null!;
}
