using Microsoft.Extensions.DependencyInjection;
using System;

namespace Meritocious.Infrastructure.Data
{
    public static class RepositoryExtensionsRegistration
    {
        public static IServiceCollection AddRepositoryExtensions(this IServiceCollection services)
        {
            // Nothing to register directly, but this method serves as a marker
            // to ensure the extension methods are compiled and available

            return services;
        }
    }
}