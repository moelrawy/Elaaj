using Microsoft.AspNetCore.Identity;

namespace Elaaj.Domain.Entities;

public class User : IdentityUser
{
    public string FullName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateOnly? DateOfBirth { get; set; }
    public virtual ICollection<Post> Posts { get; set; } = new List<Post>();
    public ICollection<PharmacyAdmin> ManagedPharmacies { get; set; } = new List<PharmacyAdmin>();
}
