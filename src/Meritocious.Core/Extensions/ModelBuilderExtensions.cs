using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Meritocious.Core.Entities;

namespace Meritocious.Core.Extensions
{
    public static class ModelBuilderExtensions
    {
        public static void ApplyUlidIdConversions(this ModelBuilder modelBuilder)
        {
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                var clrType = entityType.ClrType;
                if (clrType == null || !IsDerivedFromBaseEntity(clrType))
                {
                    continue;
                }

                // Look for the UlidId property (not Id anymore)
                var ulidProp = entityType.FindProperty("UlidId");
                if (ulidProp == null)
                {
                    continue;
                }

                var ulidType = ulidProp.ClrType;

                if (!ulidType.IsGenericType || ulidType.GetGenericTypeDefinition() != typeof(UlidId<>))
                {
                    continue;
                }

                // Create a ValueConverter<UlidId<T>, string>
                var converterType = typeof(ValueConverter<,>).MakeGenericType(ulidType, typeof(string));
                var constructor = converterType.GetConstructor(new[] { typeof(Func<,>).MakeGenericType(ulidType, typeof(string)), typeof(Func<,>).MakeGenericType(typeof(string), ulidType) });

                if (constructor == null)
                {
                    throw new InvalidOperationException("Could not find appropriate constructor for ValueConverter.");
                }

                // Create conversion expressions dynamically
                var toStringExpr = CreateToStringLambda(ulidType);
                var fromStringExpr = CreateFromStringLambda(ulidType);

                var converter = constructor.Invoke(new object[] { toStringExpr, fromStringExpr }) as ValueConverter;
                ulidProp.SetValueConverter(converter);
                ulidProp.SetMaxLength(26);
                ulidProp.SetIsUnicode(false);
                ulidProp.IsNullable = false;
            }
        }

        private static bool IsDerivedFromBaseEntity(Type type)
        {
            while (type != null)
            {
                if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(BaseEntity<>))
                {
                    return true;
                }

                type = type.BaseType;
            }

            return false;
        }

        private static object CreateToStringLambda(Type ulidType)
        {
            var param = Expression.Parameter(ulidType, "v");
            var body = Expression.Property(param, "Value");
            var lambdaType = typeof(Func<,>).MakeGenericType(ulidType, typeof(string));
            return Expression.Lambda(lambdaType, body, param);
        }

        private static object CreateFromStringLambda(Type ulidType)
        {
            var param = Expression.Parameter(typeof(string), "v");
            var ctor = ulidType.GetConstructor(new[] { typeof(string) });
            var body = Expression.New(ctor, param);
            var lambdaType = typeof(Func<,>).MakeGenericType(typeof(string), ulidType);
            return Expression.Lambda(lambdaType, body, param);
        }
    }
}
