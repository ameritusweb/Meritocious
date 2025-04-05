using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Meritocious.Core.Entities;

namespace Meritocious.Infrastructure.Data.Configurations;

public class PostRelationConfiguration : IEntityTypeConfiguration<PostRelation>
{
    public void Configure(EntityTypeBuilder<PostRelation> builder)
    {
        builder.ToTable("PostRelations");

        // Keys and properties
        builder.HasKey(r => new { r.ParentId, r.ChildId });

        builder.Property(r => r.RelationType)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(r => r.Role)
            .HasMaxLength(50);

        builder.Property(r => r.Context)
            .HasMaxLength(1000);

        builder.Property(r => r.RelevanceScore)
            .HasPrecision(5, 2)
            .HasDefaultValue(0.00m);

        builder.Property(r => r.OrderIndex)
            .HasDefaultValue(0);

        // Quotes collection as JSON
        builder.Property(r => r.Quotes)
            .HasColumnType("jsonb");

        // Relationships are configured in PostConfiguration to avoid cycles

        // Core relationship indexes
        builder.HasIndex(r => new { r.ParentId, r.RelationType })
            .IncludeProperties(r => new { r.Role, r.OrderIndex });
            
        builder.HasIndex(r => new { r.ChildId, r.RelationType })
            .IncludeProperties(r => new { r.Role, r.OrderIndex });

        // Fork graph traversal
        builder.HasIndex(r => new { r.ParentId, r.ChildId, r.RelationType })
            .IsUnique();

        // Remix-specific indexes
        builder.HasIndex(r => new { r.RelationType, r.Role, r.OrderIndex })
            .HasFilter("RelationType = 'remix'")
            .IncludeProperties(r => new { r.ParentId, r.ChildId });

        builder.HasIndex(r => new { r.ChildId, r.RelationType, r.OrderIndex })
            .HasFilter("RelationType = 'remix'")
            .IncludeProperties(r => new { r.Role, r.RelevanceScore });

        // Source ordering for remixes
        builder.HasIndex(r => new { r.ChildId, r.OrderIndex })
            .HasFilter("RelationType = 'remix'")
            .IncludeProperties(r => new { r.ParentId, r.Role });

        // Analytics and time-based queries
        builder.HasIndex(r => new { r.RelationType, r.CreatedAt })
            .IncludeProperties(r => new { r.ParentId, r.ChildId });

        builder.HasIndex(r => new { r.RelationType, r.RelevanceScore })
            .HasFilter("RelationType = 'remix'")
            .IncludeProperties(r => new { r.Role });

        // Quote search
        builder.Property(r => r.Quotes)
            .HasColumnType("jsonb")
            .HasDefaultValueSql("'[]'::jsonb");

        // TODO: Figure this out.
        // builder.HasIndex(r => r.Quotes)
        //    .HasMethod("GIN")
        //    .HasFilter("RelationType = 'remix'");
    }
}