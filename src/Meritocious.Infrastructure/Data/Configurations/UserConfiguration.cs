namespace Meritocious.Infrastructure.Data.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using Meritocious.Core.Entities;

    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.Property(u => u.UserName)
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

            builder.HasIndex(u => u.UserName)
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

            builder.HasMany(u => u.MeritScoreHistories)
                .WithOne()
                .HasForeignKey("UserId")
                .HasPrincipalKey(u => u.Id)
                .OnDelete(DeleteBehavior.Cascade);

            builder
                .HasMany<Substack>(u => u.FollowedSubstacks)
                .WithMany(s => s.Followers);

                // .UsingEntity<SubstackFollower>(
                //    "SubstackFollowers",
                //    j => j.HasOne<Substack>().WithMany().HasForeignKey("SubstackId"),
                //    j => j.HasOne<User>().WithMany().HasForeignKey("UserId"),
                //    j =>
                //    {
                //        j.HasKey("UserId", "SubstackId");
                //        j.Property(sf => sf.SubstackId)
                //        .HasConversion(new UlidIdConverter<Substack>())
                //        .HasMaxLength(26)
                //        .IsUnicode(false)
                //        .IsRequired();
                //    });
            // builder.HasMany(s => s.Followers)
            // .WithMany(u => u.FollowedSubstacks)
            // .UsingEntity<Dictionary<string, object>>(
            //    "SubstackFollowers",
            //    j => j.HasOne<User>().WithMany().HasForeignKey("UserId"),
            //    j => j.HasOne<Substack>().WithMany().HasForeignKey("SubstackId"),
            //    j =>
            //    {
            //        j.HasKey("UserId", "SubstackId");
            //        j.ToTable("SubstackFollowers");
            //    });
        }
    }
}