using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Meritocious.Core.Features.Versioning;
using Meritocious.Core.Entities;
using System.Text.Json;
using Meritocious.Core.Extensions;
using System;

namespace Meritocious.Infrastructure.Data.Configurations
{
    public class ContentVersionConfiguration : IEntityTypeConfiguration<ContentVersion>
    {
        public void Configure(EntityTypeBuilder<ContentVersion> builder)
        {
            var (converter, comparer) = EfHelpers.For<Dictionary<string, decimal>>();

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

            builder.Property(e => e.MeritScoreComponents)
              .HasConversion(converter)
               .HasColumnType("nvarchar(max)")
               .Metadata.SetValueComparer(comparer);

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
            var (converter, comparer) = EfHelpers.For<Dictionary<string, decimal>>();
            var (converterString, comparerString) = EfHelpers.For<string>();

            builder.HasKey(d => d.Id);

            builder.Property(e => e.DiffData)
               .HasConversion(converterString)
               .HasColumnType("nvarchar(max)")
               .Metadata.SetValueComparer(comparerString);

            builder.Property(e => e.TitleDiff)
               .HasConversion(converterString)
               .HasColumnType("nvarchar(max)")
               .Metadata.SetValueComparer(comparerString);

            builder.Property(d => d.MeritScoreDiff)
                .HasPrecision(5, 2);

            builder.Property(e => e.ComponentDiffs)
               .HasConversion(converter)
               .HasColumnType("nvarchar(max)")
               .Metadata.SetValueComparer(comparer);

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