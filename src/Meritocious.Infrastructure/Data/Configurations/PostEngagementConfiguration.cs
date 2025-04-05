using Meritocious.Core.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;

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
               .HasConversion(
                   v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                   v => JsonSerializer.Deserialize<Dictionary<string, int>>(v, (JsonSerializerOptions?)null))
               .HasColumnType("nvarchar(max)");

            builder.Property(e => e.ViewsByPlatform)
                .HasConversion(
                   v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                   v => JsonSerializer.Deserialize<Dictionary<string, int>>(v, (JsonSerializerOptions?)null))
               .HasColumnType("nvarchar(max)");

            builder.Property(e => e.ViewTrend)
                .HasConversion(
                   v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                   v => JsonSerializer.Deserialize<Dictionary<DateTime, int>>(v, (JsonSerializerOptions?)null))
               .HasColumnType("nvarchar(max)");

            builder.Property(e => e.SourceInfluenceScores)
                .HasConversion(
                   v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                   v => JsonSerializer.Deserialize<Dictionary<string, decimal>>(v, (JsonSerializerOptions?)null))
               .HasColumnType("nvarchar(max)");

            builder.Property(e => e.TopEngagementSources)
                .HasConversion(
                   v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                   v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions?)null))
               .HasColumnType("nvarchar(max)");

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
