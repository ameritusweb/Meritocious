﻿using Meritocious.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meritocious.Core.Entities
{
    public class ContentReport : BaseEntity<ContentReport>
    {
        public string ContentId { get; private set; }

        public ContentType ContentType { get; private set; }

        public string ReporterId { get; private set; }

        public User Reporter { get; private set; }

        public string ReportType { get; private set; }

        public string Description { get; private set; }

        public string Status { get; private set; }

        public string? ModeratorId { get; private set; }

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
