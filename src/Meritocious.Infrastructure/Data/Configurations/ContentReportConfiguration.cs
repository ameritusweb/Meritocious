using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Meritocious.Core.Features.Reporting.Models;
using System;

namespace Meritocious.Infrastructure.Data.Configurations
{
    public class ContentReportConfiguration : IEntityTypeConfiguration<ContentReport>
    {
        public void Configure(EntityTypeBuilder<ContentReport> builder)
        {
            builder.HasKey(r => r.Id);

            builder.Property(r => r.ContentType)
                .IsRequired()
                .HasConversion<string>();

            builder.Property(r => r.ReportType)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(r => r.Description)
                .IsRequired()
                .HasMaxLength(1000);

            builder.Property(r => r.Status)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(r => r.Resolution)
                .HasMaxLength(255);

            builder.Property(r => r.Notes)
                .HasMaxLength(1000);

            builder.HasIndex(r => new { r.ContentId, r.ContentType });
            builder.HasIndex(r => r.Status);
            builder.HasIndex(r => r.CreatedAt);
            builder.HasIndex(r => r.ResolvedAt);
        }
    }
}