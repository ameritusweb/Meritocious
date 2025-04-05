using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meritocious.Core.Events
{
    public class PostForkedEvent
    {
        public string OriginalPostId { get; }
        public string ForkedPostId { get; }
        public string ForkedByUserId { get; }
        public DateTime ForkedAt { get; }

        public PostForkedEvent(string originalPostId, string forkedPostId, string forkedByUserId)
        {
            OriginalPostId = originalPostId;
            ForkedPostId = forkedPostId;
            ForkedByUserId = forkedByUserId;
            ForkedAt = DateTime.UtcNow;
        }
    }
}