using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Meritocious.Core.Entities;
using Meritocious.Core.Extensions;
using Meritocious.Core.Features.Recommendations.Models;
using ContentSimilarity = Meritocious.Core.Entities.ContentSimilarity;
using Meritocious.Infrastructure.Data.Configurations;
using Meritocious.Infrastructure.Converters;
using System.Reflection;

namespace Meritocious.Infrastructure.Data
{
    public class MeritociousDbContext
        : Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityDbContext<User, Role, UlidId<User>, UserClaim, UserRole, UserLogin, RoleClaim, UserToken>
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

            modelBuilder.Entity<User>(b =>
            {
                b.Property(r => r.Id)
                    .HasConversion(new UlidIdValueConverter<User>())
                    .HasField("ulidId")
                    .HasMaxLength(26)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Role>(b =>
            {
                b.Property(r => r.Id)
                    .HasConversion(new UlidIdValueConverter<User>())
                    .HasField("ulidId")
                    .HasMaxLength(26)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<UserRole>(b =>
            {
                b.Property(ur => ur.UserId)
                    .HasConversion(new UlidIdValueConverter<User>())
                    .HasMaxLength(26)
                    .IsUnicode(false);

                b.Property(ur => ur.RoleId)
                    .HasConversion(new UlidIdValueConverter<User>())
                    .HasMaxLength(26)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<UserClaim>(b =>
            {
                b.Property(ur => ur.UserId)
                    .HasConversion(new UlidIdValueConverter<User>())
                    .HasMaxLength(26)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<UserLogin>(b =>
            {
                b.Property(ur => ur.UserId)
                    .HasConversion(new UlidIdValueConverter<User>())
                    .HasMaxLength(26)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<RoleClaim>(b =>
            {
                b.Property(ur => ur.RoleId)
                    .HasConversion(new UlidIdValueConverter<User>())
                    .HasMaxLength(26)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<UserToken>(b =>
            {
                b.Property(ur => ur.UserId)
                    .HasConversion(new UlidIdValueConverter<User>())
                    .HasMaxLength(26)
                    .IsUnicode(false);
            });

            // Apply all configurations from current assembly
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(MeritociousDbContext).Assembly);

            // Audit fields for all entities
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (typeof(IUlidEntity).IsAssignableFrom(entityType.ClrType))
                {
                    modelBuilder.Entity(entityType.ClrType)
                        .Property<DateTime>("CreatedAt")
                        .IsRequired();

                    modelBuilder.Entity(entityType.ClrType)
                        .Property<DateTime?>("UpdatedAt");
                }
            }

            modelBuilder.ApplyUlidIdConversions();
            modelBuilder.IgnoreReadOnlyProperties();
            modelBuilder.AddMissingSkipNavigations();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            foreach (var entry in ChangeTracker.Entries())
            {
                if (entry.State == EntityState.Added)
                {
                    // Handle entities implementing IUlidEntity with string Id
                    if (entry.Entity is IUlidEntity stringIdEntity)
                    {
                        if (string.IsNullOrWhiteSpace(stringIdEntity.Id))
                        {
                            stringIdEntity.Id = Ulid.NewUlid().ToString();
                        }
                    }
                    else
                    {
                        // Handle entities using UlidId<T> directly (e.g. IdentityUser)
                        var idProp = entry.Entity.GetType().GetProperty("Id", BindingFlags.Public | BindingFlags.Instance);
                        if (idProp != null &&
                            idProp.PropertyType.IsGenericType &&
                            idProp.PropertyType.GetGenericTypeDefinition() == typeof(UlidId<>))
                        {
                            var idValue = idProp.GetValue(entry.Entity);
                            var valueProp = idProp.PropertyType.GetProperty("Value");

                            string? value = valueProp?.GetValue(idValue) as string;

                            if (string.IsNullOrWhiteSpace(value))
                            {
                                var ulidNewMethod = idProp.PropertyType.GetMethod("New", BindingFlags.Static | BindingFlags.Public);
                                var newUlid = ulidNewMethod?.Invoke(null, null);
                                idProp.SetValue(entry.Entity, newUlid);
                            }
                        }
                    }
                }

                if (entry.State == EntityState.Modified)
                {
                    // Handle UpdatedAt logic for any entity with the property
                    var updatedProp = entry.Entity.GetType().GetProperty("UpdatedAt", BindingFlags.Public | BindingFlags.Instance);
                    if (updatedProp != null && updatedProp.CanWrite)
                    {
                        updatedProp.SetValue(entry.Entity, DateTime.UtcNow);
                    }
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }
    }
}