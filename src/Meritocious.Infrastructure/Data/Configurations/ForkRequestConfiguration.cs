using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Meritocious.Core.Entities;
using Meritocious.Infrastructure.Converters;

namespace Meritocious.Infrastructure.Data.Configurations;

public class ForkRequestConfiguration : IEntityTypeConfiguration<ForkRequest>
{
    public void Configure(EntityTypeBuilder<ForkRequest> builder)
    {
        builder.ToTable("ForkRequests");

        // Required fields
        builder.Property(e => e.SuggestedFocus)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(e => e.Status)
            .IsRequired()
            .HasMaxLength(20);

        // Optional fields
        builder.Property(e => e.Notes)
            .HasMaxLength(2000);

        // Relationships
        builder.HasOne(d => d.Submitter)
            .WithMany()
            .HasForeignKey("SubmitterId")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(d => d.ExternalForkSource)
            .WithMany(e => e.Requests)
            .HasForeignKey("ExternalForkSourceId")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(d => d.ClaimedBy)
            .WithMany()
            .HasForeignKey("ClaimedById")
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(d => d.FulfilledByPost)
            .WithMany()
            .HasForeignKey("FulfilledByPostId")
            .OnDelete(DeleteBehavior.SetNull);

        // Indexes
        builder.HasIndex(e => e.Status);
        builder.HasIndex(e => e.ClaimedAt);
        builder.HasIndex(e => new { e.Status, e.ExternalForkSourceId });
        builder.HasIndex(e => new { e.Status, e.ClaimedById });
    }
}