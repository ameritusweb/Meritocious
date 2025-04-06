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
    public class BlockedIpAddressConfiguration : IEntityTypeConfiguration<BlockedIpAddress>
    {
        public void Configure(EntityTypeBuilder<BlockedIpAddress> builder)
        {
            builder.Property(b => b.IpAddress)
                .IsRequired()
                .HasMaxLength(45);  // IPv6 addresses can be up to 45 characters

            builder.Property(b => b.Reason)
                .IsRequired()
                .HasMaxLength(1000);

            builder.Property(b => b.BlockedAt)
                .IsRequired();

            builder.HasOne(b => b.BlockedByUser)
                .WithMany()
                .HasForeignKey(b => b.BlockedByUserId)
                .OnDelete(DeleteBehavior.SetNull);

            // Create indexes
            builder.HasIndex(b => b.IpAddress).IsUnique();
            builder.HasIndex(b => b.BlockedAt);
            builder.HasIndex(b => b.ExpiresAt);
        }
    }
}
