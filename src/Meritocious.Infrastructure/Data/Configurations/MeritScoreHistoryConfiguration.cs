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
    public class MeritScoreHistoryConfiguration : IEntityTypeConfiguration<MeritScoreHistory>
    {
        public void Configure(EntityTypeBuilder<MeritScoreHistory> builder)
        {
            builder.HasKey(h => h.Id);

            builder.Property(h => h.ContentType)
                .HasConversion<string>();

            builder.Property(h => h.Score)
                .HasPrecision(5, 2);

            builder.Property(h => h.ModelVersion)
                .HasMaxLength(50);

            builder.Property(h => h.ComponentScores)
                .HasColumnType("jsonb");

            builder.HasIndex(h => new { h.ContentId, h.ContentType });
            builder.HasIndex(h => h.Timestamp);
        }
    }
}
