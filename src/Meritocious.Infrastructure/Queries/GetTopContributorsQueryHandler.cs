using MediatR;
using Meritocious.Common.DTOs.Auth;
using Meritocious.Core.Features.Users.Queries;
using Meritocious.Infrastructure.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class GetTopContributorsQueryHandler : IRequestHandler<GetTopContributorsQuery, List<UserProfileDto>>
{
    private readonly UserRepository userRepository;

    public GetTopContributorsQueryHandler(UserRepository userRepository)
    {
        this.userRepository = userRepository;
    }

    public async Task<List<UserProfileDto>> Handle(GetTopContributorsQuery request, CancellationToken cancellationToken)
    {
        var startDate = request.TimeFrame switch
        {
            "week" => DateTime.UtcNow.AddDays(-7),
            "month" => DateTime.UtcNow.AddMonths(-1),
            "year" => DateTime.UtcNow.AddYears(-1),
            _ => DateTime.MinValue
        };

        var users = await userRepository.GetTopContributorsAsync(request.Count, startDate);

        return users.Select(u => new UserProfileDto
        {
            Id = Guid.Parse(u.Id),
            Username = u.UserName,
            MeritScore = u.MeritScore,
            CreatedAt = u.CreatedAt,
            LastLoginAt = u.LastLoginAt
        }).ToList();
    }
}
