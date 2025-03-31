using Meritocious.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meritocious.Core.Features.Versioning
{
    public enum ContentEditType
    {
        Creation,
        MinorEdit,
        MajorEdit,
        ModeratorEdit,
        SystemEdit,
        AutomaticCorrection,
        CommunityEdit,
        Restoration
    }

    public class ContentDiff
    {
        public Guid Id { get; private set; }
        public Guid ContentVersionId { get; private set; }
        public ContentVersion ContentVersion { get; private set; }
        public string DiffData { get; private set; }  // JSON diff data
        public string TitleDiff { get; private set; }
        public decimal MeritScoreDiff { get; private set; }
        public Dictionary<string, decimal> ComponentDiffs { get; private set; }
        public DateTime CreatedAt { get; private set; }

        private ContentDiff()
        {
            ComponentDiffs = new Dictionary<string, decimal>();
        }

        public static ContentDiff Create(
            ContentVersion version,
            string diffData,
            string titleDiff,
            decimal meritScoreDiff,
            Dictionary<string, decimal> componentDiffs)
        {
            return new ContentDiff
            {
                Id = Guid.NewGuid(),
                ContentVersionId = version.Id,
                ContentVersion = version,
                DiffData = diffData,
                TitleDiff = titleDiff,
                MeritScoreDiff = meritScoreDiff,
                ComponentDiffs = componentDiffs,
                CreatedAt = DateTime.UtcNow
            };
        }
    }

    public interface IVersionable
    {
        Guid Id { get; }
        int CurrentVersion { get; }
        DateTime LastEditedAt { get; }
        List<ContentVersion> Versions { get; }
        ContentVersion CreateVersion(
            User editor,
            string editReason,
            ContentEditType editType,
            bool isModerationEdit = false,
            string moderatorNotes = null);
        void RestoreVersion(int versionNumber, User editor, string reason);
    }
}
