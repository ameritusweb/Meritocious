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
    using Meritocious.Core.Entities;
    using Meritocious.Infrastructure.Data.Services;
    using Microsoft.AspNetCore.Identity;
    using Meritocious.Core.Features.Substacks.Services;
    using Meritocious.Infrastructure.Services;

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

            // Add Identity
            services.AddIdentity<User, IdentityRole>(options =>
            {
                options.Password.RequiredLength = 8;
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = true;
                
                options.User.RequireUniqueEmail = true;
                options.SignIn.RequireConfirmedEmail = true;
                
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
                options.Lockout.MaxFailedAccessAttempts = 5;
            })
            .AddEntityFrameworkStores<MeritociousDbContext>()
            .AddDefaultTokenProviders();

            // Register Repositories
            services.AddScoped(typeof(IRepository<>), typeof(GenericRepository<>));
            services.AddScoped<UserRepository>();
            services.AddScoped<PostRepository>();
            services.AddScoped<CommentRepository>();
            services.AddScoped<TagRepository>();
            services.AddScoped<ContentSimilarityRepository>();
            services.AddScoped<ContentTopicRepository>();
            services.AddScoped<UserTopicPreferenceRepository>();
            services.AddScoped<TrendingContentRepository>();
            services.AddScoped<MeritScoreHistoryRepository>();

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
            services.AddHostedService<RecommendationBackgroundService>();
            services.AddScoped<IRemixService, RemixService>();
            services.AddScoped<ISubstackFeedService, SubstackFeedService>();
            services.AddHttpClient<ISubstackFeedService, SubstackFeedService>();

            // Add domain validators
            services.AddDomainValidators();

            // Add repository extensions
            services.AddRepositoryExtensions();

            // Add MediatR behaviors
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(TransactionBehavior<,>));

            // Add validation behavior
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

            return services;
        }
    }
}