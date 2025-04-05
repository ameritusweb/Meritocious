using Meritocious.Common.Enums;
using Meritocious.Core.Features.Versioning;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Meritocious.Core.Entities
{
    public class ContentVersion : BaseEntity<ContentVersion>
    {
        public Guid ContentId { get; private set; }
        public ContentType ContentType { get; private set; }
        public int VersionNumber { get; private set; }
        public Guid PostId { get; private set; }
        public string Title { get; private set; }
        public string Content { get; private set; }
        public string EditorId { get; private set; }
        public User Editor { get; private set; }
        public string EditReason { get; private set; }
        public decimal MeritScore { get; private set; }
        public Dictionary<string, decimal> MeritScoreComponents { get; private set; }
        public bool IsModerationEdit { get; private set; }
        public string ModeratorNotes { get; private set; }
        public ContentEditType EditType { get; private set; }

        private ContentVersion()
        {
            MeritScoreComponents = new Dictionary<string, decimal>();
        }

        public static ContentVersion Create(
            Guid contentId,
            ContentType contentType,
            int versionNumber,
            string title,
            string content,
            User editor,
            string editReason,
            decimal meritScore,
            Dictionary<string, decimal> meritScoreComponents,
            ContentEditType editType,
            bool isModerationEdit = false,
            string moderatorNotes = null)
        {
            return new ContentVersion
            {
                ContentId = contentId,
                ContentType = contentType,
                VersionNumber = versionNumber,
                Title = title,
                Content = content,
                EditorId = editor.Id,
                Editor = editor,
                EditReason = editReason,
                MeritScore = meritScore,
                MeritScoreComponents = meritScoreComponents,
                EditType = editType,
                IsModerationEdit = isModerationEdit,
                ModeratorNotes = moderatorNotes,
                CreatedAt = DateTime.UtcNow
            };
        }
    }
}
