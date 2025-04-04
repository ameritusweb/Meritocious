using Meritocious.Common.Enums;
using Meritocious.Core.Features.Recommendations.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meritocious.Core.Interfaces
{
    public interface IUserInteractionRepository
    {
        Task<List<UserContentInteraction>> GetUserInteractionsAsync(Guid userId, DateTime? since = null);
        Task<List<UserContentInteraction>> GetContentInteractionsAsync(Guid contentId, ContentType contentType, DateTime? since = null);
        Task<Dictionary<string, decimal>> GetUserInteractionPatternsAsync(Guid userId);
    }
}
