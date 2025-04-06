using Meritocious.Common.Enums;
using Meritocious.Core.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meritocious.Core.Entities
{
    public class ContentReport : BaseEntity<ContentReport>
    {
        public string ContentId { get; private set; }

        public ContentType ContentType { get; private set; }

        [ForeignKey("FK_ReporterId")]
        public UlidId<User> ReporterId { get; private set; }

        public User Reporter { get; private set; }

        public string ReportType { get; private set; }

        public string Description { get; private set; }

        public string Status { get; private set; }

        [ForeignKey("FK_ModeratorId")]
        public UlidId<User>? ModeratorId { get; private set; }

        public User Moderator { get; private set; }

        public string Resolution { get; private set; }

        public string Notes { get; private set; }

        public DateTime? ResolvedAt { get; private set; }

        private ContentReport()
        {
        }

        public static ContentReport Create(
            string contentId,
            ContentType contentType,
            string reporterId,
            string reportType,
            string description)
        {
            return new ContentReport
            {
                ContentId = contentId,
                ContentType = contentType,
                ReporterId = reporterId,
                ReportType = reportType,
                Description = description,
                Status = "pending",
                CreatedAt = DateTime.UtcNow
            };
        }

        public void Resolve(string moderatorId, string resolution, string notes)
        {
            ModeratorId = moderatorId;
            Resolution = resolution;
            Notes = notes;
            Status = "resolved";
            ResolvedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
