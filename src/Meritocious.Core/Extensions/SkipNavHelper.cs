using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meritocious.Core.Extensions
{
    using Microsoft.EntityFrameworkCore.Metadata;

    public static class SkipNavHelper
    {
        /// <summary>
        /// Determines if the given navigation should be marked as 'onDependent' based on foreign keys in the join entity.
        /// </summary>
        public static bool ResolveOnDependent(
            IEntityType joinEntity,
            IEntityType declaringEntity,
            IEntityType targetEntity)
        {
            // Look for the FK in the join entity that points to the target entity
            var fkToDeclaring = joinEntity.GetForeignKeys()
        .FirstOrDefault(fk => fk.PrincipalEntityType == declaringEntity && fk.DeclaringEntityType == joinEntity);

            if (fkToDeclaring == null)
            {
                return false;
            }

            // If the declaring entity is the *principal* in the FK, then it's NOT on the dependent side
            // So return false. Otherwise, it IS on dependent
            return fkToDeclaring.PrincipalEntityType != declaringEntity;
        }
    }
}
