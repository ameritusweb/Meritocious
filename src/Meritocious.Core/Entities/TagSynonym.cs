using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meritocious.Core.Entities
{
    public class TagSynonym : BaseEntity
    {
        public Guid SourceTagId { get; private set; }
        public Tag SourceTag { get; private set; }

        public Guid TargetTagId { get; private set; }
        public Tag TargetTag { get; private set; }

        public Guid CreatedById { get; private set; }
        public User CreatedBy { get; private set; }

        public string Status { get; private set; } = "Pending";

        public DateTime CreatedAt { get; private set; }

        // Optional approval workflow
        public bool IsApproved { get; private set; }
        public DateTime? ApprovedAt { get; private set; }
        public Guid? ApprovedById { get; private set; }
        public User? ApprovedBy { get; private set; }
        public string Name => SourceTag?.Name ?? "(unknown)";

        private TagSynonym()
        {
        }

        public static TagSynonym Create(Guid sourceTagId, Guid targetTagId, User creator)
        {
            return new TagSynonym
            {
                SourceTagId = sourceTagId,
                TargetTagId = targetTagId,
                CreatedById = Guid.Parse(creator.Id),
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
            ApprovedById = Guid.Parse(approver.Id);
            ApprovedBy = approver;
            Status = "Approved";
        }
    }
}
