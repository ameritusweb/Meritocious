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
                var clrProperty = clrType.GetProperty("UlidId", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (clrProperty == null)
                {
                    continue;
                }

                var entityBuilder = modelBuilder.Entity(clrType);

                var propertyType = clrProperty.PropertyType;
                if (!propertyType.IsGenericType || propertyType.GetGenericTypeDefinition() != typeof(UlidId<>))
                {
                    continue;
                }

                // Create ValueConverter<UlidId<T>, string>
                var toString = CreateToStringLambda(propertyType);
                var fromString = CreateFromStringLambda(propertyType);

                var converterType = typeof(ValueConverter<,>).MakeGenericType(propertyType, typeof(string));
                var converter = Activator.CreateInstance(
                    typeof(ValueConverter<,>).MakeGenericType(propertyType, typeof(string)),
                    toString,
                    fromString,
                    null);

                entityBuilder
                    .Property(clrProperty.PropertyType, clrProperty.Name)
                    .HasConversion((ValueConverter)converter!)
                    .HasMaxLength(26)
                    .IsUnicode(false)
                    .IsRequired();
            }
        }

        public static void IgnoreReadOnlyProperties(this ModelBuilder modelBuilder)
        {
            foreach (var entityType in modelBuilder.Model.GetEntityTypes().ToList())
            {
                var clrType = entityType.ClrType;

                // Skip shadow/entity types with no CLR type
                if (clrType == null || entityType.IsPropertyBag)
                {
                    continue;
                }

                // Get all public instance properties
                var props = clrType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

                foreach (var prop in props)
                {
                    // Skip if property has a setter (it's writable)
                    if (prop.SetMethod != null)
                    {
                        continue;
                    }

                    // Skip if it's an indexer or compiler-generated
                    if (prop.GetIndexParameters().Length > 0)
                    {
                        continue;
                    }

                    // Skip if already ignored/mapped (just to be safe)
                    if (entityType.FindProperty(prop.Name) != null)
                    {
                        continue;
                    }

                    // Ignore it
                    modelBuilder.Entity(clrType).Ignore(prop.Name);
                }
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
            var valueProp = Expression.Property(param, "Value");
            var delegateType = typeof(Func<,>).MakeGenericType(ulidType, typeof(string));
            return Expression.Lambda(delegateType, valueProp, param);
        }

        private static object CreateFromStringLambda(Type ulidType)
        {
            var param = Expression.Parameter(typeof(string), "v");
            var ctor = ulidType.GetConstructor(new[] { typeof(string) });
            if (ctor == null)
            {
                throw new InvalidOperationException($"No string constructor found on {ulidType.Name}");
            }

            var newExpr = Expression.New(ctor, param);
            var delegateType = typeof(Func<,>).MakeGenericType(typeof(string), ulidType);
            return Expression.Lambda(delegateType, newExpr, param);
        }
    }
}
