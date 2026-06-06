using Elaaj.Domain.Entities;
using Elaaj.infrastructure.Data.Configurations;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Elaaj.infrastructure.Data;

public class ApplicationDbContext : IdentityDbContext<User>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Notification> Notifications { get; set; }
    public DbSet<Pharmacy> Pharmacies { get; set; }
    public DbSet<Post> Posts { get; set; }
    public DbSet<PostReply> PostReplies { get; set; }
    public DbSet<PharmacyMessage> PharmacyMessages { get; set; }
    public DbSet<UserFavorite> UserFavorites { get; set; }
    public DbSet<PharmacyAdmin> PharmacyAdmins { get; set; }
    public DbSet<PrescriptionReply> PrescriptionReplies { get; set; }
    public DbSet<Prescription> Prescriptions { get; set; }
    public DbSet<UserDevices> UserDevices { get; set; } 
    public DbSet<ChatMessage> ChatMessages { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder); // ضروري جداً لجدول المستخدمين

        // 1. إعدادات PharmacyAdmin (المفتاح المركب)
        modelBuilder.Entity<PharmacyAdmin>()
            .HasKey(pa => new { pa.UserId, pa.PharmacyId });

        modelBuilder.Entity<PharmacyAdmin>()
            .HasOne(pa => pa.User)
            .WithMany(u => u.ManagedPharmacies)
            .HasForeignKey(pa => pa.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<PharmacyAdmin>()
            .HasOne(pa => pa.Pharmacy)
            .WithMany(p => p.Admins)
            .HasForeignKey(pa => pa.PharmacyId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<PharmacyAdmin>()
           .Property(pa => pa.UserId)
           .HasMaxLength(450);

        // 2. إعدادات Posts
        modelBuilder.Entity<Post>()
            .HasOne(p => p.User)
            .WithMany(u => u.Posts)
            .HasForeignKey(p => p.UserId);

        // 3. إعدادات PostReply (حل مشكلة الـ Cascade Paths)
        modelBuilder.Entity<PostReply>()
            .HasOne(pr => pr.Post)
            .WithMany(p => p.Replies)
            .HasForeignKey(pr => pr.PostId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<PostReply>()
            .HasOne(pr => pr.Pharmacy)
            .WithMany(p => p.Replies)
            .HasForeignKey(pr => pr.PharmacyId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<PostReply>()
            .HasOne(pr => pr.User)
            .WithMany() // لو مفيش Navigation property في User للردود
            .HasForeignKey(pr => pr.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        // 4. إعدادات PharmacyMessage
        modelBuilder.Entity<PharmacyMessage>()
            .HasOne(m => m.SenderPharmacy)
            .WithMany(p => p.SentMessages)
            .HasForeignKey(m => m.SenderPharmacyId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<PharmacyMessage>()
            .HasOne(m => m.ReceiverPharmacy)
            .WithMany(p => p.ReceivedMessages)
            .HasForeignKey(m => m.ReceiverPharmacyId)
            .OnDelete(DeleteBehavior.Restrict);

        // 5. إعدادات UserFavorite
        modelBuilder.Entity<UserFavorite>()
            .HasOne(uf => uf.User)
            .WithMany(u => u.Favorites)
            .HasForeignKey(uf => uf.UserId)
            .OnDelete(DeleteBehavior.Cascade); // لو اليوزر اتمسح مفضلاته تتمسح

        modelBuilder.Entity<UserFavorite>()
            .HasOne(uf => uf.Pharmacy)
            .WithMany()
            .HasForeignKey(uf => uf.PharmacyId)
            .OnDelete(DeleteBehavior.Restrict); // ممنوع مسح صيدلية وهي في مفضلة حد (أو خليها Cascade حسب الرغبة)

        // 6. إعدادات PrescriptionReply (حل مشكلة الـ Decimal Warning)
        modelBuilder.Entity<PrescriptionReply>()
            .Property(pr => pr.TotalPrice)
            .HasColumnType("decimal(18,2)"); // تحديد الدقة المالية

        // 6. ربط الروشتة بالردود (لو الروشتة اتمسحت، ردودها تتمسح)
        modelBuilder.Entity<PrescriptionReply>()
            .HasOne(pr => pr.Prescription)
            .WithMany(p => p.Replies)
            .HasForeignKey(pr => pr.PrescriptionId)
            .OnDelete(DeleteBehavior.Cascade);

        // 7. ربط الرد بالصيدلية (ممنوع مسح الصيدلية لو ليها ردود متعلقة عشان الـ History بتاع المريض)
        modelBuilder.Entity<PrescriptionReply>()
            .HasOne(pr => pr.Pharmacy)
            .WithMany() // ممكن تضيف ICollection<PrescriptionReply> في الـ Pharmacy لو حابب
            .HasForeignKey(pr => pr.PharmacyId)
            .OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<UserFavorite>()
            .HasKey(uf => new { uf.UserId, uf.PharmacyId });

        //8. إعدادات Notification
        modelBuilder.ApplyConfiguration(new NotificationConfiguration());
    }
}