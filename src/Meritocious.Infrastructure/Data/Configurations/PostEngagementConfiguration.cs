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
    public class PostEngagementConfiguration : IEntityTypeConfiguration<PostEngagement>
    {
        public void Configure(EntityTypeBuilder<PostEngagement> builder)
        {
            builder.HasKey(e => e.Id);

            builder.Property(e => e.Views)
                .HasDefaultValue(0);

            builder.Property(e => e.UniqueViews)
                .HasDefaultValue(0);

            builder.Property(e => e.Likes)
                .HasDefaultValue(0);

            builder.Property(e => e.Comments)
                .HasDefaultValue(0);

            builder.Property(e => e.Forks)
                .HasDefaultValue(0);

            builder.Property(e => e.Shares)
                .HasDefaultValue(0);

            builder.Property(e => e.AverageTimeSpentSeconds)
                .HasPrecision(10, 2)
                .HasDefaultValue(0);

            builder.Property(e => e.BounceRate)
                .HasPrecision(5, 2)
                .HasDefaultValue(0);

            // Dictionary properties as JSON
            builder.Property(e => e.ViewsByRegion)
                .HasColumnType("jsonb")
                .HasDefaultValueSql("'{}'::jsonb");

            builder.Property(e => e.ViewsByPlatform)
                .HasColumnType("jsonb")
                .HasDefaultValueSql("'{}'::jsonb");

            builder.Property(e => e.ViewTrend)
                .HasColumnType("jsonb")
                .HasDefaultValueSql("'{}'::jsonb");

            builder.Property(e => e.SourceInfluenceScores)
                .HasColumnType("jsonb")
                .HasDefaultValueSql("'{}'::jsonb");

            builder.Property(e => e.TopEngagementSources)
                .HasColumnType("jsonb")
                .HasDefaultValueSql("'[]'::jsonb");

            builder.HasOne(e => e.Post)
                .WithOne(p => p.Engagement)
                .HasForeignKey<PostEngagement>(e => e.PostId)
                .OnDelete(DeleteBehavior.Cascade);

            // Create indexes
            builder.HasIndex(e => e.PostId).IsUnique();
            builder.HasIndex(e => e.Views);
            builder.HasIndex(e => e.EngagementVelocity);
        }
    }
}
