﻿using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Meritocious.Core.Entities;
using Meritocious.Core.Features.Recommendations.Models;
using ContentSimilarity = Meritocious.Core.Entities.ContentSimilarity;

namespace Meritocious.Infrastructure.Data
{
    public class MeritociousDbContext : Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityDbContext<User>
    {
        // Original entities
        public override DbSet<User> Users { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<PostRelation> PostRelations { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<ExternalLogin> ExternalLogins { get; set; }
        public DbSet<BlockedIpAddress> BlockedIpAddresses { get; set; }
        public DbSet<UserSession> UserSessions { get; set; }
        public DbSet<MeritScoreType> MeritScoreTypes { get; set; }

        // Recommendations
        public DbSet<UserContentInteraction> UserContentInteractions { get; set; }
        public DbSet<ContentTopic> ContentTopics { get; set; }
        public DbSet<UserTopicPreference> UserTopicPreferences { get; set; }
        public DbSet<ContentSimilarity> ContentSimilarities { get; set; }
        public DbSet<TrendingContent> TrendingContents { get; set; }

        // Content versioning
        public DbSet<ContentVersion> ContentVersions { get; set; }

        // Moderation
        public DbSet<ModerationAction> ModerationActions { get; set; }
        public DbSet<ModerationActionEffect> ModerationEffects { get; set; }
        public DbSet<ModerationAppeal> ModerationAppeals { get; set; }
        public DbSet<ContentModerationEvent> ContentModerationEvents { get; set; }
        public DbSet<ContentReport> ContentReports { get; set; }

        // Notifications
        public DbSet<Notification> Notifications { get; set; }

        // Merit scoring
        public DbSet<MeritScoreHistory> MeritScoreHistory { get; set; }
        public DbSet<UserReputationMetrics> UserReputationMetrics { get; set; }
        public DbSet<ReputationSnapshot> ReputationSnapshots { get; set; }
        public DbSet<ReputationBadge> ReputationBadges { get; set; }

        // Tags
        public DbSet<Tag> Tags { get; set; }
        public DbSet<TagSynonym> TagSynonyms { get; set; }
        public DbSet<TagRelationship> TagRelationships { get; set; }
        public DbSet<TagWiki> TagWikis { get; set; }

        // Remixes
        public DbSet<QuoteLocation> QuoteLocations { get; set; }

        // Security
        public DbSet<SecurityEvent> SecurityEvents { get; set; }
        public DbSet<SecurityAuditLog> SecurityAuditLogs { get; set; }
        public DbSet<LoginAttempt> LoginAttempts { get; set; }
        public DbSet<ApiUsageLog> ApiUsageLogs { get; set; }
        public DbSet<AdminActionLog> AdminActionLogs { get; set; }

        // Substacks
        public DbSet<Substack> Substacks { get; set; }

        public MeritociousDbContext(DbContextOptions<MeritociousDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Apply all configurations from current assembly
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(MeritociousDbContext).Assembly);

            // Audit fields for all entities
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
                {
                    modelBuilder.Entity(entityType.ClrType)
                        .Property<DateTime>("CreatedAt")
                        .IsRequired();

                    modelBuilder.Entity(entityType.ClrType)
                        .Property<DateTime?>("UpdatedAt");
                }
            }
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            // Update audit fields
            foreach (var entry in ChangeTracker.Entries<BaseEntity>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedAt = DateTime.UtcNow;
                        break;
                    case EntityState.Modified:
                        entry.Entity.UpdatedAt = DateTime.UtcNow;
                        break;
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }
    }
}