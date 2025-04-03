using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Meritocious.Core.Entities;

namespace Meritocious.Infrastructure.Data.Configurations;

public class SecurityEventConfiguration : IEntityTypeConfiguration<SecurityEvent>
{
    public void Configure(EntityTypeBuilder<SecurityEvent> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.EventType)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(e => e.IpAddress)
            .IsRequired()
            .HasMaxLength(45);  // IPv6 addresses can be up to 45 characters

        builder.Property(e => e.UserAgent)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(e => e.Description)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(e => e.ResolutionNotes)
            .HasMaxLength(2000);

        // Configure relationships
        builder.HasOne(e => e.User)
            .WithMany()
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.SetNull);

        // Create indexes for commonly queried fields
        builder.HasIndex(e => e.EventType);  // Used for filtering by event type
        builder.HasIndex(e => e.CreatedAt);  // Used for sorting by date
        builder.HasIndex(e => e.Severity);  // Used for filtering by severity
        builder.HasIndex(e => e.IsResolved);  // Used for filtering resolved/unresolved events
        builder.HasIndex(e => e.RequiresAction);  // Used for filtering actionable events
        builder.HasIndex(e => e.UserId);  // Used for filtering by user
        builder.HasIndex(e => e.IpAddress);  // Used for IP-based security analysis
        
        // Composite indexes for common query patterns
        builder.HasIndex(e => new { e.IsResolved, e.RequiresAction });  // For finding unresolved actionable events
        builder.HasIndex(e => new { e.Severity, e.IsResolved });  // For finding unresolved high-severity events
        builder.HasIndex(e => new { e.UserId, e.EventType });  // For analyzing user's security events
        builder.HasIndex(e => new { e.IpAddress, e.EventType });  // For analyzing IP-based security patterns
    }
}