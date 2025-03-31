namespace Meritocious.Infrastructure.Data.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using Meritocious.Core.Entities;

    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(u => u.Id);

            builder.Property(u => u.Username)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(u => u.PasswordHash)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(u => u.MeritScore)
                .HasPrecision(5, 2)
                .HasDefaultValue(0.00m);

            builder.HasIndex(u => u.Username)
                .IsUnique();

            builder.HasIndex(u => u.Email)
                .IsUnique();

            builder.HasMany(u => u.Posts)
                .WithOne(p => p.Author)
                .HasForeignKey(p => p.AuthorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(u => u.Comments)
                .WithOne(c => c.Author)
                .HasForeignKey(c => c.AuthorId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}