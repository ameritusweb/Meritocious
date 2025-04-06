using Meritocious.Core.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using Meritocious.Core.Extensions;

namespace Meritocious.Infrastructure.Data.Configurations
{
    public class PostEngagementConfiguration : IEntityTypeConfiguration<PostEngagement>
    {
        public void Configure(EntityTypeBuilder<PostEngagement> builder)
        {
            var (converterDecimal, comparerDecimal) = EfHelpers.For<Dictionary<string, decimal>>();
            var (converterInt, comparerInt) = EfHelpers.For<Dictionary<string, int>>();
            var (converterDateTime, comparerDateTime) = EfHelpers.For<Dictionary<DateTime, int>>();
            var (converter, comparer) = EfHelpers.For<List<string>>();

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
               .HasConversion(converterInt)
               .HasColumnType("nvarchar(max)")
               .Metadata.SetValueComparer(comparerInt);

            builder.Property(e => e.ViewsByPlatform)
                .HasConversion(converterInt)
               .HasColumnType("nvarchar(max)")
               .Metadata.SetValueComparer(comparerInt);

            builder.Property(e => e.ViewTrend)
                .HasConversion(converterDateTime)
               .HasColumnType("nvarchar(max)")
               .Metadata.SetValueComparer(comparerDateTime);

            builder.Property(e => e.SourceInfluenceScores)
                .HasConversion(converterDecimal)
               .HasColumnType("nvarchar(max)")
               .Metadata.SetValueComparer(comparerDecimal);

            builder.Property(e => e.TopEngagementSources)
                .HasConversion(converter)
               .HasColumnType("nvarchar(max)")
               .Metadata.SetValueComparer(comparer);

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
