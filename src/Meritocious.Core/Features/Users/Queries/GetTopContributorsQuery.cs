using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meritocious.Core.Features.Users.Queries
{
    using MediatR;
    using Meritocious.Common.DTOs.Auth;

    public record GetTopContributorsQuery : IRequest<List<UserProfileDto>>
    {
        public int Count { get; init; } = 10;
        public string TimeFrame { get; init; } = "all"; // all, week, month, year
    }
}