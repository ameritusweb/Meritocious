using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Meritocious.Core.Entities;

namespace Meritocious.Infrastructure.Data.Configurations;

public class RemixConfiguration : IEntityTypeConfiguration<Remix>
{
    public void Configure(EntityTypeBuilder<Remix> builder)
    {
        builder.Property(r => r.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(r => r.Content)
            .IsRequired();

        builder.Property(r => r.MeritScore)
            .HasPrecision(4, 3);

        builder.Property(r => r.SynthesisMap)
            .HasColumnType("jsonb");

        builder.HasOne(r => r.Author)
            .WithMany()
            .HasForeignKey(r => r.AuthorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(r => r.Sources)
            .WithOne(s => s.Remix)
            .HasForeignKey(s => s.RemixId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(r => r.Notes)
            .WithOne(n => n.Remix)
            .HasForeignKey(n => n.RemixId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(r => r.Tags)
            .WithMany()
            .UsingEntity(j => j.ToTable("RemixTags"));

        builder.HasMany(r => r.Versions)
            .WithOne()
            .HasForeignKey("RemixId")
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class RemixSourceConfiguration : IEntityTypeConfiguration<RemixSource>
{
    public void Configure(EntityTypeBuilder<RemixSource> builder)
    {
        builder.Property(s => s.Relationship)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(s => s.Context)
            .HasMaxLength(500);

        builder.Property(s => s.QuotedExcerpts)
            .HasColumnType("jsonb");

        builder.Property(s => s.RelevanceScores)
            .HasColumnType("jsonb");

        builder.HasOne(s => s.SourcePost)
            .WithMany()
            .HasForeignKey(s => s.SourcePostId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class RemixNoteConfiguration : IEntityTypeConfiguration<RemixNote>
{
    public void Configure(EntityTypeBuilder<RemixNote> builder)
    {
        builder.Property(n => n.Type)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(n => n.Content)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(n => n.RelatedSourceIds)
            .HasColumnType("jsonb");

        builder.Property(n => n.Confidence)
            .HasPrecision(4, 3);
    }
}