using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Meritocious.Core.Entities;

namespace Meritocious.Infrastructure.Data.Configurations;

public class SubstackConfiguration : IEntityTypeConfiguration<Substack>
{
    public void Configure(EntityTypeBuilder<Substack> builder)
    {
        builder.HasKey(s => s.Id);

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

        // Configure many-to-many relationship with User (followers)
        builder.HasMany(s => s.Followers)
            .WithMany(u => u.FollowedSubstacks)
            .UsingEntity(j => j.ToTable("SubstackFollowers"));

        // Configure one-to-many relationship with Post
        builder.HasMany(s => s.Posts)
            .WithOne(p => p.Substack)
            .HasForeignKey("SubstackId")
            .OnDelete(DeleteBehavior.Cascade);

        // Configure many-to-many relationship with ContentTopic
        builder.HasMany(s => s.Topics)
            .WithMany(t => t.Substacks)
            .UsingEntity(j => j.ToTable("SubstackTopics"));
    }
}