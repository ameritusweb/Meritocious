using Meritocious.Core.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class ContentModerationEventConfiguration : IEntityTypeConfiguration<ContentModerationEvent>
{
    public void Configure(EntityTypeBuilder<ContentModerationEvent> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.ContentType)
            .HasConversion<string>();

        builder.Property(e => e.Action)
            .HasConversion<string>();

        builder.Property(e => e.Reason)
            .HasMaxLength(1000);

        builder.HasOne(e => e.Moderator)
            .WithMany()
            .HasForeignKey(e => e.ModeratorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(e => new { e.ContentId, e.ContentType });
        builder.HasIndex(e => e.ModeratedAt);
    }
}