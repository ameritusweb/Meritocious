using MediatR;

namespace Meritocious.Core.Features.Merit.Queries
{
    public record GetAdminStatsQuery : IRequest<AdminStats>;

    public class AdminStats
    {
        public int TotalUsers { get; set; }
        public int ActiveUsers24h { get; set; }
        public int PostsToday { get; set; }
    }
}