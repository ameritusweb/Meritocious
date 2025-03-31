using Microsoft.EntityFrameworkCore;
using Meritocious.Core.Entities;
using Meritocious.Core.Features.Versioning;

namespace Meritocious.Infrastructure.Data
{
    public class MeritociousDbContext : DbContext
    {
        // Original entities
        public DbSet<User> Users { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<ExternalLogin> ExternalLogins { get; set; }

        // Content versioning
        public DbSet<ContentVersion> ContentVersions { get; set; }
        public DbSet<ContentDiff> ContentDiffs { get; set; }

        // Moderation
        public DbSet<ModerationAction> ModerationActions { get; set; }
        public DbSet<ModerationActionEffect> ModerationEffects { get; set; }
        public DbSet<ModerationAppeal> ModerationAppeals { get; set; }
        public DbSet<ContentModerationEvent> ContentModerationEvents { get; set; }
        public DbSet<ContentReport> ContentReports { get; set; }

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