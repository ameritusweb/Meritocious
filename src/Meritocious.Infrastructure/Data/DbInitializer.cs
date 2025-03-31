

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