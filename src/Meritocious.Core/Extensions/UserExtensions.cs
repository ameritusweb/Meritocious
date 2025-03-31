using Meritocious.Common.DTOs.Auth;
using Meritocious.Common.DTOs.Contributions;
using Meritocious.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meritocious.Core.Extensions
{
    public static class UserExtensions
    {
        public static UserProfileDto ToDto(this User user)
        {
            return new UserProfileDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                MeritScore = user.MeritScore,
                CreatedAt = user.CreatedAt,
                LastLoginAt = user.LastLoginAt,
                TopContributions = new List<ContributionSummaryDto>()
            };
        }

        public static List<UserProfileDto> ToDtoList(this IEnumerable<User> users)
        {
            return users.Select(u => u.ToDto()).ToList();
        }
    }
}
