using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Meritocious.Core.Entities;

namespace Meritocious.Infrastructure.Data.Configurations
{
    public class ExternalLoginConfiguration : IEntityTypeConfiguration<ExternalLogin>
    {
        public void Configure(EntityTypeBuilder<ExternalLogin> builder)
        {
            builder.HasKey(e => e.Id);

            builder.Property(e => e.Provider)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(e => e.ProviderKey)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(e => e.Email)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(e => e.PictureUrl)
                .HasMaxLength(2048);

            builder.Property(e => e.RefreshToken)
                .HasMaxLength(1024);

            builder.HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(e => new { e.Provider, e.ProviderKey })
                .IsUnique();

            builder.HasIndex(e => e.Email);
            builder.HasIndex(e => e.LastLoginAt);
        }
    }
}