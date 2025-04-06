using Meritocious.Core.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection.Emit;
using System.Text.Json;
using Meritocious.Core.Extensions;

namespace Meritocious.Infrastructure.Data.Configurations
{
    public class AdminActionLogConfiguration : IEntityTypeConfiguration<AdminActionLog>
    {
        public void Configure(EntityTypeBuilder<AdminActionLog> builder)
        {
            var (converter, comparer) = EfHelpers.For<Dictionary<string, JsonElement>>();

            builder.Property(a => a.Action)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(a => a.Category)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(a => a.Details)
                .IsRequired()
                .HasMaxLength(2000);

            builder.Property(a => a.IpAddress)
                .IsRequired()
            .HasMaxLength(45);

            builder.Property(e => e.Metadata)
               .HasConversion(converter)
               .HasColumnType("nvarchar(max)")
               .Metadata.SetValueComparer(comparer);

            builder.HasOne(a => a.AdminUser)
                .WithMany()
                .HasForeignKey(a => a.AdminUserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Create indexes
            builder.HasIndex(a => a.AdminUserId);
            builder.HasIndex(a => a.Timestamp);
            builder.HasIndex(a => a.Category);
            builder.HasIndex(a => a.Action);
        }
    }
}
