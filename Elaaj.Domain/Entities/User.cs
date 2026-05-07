using Microsoft.AspNetCore.Identity;

namespace Elaaj.Domain.Entities;

public class User : IdentityUser
{
    public string FullName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateOnly? DateOfBirth { get; set; }

    public string? imageUrl { get; set; }
    public string? Address { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public virtual ICollection<Post> Posts { get; set; } = new List<Post>();
    public ICollection<PharmacyAdmin> ManagedPharmacies { get; set; } = new List<PharmacyAdmin>();
    public virtual ICollection<Pharmacy> OwnedPharmacies { get; set; } = new List<Pharmacy>();
    public virtual ICollection<UserFavorite> Favorites { get; set; } = new List<UserFavorite>();
}