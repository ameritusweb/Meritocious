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

        // Create indexes
        builder.HasIndex(e => e.EventType);
        builder.HasIndex(e => e.CreatedAt);
        builder.HasIndex(e => e.Severity);
        builder.HasIndex(e => e.IsResolved);
        builder.HasIndex(e => e.RequiresAction);
    }
}