using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meritocious.Core.Features.Users.Queries
{
    using MediatR;
    using Meritocious.Common.DTOs.Auth;
    using Meritocious.Core.Results;

    public record GetUserProfileQuery : IRequest<Result<UserProfileDto>>
    {
        public string UserId { get; init; }
    }
}