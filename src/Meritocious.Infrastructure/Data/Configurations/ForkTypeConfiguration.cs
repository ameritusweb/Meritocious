using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Meritocious.Core.Entities;
using Meritocious.Infrastructure.Converters;
using System.Text.Json;
using Meritocious.Core.Extensions;

namespace Meritocious.Infrastructure.Data.Configurations;

public class ForkTypeConfiguration : IEntityTypeConfiguration<ForkType>
{
    public void Configure(EntityTypeBuilder<ForkType> builder)
    {
        var (converterList, comparerList) = EfHelpers.For<List<string>>();

        builder.ToTable("ForkTypes");

        // Required fields
        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(e => e.DisplayName)
            .IsRequired()
            .HasMaxLength(100);

        // Optional fields
        builder.Property(e => e.Description)
            .HasMaxLength(500);

        // Subtypes as JSON array
        builder.Property(e => e.Subtypes)
            .HasConversion(converterList)
               .HasColumnType("nvarchar(max)")
               .Metadata.SetValueComparer(comparerList);

        // Relationships
        builder.HasMany(e => e.Posts)
            .WithOne(p => p.ForkType)
            .HasForeignKey("ForkTypeId")
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(e => e.Name).IsUnique();
        builder.HasIndex(e => e.IsActive);
    }
}