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
    public class LoginAttemptConfiguration : IEntityTypeConfiguration<LoginAttempt>
    {
        public void Configure(EntityTypeBuilder<LoginAttempt> builder)
        {
            builder.HasKey(a => a.Id);

            builder.Property(a => a.Username)
                .IsRequired()
                .HasMaxLength(256);

            builder.Property(a => a.IpAddress)
                .IsRequired()
                .HasMaxLength(45);

            builder.Property(a => a.UserAgent)
                .HasMaxLength(1000);

            builder.Property(a => a.FailureReason)
                .HasMaxLength(500);

            builder.Property(a => a.AuthMethod)
                .HasMaxLength(50);

            builder.Property(a => a.Location)
                .HasMaxLength(100);

            builder.Property(a => a.Device)
                .HasMaxLength(100);

            // Create indexes
            builder.HasIndex(a => a.Username);
            builder.HasIndex(a => a.IpAddress);
            builder.HasIndex(a => a.Timestamp);
            builder.HasIndex(a => a.Success);
            builder.HasIndex(a => a.IsSuspicious);
        }
    }
}
