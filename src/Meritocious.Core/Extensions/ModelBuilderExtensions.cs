using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Meritocious.Core.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.VisualBasic;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Reflection.Emit;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations.Schema;

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

                var entityBuilder = modelBuilder.Entity(clrType);

                // Handle main UlidId property
                var ulidProperty = clrType.GetProperty("UlidId", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (ulidProperty != null)
                {
                    ApplyUlidIdConversion(entityBuilder, ulidProperty);
                }

                // Handle [ForeignKey] UlidId<T> props
                foreach (var prop in clrType.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
                {
                    var type = prop.PropertyType;
                    var underlying = Nullable.GetUnderlyingType(type);

                    var effectiveType = underlying ?? type;

                    if (effectiveType.IsGenericType &&
                        effectiveType.GetGenericTypeDefinition() == typeof(UlidId<>) &&
                        prop.GetCustomAttribute<ForeignKeyAttribute>() != null)
                    {
                        ApplyUlidIdConversion(entityBuilder, prop);
                    }
                }
            }
        }

        private static void ApplyUlidIdConversion(EntityTypeBuilder entityBuilder, PropertyInfo property)
        {
            var propertyType = property.PropertyType;
            var isNullable = Nullable.GetUnderlyingType(propertyType) != null;

            var ulidType = isNullable ? Nullable.GetUnderlyingType(propertyType) ! : propertyType;

            var toString = CreateToStringLambda(ulidType, isNullable);
            var fromString = CreateFromStringLambda(ulidType, isNullable);

            var converterType = typeof(ValueConverter<,>).MakeGenericType(propertyType, typeof(string));
            var converter = Activator.CreateInstance(converterType, toString, fromString, null);

            entityBuilder
                .Property(propertyType, property.Name)
                .HasConversion((ValueConverter)converter!)
                .HasMaxLength(26)
                .IsUnicode(false)
                .IsRequired(!isNullable); // optional if nullable
        }

        public static void AddMissingSkipNavigations(this ModelBuilder modelBuilder)
        {
            // Example: Substack.Followers ↔ User.FollowedSubstacks (only one side defined)
            FixUnidirectionalSkipNavigation<Substack, User>(
                modelBuilder,
                navigationNameOnA: "Followers",
                inverseNameOnB: "FollowedSubstacks");

            FixUnidirectionalSkipNavigation<Tag, Post>(
                modelBuilder,
                navigationNameOnA: "Posts",
                inverseNameOnB: "Tags");
        }

        private static void FixUnidirectionalSkipNavigation<TSource, TTarget>(
            ModelBuilder modelBuilder,
            string navigationNameOnA,
            string inverseNameOnB)
            where TSource : class
            where TTarget : class
        {
            var sourceEntity = modelBuilder.Model.FindEntityType(typeof(TSource));
            var targetEntity = modelBuilder.Model.FindEntityType(typeof(TTarget));

            if (sourceEntity == null || targetEntity == null)
            {
                return;
            }

            var skipNav = sourceEntity.FindSkipNavigation(navigationNameOnA) as SkipNavigation;
            if (skipNav == null || skipNav.Inverse != null)
            {
                return;
            }

            var onDependent = SkipNavHelper.ResolveOnDependent(
                joinEntity: skipNav.JoinEntityType,
                declaringEntity: sourceEntity as IEntityType,
                targetEntity: targetEntity as IEntityType);

            // Add inverse skip navigation on the target side
            var inverse = targetEntity.AddSkipNavigation(
                name: inverseNameOnB,
                navigationType: typeof(ICollection<TSource>),
                memberInfo: null,
                targetEntityType: sourceEntity,
                collection: true,
                onDependent: onDependent) as SkipNavigation;

            if (inverse != null)
            {
                inverse.SetInverse(skipNav, ConfigurationSource.Convention);
                skipNav.SetInverse(inverse, ConfigurationSource.Convention);
                if (inverse.ForeignKey == null)
                {
                    var joinEntity = inverse.JoinEntityType ?? skipNav.JoinEntityType;

                    if (joinEntity != null)
                    {
                        var fkToTarget = joinEntity
                            .GetForeignKeys()
                            .FirstOrDefault(fk => fk.PrincipalEntityType == targetEntity);

                        if (fkToTarget != null)
                        {
                            inverse.SetForeignKey(fkToTarget, ConfigurationSource.Convention);
                        }
                    }
                }
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

                    if (prop.PropertyType.IsGenericType)
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

        private static LambdaExpression CreateToStringLambda(Type ulidIdType, bool isNullable)
        {
            var param = Expression.Parameter(isNullable ? typeof(Nullable<>).MakeGenericType(ulidIdType) : ulidIdType, "v");
            Expression body;

            if (isNullable)
            {
                var hasValue = Expression.Property(param, "HasValue");
                var value = Expression.Property(param, "Value");
                var valueProp = Expression.Property(value, "Value");
                body = Expression.Condition(
                    hasValue,
                    valueProp,
                    Expression.Constant(null, typeof(string)));
            }
            else
            {
                body = Expression.Property(param, "Value");
            }

            return Expression.Lambda(body, param);
        }

        private static LambdaExpression CreateFromStringLambda(Type ulidIdType, bool isNullable)
        {
            var param = Expression.Parameter(typeof(string), "v");
            var ctor = ulidIdType.GetConstructor(new[] { typeof(string) });

            Expression body = Expression.New(ctor!, param);
            if (isNullable)
            {
                var nullableType = typeof(Nullable<>).MakeGenericType(ulidIdType);
                body = Expression.Condition(
                    Expression.Equal(param, Expression.Constant(null, typeof(string))),
                    Expression.Default(nullableType),
                    Expression.Convert(body, nullableType));
            }

            return Expression.Lambda(body, param);
        }
    }
}
