namespace Meritocious.Infrastructure.Data.Repositories
{
    using Microsoft.EntityFrameworkCore;
    using Meritocious.Core.Entities;
    using Meritocious.Core.Features.Recommendations.Models;
    using System.Linq.Expressions;
    using Meritocious.Core.Interfaces;

    public partial class ForkRequestRepository : GenericRepository<ForkRequest>, IForkRequestRepository
    {
        public ForkRequestRepository(MeritociousDbContext context) : base(context)
        {
        }

        public async Task<List<ForkRequest>> GetOpenRequestsByPlatform(
            string platform)
        {
            return await dbSet
                .Include(r => r.ExternalForkSource)
                .Where(r =>
                    r.Status == "open" &&
                    r.ExternalForkSource.Platform == platform).ToListAsync();
        }
    }
}
