using Elaaj.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elaaj.infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        public DbSet<Pharmacy> Pharmacies { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<PostReply> PostReplies { get; set; }
        public DbSet<PharmacyMessage> PharmacyMessages { get; set; }
        public DbSet<UserFavorite> UserFavorites { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //modelBuilder.Entity<Post>()
            //    .HasOne(p => p.Patient)
            //    .WithMany(p => p.posts)
            //    .HasForeignKey(p => p.PatientId);

            modelBuilder.Entity<PostReply>()
                .HasOne(r => r.Pharmacy)
                .WithMany(p => p.Replies)
                .HasForeignKey(r => r.PharmacyId)
                .OnDelete(DeleteBehavior.Restrict);

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
        }
    }
}
