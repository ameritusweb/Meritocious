using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Meritocious.Core.Features.Moderation.Models;
using Meritocious.Common.Enums;
using Meritocious.Core.Entities;

namespace Meritocious.Infrastructure.Data.Configurations
{
    public class ModerationActionConfiguration : IEntityTypeConfiguration<ModerationAction>
    {
        public void Configure(EntityTypeBuilder<ModerationAction> builder)
        {
            builder.HasKey(a => a.Id);

            builder.Property(a => a.ContentType)
                .IsRequired()
                .HasConversion<string>();

            builder.Property(a => a.ActionType)
                .IsRequired()
                .HasConversion<string>();

            builder.Property(a => a.Reason)
                .IsRequired()
                .HasMaxLength(1000);

            builder.Property(a => a.ToxicityScores)
                .HasColumnType("jsonb");

            builder.Property(a => a.AutomatedAnalysis)
                .HasColumnType("jsonb");

            builder.Property(a => a.ModeratorNotes)
                .HasMaxLength(2000);

            builder.Property(a => a.Outcome)
                .HasConversion<string>();

            builder.Property(a => a.Severity)
                .IsRequired()
                .HasConversion<string>();

            builder.Property(a => a.ReviewNotes)
                .HasMaxLength(2000);

            builder.HasOne(a => a.Moderator)
                .WithMany()
                .HasForeignKey(a => a.ModeratorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(a => a.ReviewedBy)
                .WithMany()
                .HasForeignKey(a => a.ReviewedById)
                .OnDelete(DeleteBehavior.Restrict);

            // Create indexes
            builder.HasIndex(a => new { a.ContentId, a.ContentType });
            builder.HasIndex(a => a.ModeratorId);
            builder.HasIndex(a => a.CreatedAt);
            builder.HasIndex(a => a.Severity);
            builder.HasIndex(a => a.Outcome);
        }
    }

    public class ModerationActionEffectConfiguration : IEntityTypeConfiguration<ModerationActionEffect>
    {
        public void Configure(EntityTypeBuilder<ModerationActionEffect> builder)
        {
            builder.HasKey(e => e.Id);

            builder.Property(e => e.EffectType)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(e => e.EffectData)
                .HasColumnType("jsonb");

            builder.Property(e => e.RevertReason)
                .HasMaxLength(500);

            builder.HasOne(e => e.ModerationAction)
                .WithMany(a => a.Effects)
                .HasForeignKey(e => e.ModerationActionId)
                .OnDelete(DeleteBehavior.Cascade);

            // Create indexes
            builder.HasIndex(e => e.ModerationActionId);
            builder.HasIndex(e => e.EffectType);
            builder.HasIndex(e => e.ExpiresAt);
            builder.HasIndex(e => e.IsReverted);
        }
    }

    public class ModerationAppealConfiguration : IEntityTypeConfiguration<ModerationAppeal>
    {
        public void Configure(EntityTypeBuilder<ModerationAppeal> builder)
        {
            builder.HasKey(a => a.Id);

            builder.Property(a => a.Reason)
                .IsRequired()
                .HasMaxLength(2000);

            builder.Property(a => a.AdditionalContext)
                .HasMaxLength(5000);

            builder.Property(a => a.Status)
                .IsRequired()
                .HasConversion<string>();

            builder.Property(a => a.ReviewerNotes)
                .HasMaxLength(2000);

            builder.Property(a => a.Decision)
                .HasConversion<string>();

            builder.HasOne(a => a.ModerationAction)
                .WithMany()
                .HasForeignKey(a => a.ModerationActionId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(a => a.Appealer)
                .WithMany()
                .HasForeignKey(a => a.AppealerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(a => a.Reviewer)
                .WithMany()
                .HasForeignKey(a => a.ReviewerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Create indexes
            builder.HasIndex(a => a.ModerationActionId);
            builder.HasIndex(a => a.AppealerId);
            builder.HasIndex(a => a.Status);
            builder.HasIndex(a => a.CreatedAt);
        }
    }
}