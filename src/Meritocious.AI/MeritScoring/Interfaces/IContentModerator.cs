using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meritocious.AI.Moderation.Interfaces
{
    using Meritocious.Common.Enums;

    public interface IContentModerator
    {
        Task<ModerationAction> EvaluateContentAsync(string content);
        Task<Dictionary<string, decimal>> GetToxicityScoresAsync(string content);
    }
}