namespace Meritocious.Infrastructure.Data.Repositories
{
    using Microsoft.EntityFrameworkCore;
    using Meritocious.Core.Entities;
    using Meritocious.Core.Features.Recommendations.Models;
    using System.Linq.Expressions;
    using Meritocious.Core.Interfaces;

    public partial class ExternalForkSourceRepository : GenericRepository<ExternalForkSource>, IExternalForkSourceRepository
    {
        public ExternalForkSourceRepository(MeritociousDbContext context) : base(context)
        {
        }

        public async Task<List<ExternalForkSource>> GetSourcesWithLocationTypeAsync(
         string locationType,
         CancellationToken cancellationToken = default)
            {
                return await dbSet
                    .Where(s =>
                        s.LocationMetadata != null &&
                        s.LocationMetadata.ContainsKey("type") &&
                        s.LocationMetadata["type"].GetString() == locationType)
                    .OrderByDescending(s => s.CreatedAt)
                    .ToListAsync(cancellationToken);
            }
        }
}
