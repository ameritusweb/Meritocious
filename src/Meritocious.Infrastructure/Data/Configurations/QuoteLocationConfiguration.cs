using Meritocious.Core.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meritocious.Infrastructure.Data.Configurations
{
    public class QuoteLocationConfiguration : IEntityTypeConfiguration<QuoteLocation>
    {
        public void Configure(EntityTypeBuilder<QuoteLocation> builder)
        {
            builder.HasKey(q => q.Id);

            builder.Property(q => q.Content)
                .IsRequired()
                .HasColumnType("ntext");

            builder.Property(q => q.Context)
                .HasColumnType("ntext");

            builder.Property(q => q.StartPosition)
                .IsRequired();

            builder.Property(q => q.EndPosition)
                .IsRequired();

            builder.HasOne(q => q.PostSource)
                .WithMany()
                .HasForeignKey(q => q.PostSourceId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(q => q.PostRelation)
                .WithMany(r => r.Quotes)
                .HasForeignKey(q => new { q.PostRelationParentId, q.PostRelationChildId })
                .HasPrincipalKey(r => new { r.ParentId, r.ChildId })
                .OnDelete(DeleteBehavior.Cascade);

            // Create index on the relationship
            builder.HasIndex(new[] { "PostRelationParentId", "PostRelationChildId" });
        }
    }
}
