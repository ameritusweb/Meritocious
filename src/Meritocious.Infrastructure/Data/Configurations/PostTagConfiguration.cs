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
    public class PostTagConfiguration : IEntityTypeConfiguration<PostTag>
    {
        public void Configure(EntityTypeBuilder<PostTag> builder)
        {
            builder.ToTable("PostTags");

            builder.HasKey("PostId", "TagId");

            builder.Property(pt => pt.PostId)
                   .IsRequired()
                   .HasMaxLength(26);

            builder.Property(pt => pt.TagId)
                   .IsRequired()
                   .HasMaxLength(26);

            builder.HasOne(pt => pt.Post)
                   .WithMany()
                   .HasForeignKey(pt => pt.PostId);

            builder.HasOne(pt => pt.Tag)
                   .WithMany()
                   .HasForeignKey(pt => pt.TagId);
        }
    }
}