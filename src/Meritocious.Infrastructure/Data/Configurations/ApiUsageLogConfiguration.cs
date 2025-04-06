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
    public class ApiUsageLogConfiguration : IEntityTypeConfiguration<ApiUsageLog>
    {
        public void Configure(EntityTypeBuilder<ApiUsageLog> builder)
        {
            var (converter, comparer) = EfHelpers.For<Dictionary<string, JsonElement>>();

            builder.HasKey(a => a.Id);

            builder.Property(a => a.Endpoint)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(a => a.Method)
                .IsRequired()
                .HasMaxLength(10);

            builder.Property(a => a.IpAddress)
                .IsRequired()
                .HasMaxLength(45);

            builder.Property(e => e.RequestMetadata)
               .HasConversion(converter)
               .HasColumnType("nvarchar(max)")
               .Metadata.SetValueComparer(comparer);

            builder.HasOne(a => a.User)
                .WithMany()
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.SetNull);

            // Create indexes
            builder.HasIndex(a => a.Endpoint);
            builder.HasIndex(a => a.StatusCode);
            builder.HasIndex(a => a.Timestamp);
            builder.HasIndex(a => a.UserId);
        }
    }
}
