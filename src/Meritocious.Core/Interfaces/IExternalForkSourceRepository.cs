using Meritocious.Core.Entities;
using Meritocious.Core.Features.Recommendations.Models;

namespace Meritocious.Core.Interfaces
{
    public interface IExternalForkSourceRepository
    {
        Task<List<ExternalForkSource>> GetSourcesWithLocationTypeAsync(string locationType, CancellationToken cancellationToken = default);
    }
}
