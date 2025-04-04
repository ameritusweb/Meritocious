using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meritocious.Core.Entities
{
    public class TagSynonym : BaseEntity
    {
        public Guid TagId { get; private set; }
        public Tag Tag { get; private set; }
        public string Name { get; private set; }
        public string Slug { get; private set; }
        public Guid CreatorId { get; private set; }
        public User Creator { get; private set; }
        public int UseCount { get; private set; }
        public bool IsApproved { get; private set; }
        public DateTime? ApprovedAt { get; private set; }
        public Guid? ApprovedById { get; private set; }
        public User ApprovedBy { get; private set; }

        private TagSynonym()
        {
        }

        public static TagSynonym Create(Tag tag, string name, User creator)
        {
            return new TagSynonym
            {
                TagId = tag.Id,
                Tag = tag,
                Name = name,
                Slug = Tag.GenerateSlug(name),
                CreatorId = Guid.Parse(creator.Id),
                Creator = creator,
                UseCount = 0,
                IsApproved = false,
                CreatedAt = DateTime.UtcNow,
            };
        }

        public void Approve(User approver)
        {
            IsApproved = true;
            ApprovedAt = DateTime.UtcNow;
            ApprovedById = Guid.Parse(approver.Id);
            ApprovedBy = approver;
            UpdatedAt = DateTime.UtcNow;
        }

        public void IncrementUseCount()
        {
            UseCount++;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
