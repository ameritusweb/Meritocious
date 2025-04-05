

namespace Meritocious.Infrastructure.Data.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using Meritocious.Core.Entities;

    public class PostConfiguration : IEntityTypeConfiguration<Post>
    {
        public void Configure(EntityTypeBuilder<Post> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(p => p.Title)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(p => p.Content)
                .IsRequired()
                .HasColumnType("ntext");

            // Base metrics
            builder.Property(p => p.MeritScore)
                .HasPrecision(5, 2)
                .HasDefaultValue(0.00m);

            builder.Property(p => p.ViewCount)
                .HasDefaultValue(0);

            builder.Property(p => p.UniqueViewCount)
                .HasDefaultValue(0);

            builder.Property(p => p.LikeCount)
                .HasDefaultValue(0);

            builder.Property(p => p.ShareCount)
                .HasDefaultValue(0);

            builder.Property(p => p.AverageTimeSpentSeconds)
                .HasPrecision(10, 2)
                .HasDefaultValue(0.00m);

            // Merit components as JSON
            builder.Property(p => p.MeritComponents)
                .HasColumnType("jsonb");

            // Relationships
            builder.HasMany(p => p.ParentRelations)
                .WithOne(r => r.Child)
                .HasForeignKey(r => r.ChildId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(p => p.ChildRelations)
                .WithOne(r => r.Parent)
                .HasForeignKey(r => r.ParentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(p => p.MeritScores)
            .WithOne()
            .HasForeignKey("ContentId")
            .HasPrincipalKey(p => p.Id)
            .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(p => p.MeritScoreHistories)
                .WithOne()
                .HasForeignKey("ContentId")
                .HasPrincipalKey(p => p.Id)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes for common queries
            builder.HasIndex(p => p.AuthorId);
            builder.HasIndex(p => p.IsDeleted);
            builder.HasIndex(p => p.CreatedAt);
            
            // Composite indexes for filtered queries
            builder.HasIndex(p => new { p.AuthorId, p.IsDeleted });
            builder.HasIndex(p => new { p.IsDeleted, p.MeritScore });
            builder.HasIndex(p => new { p.IsDeleted, p.CreatedAt });
            
            // TODO: 
            // builder.HasIndex(p => p.Title)
            //    .HasMethod("GIN") // PostgreSQL GIN index for text search
            //    .IsTsVectorExpressionIndex("english");
            // Metrics indexes for analytics
            builder.HasIndex(p => new { p.IsDeleted, p.ViewCount });
            builder.HasIndex(p => new { p.IsDeleted, p.LikeCount });
            builder.HasIndex(p => new { p.IsDeleted, p.ShareCount });
            
            // Engagement search
            builder.HasIndex(p => new { p.IsDeleted, p.MeritScore, p.ViewCount });
            
            // Merit components as JSONB for efficient component queries
            builder.Property(p => p.MeritComponents)
                .HasColumnType("jsonb")
                .HasDefaultValueSql("'{}'::jsonb");

            // TODO: 
            // builder.HasIndex(p => p.MeritComponents)
            //    .HasMethod("GIN"); // PostgreSQL GIN index for JSONB
            builder.HasMany(p => p.Comments)
                .WithOne(c => c.Post)
                .HasForeignKey(c => c.PostId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(p => p.Tags)
                .WithMany(t => t.Posts)
                .UsingEntity(j => j.ToTable("PostTags"));
        }
    }
}