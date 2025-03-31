

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

            builder.Property(p => p.MeritScore)
                .HasPrecision(5, 2)
                .HasDefaultValue(0.00m);

            builder.HasOne(p => p.ParentPost)
                .WithMany(p => p.Forks)
                .HasForeignKey(p => p.ParentPostId)
                .OnDelete(DeleteBehavior.Restrict);

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