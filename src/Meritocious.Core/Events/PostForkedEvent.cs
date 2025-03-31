using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meritocious.Core.Events
{
    public class PostForkedEvent
    {
        public Guid OriginalPostId { get; }
        public Guid ForkedPostId { get; }
        public Guid ForkedByUserId { get; }
        public DateTime ForkedAt { get; }

        public PostForkedEvent(Guid originalPostId, Guid forkedPostId, Guid forkedByUserId)
        {
            OriginalPostId = originalPostId;
            ForkedPostId = forkedPostId;
            ForkedByUserId = forkedByUserId;
            ForkedAt = DateTime.UtcNow;
        }
    }
}