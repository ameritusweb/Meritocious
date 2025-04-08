using Meritocious.Core.Entities;
using Meritocious.Core.Features.Recommendations.Models;

namespace Meritocious.Core.Interfaces
{
    public interface IForkRequestRepository
    {
        Task<List<ForkRequest>> GetOpenRequestsByPlatform(string platform);
    }
}
