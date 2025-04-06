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
    public class SubstackFollowerConfiguration : IEntityTypeConfiguration<SubstackFollower>
    {
        public void Configure(EntityTypeBuilder<SubstackFollower> builder)
        {
            builder.ToTable("SubstackFollowers");

            builder.HasKey("SubstackId", "UserId");

            builder.Property(pt => pt.SubstackId)
                   .IsRequired()
                   .HasMaxLength(26);

            builder.Property(pt => pt.UserId)
                   .IsRequired()
                   .HasMaxLength(26);

            builder.HasOne(pt => pt.Substack)
                   .WithMany()
                   .HasForeignKey(pt => pt.SubstackId);

            builder.HasOne(pt => pt.User)
                   .WithMany()
                   .HasForeignKey(pt => pt.UserId);
        }
    }
}
