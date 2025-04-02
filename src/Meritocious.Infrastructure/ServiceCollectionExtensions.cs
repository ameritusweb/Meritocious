using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meritocious.Infrastructure
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Meritocious.Core.Interfaces;
    using Meritocious.Infrastructure.Data;
    using Meritocious.Infrastructure.Data.Repositories;
    using Meritocious.Core.Services;
    using MediatR;
    using Meritocious.Core.Behaviors;
    using Meritocious.Core.Extensions;
    using Meritocious.Core.Features.Notifications.Models;
    using Meritocious.Core.Entities;
    using Meritocious.Infrastructure.Data.Services;

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMeritociousInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // Add DbContext
            services.AddDbContext<MeritociousDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly(typeof(MeritociousDbContext).Assembly.FullName)));

            // Register Repositories
            services.AddScoped(typeof(IRepository<>), typeof(GenericRepository<>));
            services.AddScoped<UserRepository>();
            services.AddScoped<PostRepository>();
            services.AddScoped<CommentRepository>();
            services.AddScoped<TagRepository>();
            services.AddScoped<RemixRepository>();
            services.AddScoped<RemixSourceRepository>();
            services.AddScoped<RemixNoteRepository>();

            // Register Core Services
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IPostService, PostService>();
            services.AddScoped<ICommentService, CommentService>();
            services.AddScoped<IMeritScoringService, MeritScoringService>();
            services.AddScoped<ITagService, TagService>();

            // Register additional services
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<IReportingService, ReportingService>();
            services.AddScoped<ISearchService, SearchService>();
            services.AddScoped<IRemixService, RemixService>();

            // Add domain validators
            services.AddDomainValidators();

            // Add repository extensions
            services.AddRepositoryExtensions();

            // Add MediatR behaviors
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(TransactionBehavior<,>));

            // Add validation behavior
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

            // Add DbSets for missing entities
            services.AddDbContext<MeritociousDbContext>((serviceProvider, options) =>
            {
                var currentOptions = serviceProvider.GetRequiredService<DbContextOptions<MeritociousDbContext>>();
                var dbContext = new MeritociousDbContext(currentOptions);

                // Ensure DbSet properties are created for all entities
                if (!dbContext.Model.FindEntityType(typeof(Notification)) != null)
                {
                    options.Entity<Notification>();
                }

                if (!dbContext.Model.FindEntityType(typeof(ContentReport)) != null)
                {
                    options.Entity<ContentReport>();
                }

                if (!dbContext.Model.FindEntityType(typeof(MeritScoreHistory)) != null)
                {
                    options.Entity<MeritScoreHistory>();
                }

                if (!dbContext.Model.FindEntityType(typeof(ContentModerationEvent)) != null)
                {
                    options.Entity<ContentModerationEvent>();
                }
            });

            return services;
        }
    }
}