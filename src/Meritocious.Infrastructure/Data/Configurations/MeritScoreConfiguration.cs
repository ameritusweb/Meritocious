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
    public class MeritScoreConfiguration : IEntityTypeConfiguration<MeritScore>
    {
        public void Configure(EntityTypeBuilder<MeritScore> builder)
        {
            builder.HasKey(s => s.Id);

            builder.Property(s => s.ContentId)
                .IsRequired();

            builder.Property(s => s.ContentType)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(s => s.Score)
                .HasPrecision(5, 2);

            // Navigation property
            builder.HasOne(s => s.ScoreType)
                .WithMany(t => t.Scores)
                .HasForeignKey(s => s.ScoreTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            // Create indexes for common queries
            builder.HasIndex(s => new { s.ContentId, s.ContentType });
            builder.HasIndex(s => s.ScoreTypeId);
            builder.HasIndex(s => s.Score);
        }
    }
}
