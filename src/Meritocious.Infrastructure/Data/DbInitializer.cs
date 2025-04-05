namespace Meritocious.Infrastructure.Data
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Threading.Tasks;
    using Meritocious.Core.Entities;
    using Microsoft.AspNetCore.Identity;
    using System.Data;

    public static class DbInitializer
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var services = scope.ServiceProvider;
            try
            {
                var context = services.GetRequiredService<MeritociousDbContext>();
                var logger = services.GetRequiredService<ILogger<MeritociousDbContext>>();
                if (context.Database.IsSqlServer())
                {
                    await context.Database.MigrateAsync();
                }

                await SeedDataAsync(context, logger);
            }
            catch (Exception ex)
            {
                var logger = services.GetRequiredService<ILogger<MeritociousDbContext>>();
                logger.LogError(ex, "An error occurred while initializing the database.");
                throw;
            }
        }

        public static class DefaultMeritScoreTypes
        {
            public static readonly Guid Clarity = new("10A12E0C-9E4F-4B1C-9E9E-F0555BAFF9C2");
            public static readonly Guid Novelty = new("20B23F1D-AF5G-5C2D-AF0F-G1666CBGG0D3");
            public static readonly Guid Contribution = new("30C34G2E-BG6H-6D3E-BG1G-H2777DCHH1E4");
            public static readonly Guid Civility = new("40D45H3F-CH7I-7E4F-CH2H-I3888EDII2F5");
            public static readonly Guid Relevance = new("50E56I4G-DI8J-8F5G-DI3I-J4999FEJJ3G6");
        }

        public static async Task SeedAdminUserAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<Role>>();
            var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("SeedAdmin");

            var adminEmail = "admin@meritocious.com";
            var adminPassword = "GuessablePassword";
            var adminRoleName = "Admin";

            // Create role if it doesn't exist
            if (!await roleManager.RoleExistsAsync(adminRoleName))
            {
                var role = new Role();
                role.Name = adminRoleName;
                var roleResult = await roleManager.CreateAsync(role);
                if (!roleResult.Succeeded)
                {
                    logger.LogError("Failed to create role {Role}: {Errors}", adminRoleName, string.Join(", ", roleResult.Errors.Select(e => e.Description)));
                    return;
                }
            }

            // Create admin user if not exists
            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                adminUser = User.Create(adminEmail, adminEmail, Ulid.NewUlid().ToString());

                var createResult = await userManager.CreateAsync(adminUser, adminPassword);
                if (!createResult.Succeeded)
                {
                    logger.LogError("Failed to create admin user: {Errors}", string.Join(", ", createResult.Errors.Select(e => e.Description)));
                    return;
                }
            }

            // Assign to Admin role if not already
            if (!await userManager.IsInRoleAsync(adminUser, adminRoleName))
            {
                var addToRole = await userManager.AddToRoleAsync(adminUser, adminRoleName);
                if (!addToRole.Succeeded)
                {
                    logger.LogError("Failed to add admin to role: {Errors}", string.Join(", ", addToRole.Errors.Select(e => e.Description)));
                }
            }

            // Enable and require 2FA for admin
            adminUser.TwoFactorRequired = true;
            adminUser.TwoFactorEnabled = true;
            await userManager.UpdateAsync(adminUser);

            logger.LogInformation("Admin user seeded successfully.");
        }

        private static async Task SeedDataAsync(MeritociousDbContext context, ILogger logger)
        {
            // Seed merit score types
            if (!await context.Set<MeritScoreType>().AnyAsync())
            {
                logger.LogInformation("Seeding merit score types...");
                var defaultMeritScoreTypes = new[]
                {
                    new MeritScoreType
                    {
                        Id = DefaultMeritScoreTypes.Clarity,
                        Name = "Clarity",
                        Description = "Semantic coherence, grammar, readability, and structure",
                        Weight = 0.25m,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    },
                    new MeritScoreType
                    {
                        Id = DefaultMeritScoreTypes.Novelty,
                        Name = "Novelty",
                        Description = "Degree of semantic divergence from nearby posts",
                        Weight = 0.25m,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    },
                    new MeritScoreType
                    {
                        Id = DefaultMeritScoreTypes.Contribution,
                        Name = "Contribution",
                        Description = "How much it moves the discussion forward",
                        Weight = 0.20m,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    },
                    new MeritScoreType
                    {
                        Id = DefaultMeritScoreTypes.Civility,
                        Name = "Civility",
                        Description = "Tone, respectfulness, empathy, and non-toxic language",
                        Weight = 0.15m,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    },
                    new MeritScoreType
                    {
                        Id = DefaultMeritScoreTypes.Relevance,
                        Name = "Relevance",
                        Description = "How well it connects to the thread or topic",
                        Weight = 0.15m,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    }
                };
                await context.Set<MeritScoreType>().AddRangeAsync(defaultMeritScoreTypes);
                await context.SaveChangesAsync();
            }

            // Seed tags
            if (!await context.Tags.AnyAsync())
            {
                logger.LogInformation("Seeding initial tags...");
                var defaultTags = new[]
                {
                    Core.Entities.Tag.Create("Discussion", "General discussions", Core.Features.Tags.Models.TagCategory.Topic),
                    Core.Entities.Tag.Create("Question", "Questions and inquiries", Core.Features.Tags.Models.TagCategory.Topic),
                    Core.Entities.Tag.Create("Insight", "Insightful contributions", Core.Features.Tags.Models.TagCategory.Topic),
                    Core.Entities.Tag.Create("Meta", "About Meritocious", Core.Features.Tags.Models.TagCategory.Topic),
                };
                await context.Tags.AddRangeAsync(defaultTags);
                await context.SaveChangesAsync();
            }
        }
    }
}