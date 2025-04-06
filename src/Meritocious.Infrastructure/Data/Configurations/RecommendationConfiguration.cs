using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Meritocious.Core.Features.Recommendations.Models;
using Meritocious.Core.Entities;

namespace Meritocious.Infrastructure.Data.Configurations
{
    public class UserContentInteractionConfiguration : IEntityTypeConfiguration<UserContentInteraction>
    {
        public void Configure(EntityTypeBuilder<UserContentInteraction> builder)
        {
            builder.Property(i => i.ContentType)
                .IsRequired()
                .HasConversion<string>();

            builder.Property(i => i.InteractionType)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(i => i.EngagementScore)
                .HasPrecision(5, 2);

            builder.HasOne(i => i.User)
                .WithMany()
                .HasForeignKey(i => i.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Create indexes for common queries
            builder.HasIndex(i => new { i.UserId, i.InteractedAt });
            builder.HasIndex(i => new { i.ContentId, i.ContentType, i.InteractedAt });
            builder.HasIndex(i => i.InteractionType);
        }
    }

    public class ContentTopicConfiguration : IEntityTypeConfiguration<ContentTopic>
    {
        public void Configure(EntityTypeBuilder<ContentTopic> builder)
        {
            builder.HasKey(t => t.Id);

            builder.Property(t => t.ContentType)
                .IsRequired()
                .HasConversion<string>();

            builder.Property(t => t.Topic)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(t => t.Relevance)
                .HasPrecision(5, 2);

            // Create indexes for common queries
            builder.HasIndex(t => new { t.ContentId, t.ContentType });
            builder.HasIndex(t => t.Topic);
            builder.HasIndex(t => t.ExtractedAt);
        }
    }

    public class UserTopicPreferenceConfiguration : IEntityTypeConfiguration<UserTopicPreference>
    {
        public void Configure(EntityTypeBuilder<UserTopicPreference> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(p => p.Topic)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(p => p.Weight)
                .HasPrecision(5, 2);

            builder.HasOne(p => p.User)
                .WithMany()
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Create unique constraint for user-topic combinations
            builder.HasIndex(p => new { p.UserId, p.Topic }).IsUnique();
            builder.HasIndex(p => p.LastUpdated);
        }
    }

    public class ContentSimilarityConfiguration : IEntityTypeConfiguration<ContentSimilarity>
    {
        public void Configure(EntityTypeBuilder<ContentSimilarity> builder)
        {
            builder.HasKey(s => s.Id);

            builder.Property(s => s.SimilarityScore)
                .HasPrecision(5, 2);

            builder.HasOne(s => s.Content1)
                      .WithMany()
                      .HasForeignKey(s => s.Content1Id)
                      .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(s => s.Content2)
                     .WithMany()
                     .HasForeignKey(s => s.Content2Id)
                     .OnDelete(DeleteBehavior.NoAction);

            // Ensure consistent ordering of ContentId1 and ContentId2
            builder.HasCheckConstraint("CK_ContentSimilarity_IdOrder", "ContentId1 < ContentId2");

            // Create unique constraint and index for content pairs
            builder.HasIndex(s => new { s.ContentId1, s.ContentId2 }).IsUnique();
            builder.HasIndex(s => s.SimilarityScore);
            builder.HasIndex(s => s.LastUpdated);
        }
    }

    public class TrendingContentConfiguration : IEntityTypeConfiguration<TrendingContent>
    {
        public void Configure(EntityTypeBuilder<TrendingContent> builder)
        {
            builder.HasKey(t => t.Id);

            builder.Property(t => t.ContentType)
                .IsRequired()
                .HasConversion<string>();

            builder.Property(t => t.TrendingScore)
                .HasPrecision(5, 2);

            builder.Property(t => t.AverageMeritScore)
                .HasPrecision(5, 2);

            // Create indexes for common queries
            builder.HasIndex(t => new { t.ContentId, t.ContentType }).IsUnique();
            builder.HasIndex(t => t.TrendingScore);
            builder.HasIndex(t => new { t.WindowStart, t.WindowEnd });
        }
    }
}