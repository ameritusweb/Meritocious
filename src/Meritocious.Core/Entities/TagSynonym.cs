using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meritocious.Core.Entities
{
    public class TagSynonym : BaseEntity<TagSynonym>
    {
        public string SourceTagId { get; private set; }
        public Tag SourceTag { get; private set; }

        public string TargetTagId { get; private set; }
        public Tag TargetTag { get; private set; }

        public string CreatedById { get; private set; }
        public User CreatedBy { get; private set; }

        public string Status { get; private set; } = "Pending";

        public DateTime CreatedAt { get; private set; }

        // Optional approval workflow
        public bool IsApproved { get; private set; }
        public DateTime? ApprovedAt { get; private set; }
        public string? ApprovedById { get; private set; }
        public User? ApprovedBy { get; private set; }
        public string Name => SourceTag?.Name ?? "(unknown)";

        private TagSynonym()
        {
        }

        public static TagSynonym Create(string sourceTagId, string targetTagId, User creator)
        {
            return new TagSynonym
            {
                SourceTagId = sourceTagId,
                TargetTagId = targetTagId,
                CreatedById = creator.Id,
                CreatedBy = creator,
                CreatedAt = DateTime.UtcNow,
                Status = "Pending",
                IsApproved = false
            };
        }

        public void Approve(User approver)
        {
            IsApproved = true;
            ApprovedAt = DateTime.UtcNow;
            ApprovedById = approver.Id;
            ApprovedBy = approver;
            Status = "Approved";
        }
    }
}
