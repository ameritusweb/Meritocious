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
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using Microsoft.EntityFrameworkCore.Infrastructure;
    using Meritocious.Infrastructure.Validation;
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMeritociousInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddDbContext<MeritociousDbContext>((serviceProvider, options) =>
            {
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly("Meritocious.Web"));

                options.ReplaceService<IModelValidator, SoftSkipNavigationValidator>();
            });

            // Add Identity
            services.AddIdentity<User, Role>(options =>
            {
                options.Password.RequiredLength = 8;
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = true;
                
                options.User.RequireUniqueEmail = true;
                options.SignIn.RequireConfirmedEmail = true;
            })
            .AddEntityFrameworkStores<MeritociousDbContext>()
            .AddUserStore<UserStore<User, Role, MeritociousDbContext, UlidId<User>, UserClaim, UserRole, UserLogin, UserToken, RoleClaim>>()
            .AddRoleStore<RoleStore<Role, MeritociousDbContext, UlidId<User>, UserRole, RoleClaim>>()
            .AddDefaultTokenProviders();

            services.Configure<IdentityOptions>(options =>
            {
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;
            });

            // Register Repositories
            services.AddScoped(typeof(IRepository<>), typeof(GenericRepository<>));
            services.Scan(scan => scan
                .FromAssemblyOf<PostRepository>()
                .AddClasses(classes => classes.AssignableTo(typeof(IRepository<>)))
                .AsImplementedInterfaces()
                .WithScopedLifetime());

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
            services.AddScoped<IUserPreferenceService, UserPreferenceService>();
            services.AddScoped<ITransactionService, TransactionService>();
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