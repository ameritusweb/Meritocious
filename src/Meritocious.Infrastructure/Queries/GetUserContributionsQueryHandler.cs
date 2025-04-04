using MediatR;
using Microsoft.EntityFrameworkCore;
using Meritocious.Core.Features.Users.Queries;
using Meritocious.Common.DTOs;
using Meritocious.Infrastructure.Data;
using Meritocious.Common.DTOs.Contributions;

namespace Meritocious.Infrastructure.Queries;

public class GetUserContributionsQueryHandler : IRequestHandler<GetUserContributionsQuery, IEnumerable<ContributionSummaryDto>>
{
    private readonly MeritociousDbContext context;

    public GetUserContributionsQueryHandler(MeritociousDbContext context)
    {
        this.context = context;
    }

    public async Task<IEnumerable<ContributionSummaryDto>> Handle(GetUserContributionsQuery request, CancellationToken cancellationToken)
    {
        var skip = (request.Page - 1) * request.PageSize;

        return await context.Posts
            .Where(p => p.AuthorId.ToString() == request.UserId)
            .OrderByDescending(p => p.CreatedAt)
            .Skip(skip)
            .Take(request.PageSize)
            .Select(p => new ContributionSummaryDto
            {
                Id = p.Id,
                Title = p.Title,
                CreatedAt = p.CreatedAt,
                Type = "Post",
                MeritScore = p.MeritScore
            })
            .ToListAsync(cancellationToken);
    }
}