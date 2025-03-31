

namespace Meritocious.Infrastructure.Data
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Threading.Tasks;

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

        private static async Task SeedDataAsync(MeritociousDbContext context, ILogger logger)
        {
            // Add any seed data here
            if (!await context.Tags.AnyAsync())
            {
                logger.LogInformation("Seeding initial tags...");

                var defaultTags = new[]
                {
                    new Core.Entities.Tag { Name = "Discussion", Description = "General discussions" },
                    new Core.Entities.Tag { Name = "Question", Description = "Questions and inquiries" },
                    new Core.Entities.Tag { Name = "Insight", Description = "Insightful contributions" },
                    new Core.Entities.Tag { Name = "Meta", Description = "About Meritocious" }
                };

                await context.Tags.AddRangeAsync(defaultTags);
                await context.SaveChangesAsync();
            }
        }
    }
}