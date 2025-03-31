using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Meritocious.Core.Features.Versioning;
using Meritocious.Core.Entities;

namespace Meritocious.Infrastructure.Data.Configurations
{
    public class ContentVersionConfiguration : IEntityTypeConfiguration<ContentVersion>
    {
        public void Configure(EntityTypeBuilder<ContentVersion> builder)
        {
            builder.HasKey(v => v.Id);

            builder.Property(v => v.ContentType)
                .IsRequired()
                .HasConversion<string>();

            builder.Property(v => v.VersionNumber)
                .IsRequired();

            builder.Property(v => v.Title)
                .HasMaxLength(255);

            builder.Property(v => v.Content)
                .IsRequired()
                .HasColumnType("ntext");

            builder.Property(v => v.EditReason)
                .HasMaxLength(500);

            builder.Property(v => v.MeritScore)
                .HasPrecision(5, 2);

            builder.Property(v => v.MeritScoreComponents)
                .HasColumnType("jsonb");

            builder.Property(v => v.ModeratorNotes)
                .HasMaxLength(1000);

            builder.Property(v => v.EditType)
                .IsRequired()
                .HasConversion<string>();

            builder.HasOne(v => v.Editor)
                .WithMany()
                .HasForeignKey(v => v.EditorId)
                .OnDelete(DeleteBehavior.Restrict);

            // Create indexes
            builder.HasIndex(v => new { v.ContentId, v.ContentType, v.VersionNumber });
            builder.HasIndex(v => v.CreatedAt);
            builder.HasIndex(v => v.EditorId);
        }
    }

    public class ContentDiffConfiguration : IEntityTypeConfiguration<ContentDiff>
    {
        public void Configure(EntityTypeBuilder<ContentDiff> builder)
        {
            builder.HasKey(d => d.Id);

            builder.Property(d => d.DiffData)
                .IsRequired()
                .HasColumnType("jsonb");

            builder.Property(d => d.TitleDiff)
                .HasColumnType("jsonb");

            builder.Property(d => d.MeritScoreDiff)
                .HasPrecision(5, 2);

            builder.Property(d => d.ComponentDiffs)
                .HasColumnType("jsonb");

            builder.HasOne(d => d.ContentVersion)
                .WithMany()
                .HasForeignKey(d => d.ContentVersionId)
                .OnDelete(DeleteBehavior.Cascade);

            // Create indexes
            builder.HasIndex(d => d.ContentVersionId);
            builder.HasIndex(d => d.CreatedAt);
        }
    }
}