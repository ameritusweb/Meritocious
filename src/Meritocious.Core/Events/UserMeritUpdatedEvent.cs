using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meritocious.Core.Events
{
    public class UserMeritUpdatedEvent
    {
        public Guid UserId { get; }
        public decimal OldScore { get; }
        public decimal NewScore { get; }
        public DateTime UpdatedAt { get; }

        public UserMeritUpdatedEvent(Guid userId, decimal oldScore, decimal newScore)
        {
            UserId = userId;
            OldScore = oldScore;
            NewScore = newScore;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}