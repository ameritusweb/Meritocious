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

            // Register Services
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IPostService, PostService>();
            services.AddScoped<ICommentService, CommentService>();
            services.AddScoped<IMeritScoringService, MeritScoringService>();
            services.AddScoped<ITagService, TagService>();

            // Add domain validators
            services.AddDomainValidators();

            // Add MediatR behaviors
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(TransactionBehavior<,>));

            // Add validation behavior
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));


            return services;
        }
    }
}