using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Elaaj.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Elaaj.infrastructure.Data.Configurations
{
    public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> builder)
        {
        builder.ToTable("Notifications");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.UserId)
                .IsRequired()
                .HasMaxLength(450);

            builder.Property(x => x.Title)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.Message)
                .IsRequired()
                .HasMaxLength(1000);

            builder.Property(x => x.Type)
                .HasConversion<int>()
                .IsRequired();

            builder.HasIndex(n => n.UserId);
            builder.HasIndex(n=> new { n.UserId, n.IsRead });
        }
    }
}
