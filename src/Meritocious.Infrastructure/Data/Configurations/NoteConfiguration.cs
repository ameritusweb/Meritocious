﻿using Meritocious.Core.Entities;
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
    public class NoteConfiguration : IEntityTypeConfiguration<Note>
    {
        public void Configure(EntityTypeBuilder<Note> builder)
        {
            builder.HasKey(n => n.Id);

            builder.Property(n => n.Type)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(n => n.Content)
                .IsRequired()
                .HasColumnType("ntext");

            builder.Property(n => n.RelatedSourceIds)
                .HasConversion(
                   v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                   v => JsonSerializer.Deserialize<List<Guid>>(v, (JsonSerializerOptions?)null))
               .HasColumnType("nvarchar(max)");

            builder.Property(n => n.Confidence)
                .HasPrecision(5, 2);

            builder.HasOne(n => n.Post)
                .WithMany(p => p.Notes)
                .HasForeignKey(n => n.PostId)
                .OnDelete(DeleteBehavior.Cascade);

            // Create indexes
            builder.HasIndex(n => n.PostId);
            builder.HasIndex(n => n.Type);
            builder.HasIndex(n => n.IsApplied);
        }
    }
}
