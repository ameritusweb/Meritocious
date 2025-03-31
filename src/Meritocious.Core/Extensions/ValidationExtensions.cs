using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meritocious.Core.Extensions
{
    using FluentValidation;
    using Microsoft.Extensions.DependencyInjection;
    using System.Reflection;

    public static class ValidationExtensions
    {
        public static IServiceCollection AddDomainValidators(this IServiceCollection services)
        {
            // Register all validators in the Core assembly
            var assembly = Assembly.GetExecutingAssembly();
            var validatorType = typeof(IValidator<>);

            var validatorTypes = assembly.GetTypes()
                .Where(t => t.GetInterfaces()
                    .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == validatorType));

            foreach (var validator in validatorTypes)
            {
                var entityType = validator.GetInterfaces()
                    .First(i => i.IsGenericType && i.GetGenericTypeDefinition() == validatorType)
                    .GetGenericArguments()[0];

                services.AddScoped(typeof(IValidator<>).MakeGenericType(entityType), validator);
            }

            return services;
        }
    }
}