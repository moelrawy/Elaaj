using Elaaj.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace Elaaj.Application.Interfaces
{
    public interface IApplicationDbContext
    {
        DbSet<Notification> Notifications { get; }
        DbSet<Pharmacy> Pharmacies { get; }
        DbSet<Post> Posts { get; }
        DbSet<PostReply> PostReplies { get; }
        DbSet<PharmacyMessage> PharmacyMessages { get; }
        DbSet<UserFavorite> UserFavorites { get; }
        DbSet<PharmacyAdmin> PharmacyAdmins { get; }
        DbSet<PrescriptionReply> PrescriptionReplies { get; }
        DbSet<Prescription> Prescriptions { get; }
        DbSet<UserDevices> UserDevices { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}