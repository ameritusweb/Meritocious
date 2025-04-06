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
            public static readonly string Clarity = "01HZY3Q2YXY0C7J1YV4KYD3KDH";
            public static readonly string Novelty = "01HZY3Q3DTR0PK51ZNRCP1JD36";
            public static readonly string Contribution = "01HZY3Q3P4NYP6J3A3SX2DGVFM";
            public static readonly string Civility = "01HZY3Q3XF9SSXWMTG1D06E4N1";
            public static readonly string Relevance = "01HZY3Q43VWSYQ0W2SZ6V91N1X";
        }

        public static async Task SeedAdminUserAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<Role>>();
            var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("SeedAdmin");

            var adminEmail = "admin@meritocious.com";
            var adminPassword = "GuessablePassword123!@#";
            var adminRoleName = "Admin";
            var normalizedAdminRoleName = adminRoleName.ToUpper();

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
            if (!await userManager.IsInRoleAsync(adminUser, normalizedAdminRoleName))
            {
                var addToRole = await userManager.AddToRoleAsync(adminUser, normalizedAdminRoleName);
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
            if (!await context.MeritScoreTypes.AnyAsync())
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