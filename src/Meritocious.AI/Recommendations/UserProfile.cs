using Meritocious.Core.Features.Recommendations.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meritocious.AI.Recommendations
{
    public class UserProfile
    {
        public string UserId { get; set; }
        public List<UserInteractionHistory> InteractionHistory { get; set; } = new List<UserInteractionHistory>();
        public Dictionary<string, decimal> TopicPreferences { get; set; } = new Dictionary<string, decimal>();
        public Dictionary<string, decimal> InteractionPatterns { get; set; } = new Dictionary<string, decimal>();
    }
}
