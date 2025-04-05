using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meritocious.Core.Events
{
    using Meritocious.Common.Enums;
    using Meritocious.Common.DTOs.Merit;

    public class ContentScoredEvent
    {
        public string ContentId { get; }
        public ContentType ContentType { get; }
        public MeritScoreDto Score { get; }
        public DateTime ScoredAt { get; }

        public ContentScoredEvent(string contentId, ContentType contentType, MeritScoreDto score)
        {
            ContentId = contentId;
            ContentType = contentType;
            Score = score;
            ScoredAt = DateTime.UtcNow;
        }
    }
}