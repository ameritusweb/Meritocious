using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Meritocious.Core.Features.Tags.Models;
using Meritocious.Core.Entities;
using System.Text.Json;
using Meritocious.Core.Extensions;

namespace Meritocious.Infrastructure.Data.Configurations
{
    public class TagConfiguration : IEntityTypeConfiguration<Tag>
    {
        public void Configure(EntityTypeBuilder<Tag> builder)
        {
            var (converter, comparer) = EfHelpers.For<Dictionary<string, string>>();

            builder.HasKey(t => t.Id);

            builder.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(t => t.Slug)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(t => t.Description)
                .HasMaxLength(1000);

            builder.Property(t => t.Category)
                .IsRequired()
                .HasConversion<string>();

            builder.Property(t => t.MeritThreshold)
                .HasPrecision(5, 2);

            builder.Property(t => t.Status)
                .IsRequired()
                .HasConversion<string>();

            builder.Property(e => e.Metadata)
               .HasConversion(converter)
               .HasColumnType("nvarchar(max)")
               .Metadata.SetValueComparer(comparer);

            builder.HasOne(t => t.ParentTag)
                .WithMany(t => t.ChildTags)
                .HasForeignKey(t => t.ParentTagId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
               .HasMany<Post>(u => u.Posts)
               .WithMany(s => s.Tags);

               // .UsingEntity<PostTag>(
               //    "PostTags",
               //    j => j.HasOne<Post>().WithMany().HasForeignKey("PostId"),
               //    j => j.HasOne<Tag>().WithMany().HasForeignKey("TagId"),
               //    j => j.HasKey("PostId", "TagId"));

            // Create indexes
            builder.HasIndex(t => t.Name).IsUnique();
            builder.HasIndex(t => t.Slug).IsUnique();
            builder.HasIndex(t => t.Category);
            builder.HasIndex(t => t.Status);
            builder.HasIndex(t => t.UseCount);
        }
    }

    public class TagSynonymConfiguration : IEntityTypeConfiguration<TagSynonym>
    {
        public void Configure(EntityTypeBuilder<TagSynonym> builder)
        {
            builder.HasKey(s => s.Id);

            builder.Property(s => s.Name)
               .IsRequired()
               .HasMaxLength(100);

            // builder.Property(s => s.Slug)
            //   .IsRequired()
            //   .HasMaxLength(100);
            builder.HasOne(s => s.SourceTag)
               .WithMany(t => t.Synonyms)
               .HasForeignKey(s => s.SourceTagId)
               .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(s => s.CreatedBy)
               .WithMany()
               .HasForeignKey(s => s.CreatedById)
               .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(s => s.ApprovedBy)
                .WithMany()
                .HasForeignKey(s => s.ApprovedById)
                .OnDelete(DeleteBehavior.Restrict);

            // TODO: Create indexes
            // builder.HasIndex(s => new { s.TagId, s.Name }).IsUnique();
            // builder.HasIndex(s => s.Slug);
            // builder.HasIndex(s => s.UseCount);
            builder.HasIndex(s => s.IsApproved);
        }
    }

    public class TagRelationshipConfiguration : IEntityTypeConfiguration<TagRelationship>
    {
        public void Configure(EntityTypeBuilder<TagRelationship> builder)
        {
            builder.HasKey(r => r.Id);

            builder.Property(r => r.RelationType)
                .IsRequired()
                .HasConversion<string>();

            builder.Property(r => r.Strength)
                .HasPrecision(5, 2);

            builder.HasOne(r => r.SourceTag)
                .WithMany(t => t.RelatedTags)
                .HasForeignKey(r => r.SourceTagId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(r => r.RelatedTag)
                .WithMany()
                .HasForeignKey(r => r.RelatedTagId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(r => r.Creator)
                .WithMany()
                .HasForeignKey(r => r.CreatorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(r => r.ApprovedBy)
                .WithMany()
                .HasForeignKey(r => r.ApprovedById)
                .OnDelete(DeleteBehavior.Restrict);

            // Create indexes
            builder.HasIndex(r => new { r.SourceTagId, r.RelatedTagId, r.RelationType }).IsUnique();
            builder.HasIndex(r => r.Strength);
            builder.HasIndex(r => r.IsApproved);
        }
    }

    public class TagWikiConfiguration : IEntityTypeConfiguration<TagWiki>
    {
        public void Configure(EntityTypeBuilder<TagWiki> builder)
        {
            builder.HasKey(w => w.Id);

            builder.Property(w => w.Content)
                .IsRequired()
                .HasColumnType("ntext");

            builder.Property(w => w.EditReason)
                .HasMaxLength(500);

            builder.HasOne(w => w.Tag)
                .WithMany(t => t.WikiVersions)
                .HasForeignKey(w => w.TagId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(w => w.Editor)
                .WithMany()
                .HasForeignKey(w => w.EditorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(w => w.ApprovedBy)
                .WithMany()
                .HasForeignKey(w => w.ApprovedById)
                .OnDelete(DeleteBehavior.Restrict);

            // Create indexes
            builder.HasIndex(w => new { w.TagId, w.VersionNumber }).IsUnique();
            builder.HasIndex(w => w.IsApproved);
        }
    }
}