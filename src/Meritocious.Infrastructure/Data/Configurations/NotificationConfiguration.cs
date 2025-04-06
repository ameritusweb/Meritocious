using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using Meritocious.Core.Entities;

namespace Meritocious.Infrastructure.Data.Configurations
{
    public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> builder)
        {     
            builder.Property(n => n.Type)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(n => n.Title)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(n => n.Message)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(n => n.Link)
                .HasMaxLength(255);

            builder.HasOne(n => n.User)
                .WithMany()
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(n => n.Post)
                .WithMany()
                .HasForeignKey(n => n.PostId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(n => n.Comment)
                .WithMany()
                .HasForeignKey(n => n.CommentId)
                .OnDelete(DeleteBehavior.NoAction);

            // Create indexes for commonly queried fields
            builder.HasIndex(n => n.UserId);  // Used for getting user's notifications
            builder.HasIndex(n => n.CreatedAt);  // Used for sorting by date
            builder.HasIndex(n => n.IsRead);  // Used for filtering unread notifications
            builder.HasIndex(n => n.Type);  // Used for filtering by notification type
            
            // Composite indexes for common query patterns
            builder.HasIndex(n => new { n.UserId, n.IsRead });  // For getting user's unread notifications
            builder.HasIndex(n => new { n.UserId, n.CreatedAt });  // For getting user's notifications by date
            builder.HasIndex(n => new { n.UserId, n.Type });  // For getting user's notifications by type
        }
    }
}