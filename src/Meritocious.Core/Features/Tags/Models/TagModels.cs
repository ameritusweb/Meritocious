using System;
using System.Collections.Generic;
using System.Linq;
using Meritocious.Core.Entities;

namespace Meritocious.Core.Features.Tags.Models
{
    public enum TagCategory
    {
        Topic,
        Domain,
        Technology,
        Skill,
        Industry,
        Language,
        Location,
        Event,
        Organization,
        Project,
        Other
    }

    public enum TagStatus
    {
        Proposed,
        Active,
        Deprecated,
        Merged,
        Blacklisted
    }

    public enum TagRelationType
    {
        Related,
        RequiredBy,
        Requires,
        Specializes,
        Generalizes,
        Contradicts,
        Equivalent
    }
}