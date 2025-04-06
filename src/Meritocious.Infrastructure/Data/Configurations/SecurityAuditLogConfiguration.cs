using Meritocious.Core.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using Meritocious.Core.Extensions;

namespace Meritocious.Infrastructure.Data.Configurations
{
    public class SecurityAuditLogConfiguration : IEntityTypeConfiguration<SecurityAuditLog>
    {
        public void Configure(EntityTypeBuilder<SecurityAuditLog> builder)
        {
            var (converter, comparer) = EfHelpers.For<Dictionary<string, JsonElement>>();

            builder.HasKey(s => s.Id);

            builder.Property(s => s.EventType)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(s => s.Severity)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(s => s.Description)
                .IsRequired()
                .HasMaxLength(2000);

            builder.Property(s => s.IpAddress)
                .IsRequired()
                .HasMaxLength(45);

            builder.Property(s => s.UserAgent)
                .HasMaxLength(1000);

            builder.Property(e => e.Context)
               .HasConversion(converter)
               .HasColumnType("nvarchar(max)")
               .Metadata.SetValueComparer(comparer);

            builder.Property(s => s.Level)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(s => s.Message)
                .IsRequired()
                .HasMaxLength(2000);

            builder.Property(s => s.Source)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(s => s.Exception)
                .HasColumnType("ntext");

            builder.Property(s => s.AdditionalData)
                .HasColumnType("ntext");

            builder.HasOne(s => s.User)
                .WithMany()
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.SetNull);

            // Create indexes
            builder.HasIndex(s => s.EventType);
            builder.HasIndex(s => s.Timestamp);
            builder.HasIndex(s => s.Severity);
            builder.HasIndex(s => s.UserId);
            builder.HasIndex(s => s.IpAddress);
            builder.HasIndex(s => s.Level);
        }
    }
}
