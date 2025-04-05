using Meritocious.Core.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meritocious.Infrastructure.Data.Configurations
{
    public class UserSessionConfiguration : IEntityTypeConfiguration<UserSession>
    {
        public void Configure(EntityTypeBuilder<UserSession> builder)
        {
            builder.HasKey(s => s.Id);

            builder.Property(s => s.SessionId)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(s => s.IpAddress)
                .IsRequired()
                .HasMaxLength(45);

            builder.Property(s => s.UserAgent)
                .IsRequired()
                .HasMaxLength(1000);

            builder.HasOne(s => s.User)
                .WithMany()
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Create indexes
            builder.HasIndex(s => s.SessionId).IsUnique();
            builder.HasIndex(s => s.UserId);
            builder.HasIndex(s => s.LastActivityAt);
            builder.HasIndex(s => s.ExpiresAt);
        }
    }
}
