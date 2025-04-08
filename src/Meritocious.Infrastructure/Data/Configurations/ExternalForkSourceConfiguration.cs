using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Meritocious.Core.Entities;
using Meritocious.Infrastructure.Converters;
using System.Text.Json;
using Meritocious.Core.Extensions;

namespace Meritocious.Infrastructure.Data.Configurations;

public class ExternalForkSourceConfiguration : IEntityTypeConfiguration<ExternalForkSource>
{
    public void Configure(EntityTypeBuilder<ExternalForkSource> builder)
    {
        var (converter, comparer) = EfHelpers.For<Dictionary<string, JsonElement>>();
        var (converterList, comparerList) = EfHelpers.For<List<string>>();

        builder.ToTable("ExternalForkSources");

        // Required fields with constraints
        builder.Property(e => e.Type)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(e => e.Platform)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.Title)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(e => e.SourceUrl)
            .IsRequired()
            .HasMaxLength(2000);

        // Optional fields with constraints
        builder.Property(e => e.AuthorName)
            .HasMaxLength(200);

        builder.Property(e => e.Subtype)
            .HasMaxLength(50);

        // JSON storage for complex data
        builder.Property(e => e.LocationMetadata)
            .HasConversion(converter)
               .HasColumnType("nvarchar(max)")
               .Metadata.SetValueComparer(comparer);

        builder.Property(e => e.AdditionalMetadata)
            .HasConversion(converter)
               .HasColumnType("nvarchar(max)")
               .Metadata.SetValueComparer(comparer);

        builder.Property(e => e.Tags)
            .HasConversion(converterList)
               .HasColumnType("nvarchar(max)")
               .Metadata.SetValueComparer(comparerList);

        builder.HasMany(e => e.Forks)
            .WithOne(p => p.ExternalForkSource)
            .HasForeignKey("ExternalForkSourceId")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(e => e.Requests)
            .WithOne(r => r.ExternalForkSource)
            .HasForeignKey("ExternalForkSourceId")
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes for common queries
        builder.HasIndex(e => e.Type);
        builder.HasIndex(e => e.Platform);
        builder.HasIndex(e => e.Timestamp);
        builder.HasIndex(e => e.SourceUrl).IsUnique();
    }
}