using Meritocious.Core.Features.Tags.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meritocious.Core.Entities
{
    public class TagRelationship : BaseEntity
    {
        public Guid SourceTagId { get; private set; }
        public Tag SourceTag { get; private set; }
        public Guid RelatedTagId { get; private set; }
        public Tag RelatedTag { get; private set; }
        public TagRelationType RelationType { get; private set; }
        public decimal Strength { get; private set; }
        public string CreatorId { get; private set; }
        public User Creator { get; private set; }
        public bool IsBidirectional { get; private set; }
        public bool IsApproved { get; private set; }
        public DateTime? ApprovedAt { get; private set; }
        public string? ApprovedById { get; private set; }
        public User ApprovedBy { get; private set; }
        public string ParentTagId { get; internal set; }
        public string ChildTagId { get; internal set; }

        private TagRelationship()
        {
        }

        public static TagRelationship Create(
            Tag sourceTag,
            Tag relatedTag,
            TagRelationType relationType,
            decimal strength,
            User creator,
            bool isBidirectional = true)
        {
            return new TagRelationship
            {
                SourceTagId = sourceTag.Id,
                SourceTag = sourceTag,
                RelatedTagId = relatedTag.Id,
                RelatedTag = relatedTag,
                RelationType = relationType,
                Strength = strength,
                CreatorId = creator.Id,
                Creator = creator,
                IsBidirectional = isBidirectional,
                IsApproved = false,
                CreatedAt = DateTime.UtcNow
            };
        }

        public void Approve(User approver)
        {
            IsApproved = true;
            ApprovedAt = DateTime.UtcNow;
            ApprovedById = approver.Id;
            ApprovedBy = approver;
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateStrength(decimal newStrength)
        {
            Strength = newStrength;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
