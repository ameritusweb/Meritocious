using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore;

namespace Meritocious.Infrastructure.Validation;

public class SoftSkipNavigationValidator : ModelValidator
{
    public SoftSkipNavigationValidator(ModelValidatorDependencies dependencies)
        : base(dependencies)
    {
    }

    protected override void ValidateRelationships(
        IModel model,
        IDiagnosticsLogger<DbLoggerCategory.Model.Validation> logger)
    {
        foreach (var entityType in model.GetEntityTypes())
        {
            foreach (var skip in entityType.GetDeclaredSkipNavigations())
            {
                if (skip.Inverse == null)
                {
                    var candidate = skip.TargetEntityType
                        .GetSkipNavigations()
                        .FirstOrDefault(s =>
                            s.Inverse == null &&
                            s.TargetEntityType == skip.DeclaringEntityType);

                    var all = skip.TargetEntityType
                        .GetSkipNavigations().Where(s => s.Inverse == null).ToList();

                    if (candidate != null)
                    {
                        // Automatically link both as inverses
                        // ((SkipNavigation)skip).SetInverse((SkipNavigation)candidate, ConfigurationSource.Convention);
                        // ((SkipNavigation)candidate).SetInverse((SkipNavigation)skip, ConfigurationSource.Convention);
                    }
                }
            }
        }

        base.ValidateRelationships(model, logger);
    }
}
