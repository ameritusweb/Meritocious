using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Meritocious.Core.Entities;

namespace Meritocious.Infrastructure.Data.Configurations;

public class SubstackConfiguration : IEntityTypeConfiguration<Substack>
{
    public void Configure(EntityTypeBuilder<Substack> builder)
    {
        builder.Property(s => s.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(s => s.Subdomain)
            .IsRequired()
            .HasMaxLength(200);
        
        builder.HasIndex(s => s.Subdomain)
            .IsUnique();

        builder.Property(s => s.CustomDomain)
            .HasMaxLength(200);

        builder.Property(s => s.AuthorName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(s => s.Description)
            .HasMaxLength(2000);

        builder.Property(s => s.LogoUrl)
            .HasMaxLength(500);

        builder.Property(s => s.CoverImageUrl)
            .HasMaxLength(500);

        builder.Property(s => s.TwitterHandle)
            .HasMaxLength(50);

        // Configure one-to-many relationship with Post
        builder.HasMany(s => s.Posts)
            .WithOne(p => p.Substack)
            .HasForeignKey("SubstackId")
            .OnDelete(DeleteBehavior.Cascade);

        // Configure many-to-many relationship with ContentTopic
        builder.HasMany(s => s.Topics)
            .WithMany(t => t.Substacks)
            .UsingEntity(j => j.ToTable("SubstackTopics"));

        // Create indexes for commonly queried fields
        builder.HasIndex(s => s.CustomDomain);  // Used for lookup by custom domain
        builder.HasIndex(s => s.AuthorName);  // Used for searching by author
        builder.HasIndex(s => s.CreatedAt);  // Used for sorting by date
        builder.HasIndex(s => s.LastPostDate);  // Used for activity-based queries
        builder.HasIndex(s => s.EngagementRate);  // Used for trending queries
        builder.HasIndex(s => s.AvgMeritScore);  // Used for quality-based sorting
        builder.HasIndex(s => s.IsVerified);  // Used for filtering verified substacks
        
        // Composite indexes for common query patterns
        builder.HasIndex(s => new { s.IsVerified, s.EngagementRate });  // For trending verified substacks
        builder.HasIndex(s => new { s.IsVerified, s.AvgMeritScore });  // For top-rated verified substacks
        builder.HasIndex(s => new { s.LastPostDate, s.EngagementRate });  // For recently active trending substacks
    }
}