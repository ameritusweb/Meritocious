using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Meritocious.Core.Features.Merit.Models;
using Meritocious.Core.Entities;

namespace Meritocious.Infrastructure.Data.Configurations
{
    public class MeritScoreHistoryConfiguration : IEntityTypeConfiguration<MeritScoreHistory>
    {
        public void Configure(EntityTypeBuilder<MeritScoreHistory> builder)
        {
            builder.HasKey(h => h.Id);

            builder.Property(h => h.ContentType)
                .IsRequired()
                .HasConversion<string>();

            builder.Property(h => h.Score)
                .HasPrecision(5, 2);

            builder.Property(h => h.Components)
                .HasColumnType("jsonb");

            builder.Property(h => h.ModelVersion)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(h => h.Explanations)
                .HasColumnType("jsonb");

            builder.Property(h => h.Context)
                .HasColumnType("ntext");

            builder.Property(h => h.RecalculationReason)
                .HasMaxLength(500);

            // Create indexes
            builder.HasIndex(h => new { h.ContentId, h.ContentType });
            builder.HasIndex(h => h.EvaluatedAt);
            builder.HasIndex(h => h.ModelVersion);
        }
    }

    public class UserReputationMetricsConfiguration : IEntityTypeConfiguration<UserReputationMetrics>
    {
        public void Configure(EntityTypeBuilder<UserReputationMetrics> builder)
        {
            builder.HasKey(m => m.Id);

            builder.Property(m => m.OverallMeritScore)
                .HasPrecision(5, 2);

            builder.Property(m => m.CategoryScores)
                .HasColumnType("jsonb");

            builder.Property(m => m.ContributionCounts)
                .HasColumnType("jsonb");

            builder.Property(m => m.TopicExpertise)
                .HasColumnType("jsonb");

            builder.Property(m => m.Level)
                .IsRequired()
                .HasConversion<string>();

            builder.Property(m => m.ContentQualityAverage)
                .HasPrecision(5, 2);

            builder.Property(m => m.CommunityImpact)
                .HasPrecision(5, 2);

            builder.Property(m => m.BadgeProgress)
                .HasColumnType("jsonb");

            builder.HasOne(m => m.User)
                .WithMany()
                .HasForeignKey(m => m.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Create indexes
            builder.HasIndex(m => m.UserId).IsUnique();
            builder.HasIndex(m => m.OverallMeritScore);
            builder.HasIndex(m => m.Level);
            builder.HasIndex(m => m.TotalContributions);
        }
    }

    public class ReputationSnapshotConfiguration : IEntityTypeConfiguration<ReputationSnapshot>
    {
        public void Configure(EntityTypeBuilder<ReputationSnapshot> builder)
        {
            builder.HasKey(s => s.Id);

            builder.Property(s => s.OverallMeritScore)
                .HasPrecision(5, 2);

            builder.Property(s => s.MetricSnapshots)
                .HasColumnType("jsonb");

            builder.Property(s => s.Level)
                .IsRequired()
                .HasConversion<string>();

            builder.Property(s => s.TimeFrame)
                .IsRequired()
                .HasMaxLength(20);

            builder.HasOne(s => s.User)
                .WithMany()
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Create indexes
            builder.HasIndex(s => new { s.UserId, s.TimeFrame, s.StartDate });
            builder.HasIndex(s => s.EndDate);
            builder.HasIndex(s => s.Level);
        }
    }

    public class ReputationBadgeConfiguration : IEntityTypeConfiguration<ReputationBadge>
    {
        public void Configure(EntityTypeBuilder<ReputationBadge> builder)
        {
            builder.HasKey(b => b.Id);

            builder.Property(b => b.BadgeType)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(b => b.Category)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(b => b.Criteria)
                .HasColumnType("jsonb");

            builder.Property(b => b.Progress)
                .HasColumnType("jsonb");

            builder.Property(b => b.AwardReason)
                .HasMaxLength(500);

            builder.HasOne(b => b.User)
                .WithMany()
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Create indexes
            builder.HasIndex(b => new { b.UserId, b.BadgeType, b.Level }).IsUnique();
            builder.HasIndex(b => b.Category);
            builder.HasIndex(b => b.AwardedAt);
        }
    }
}