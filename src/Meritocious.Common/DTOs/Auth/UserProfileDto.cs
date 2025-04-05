using Meritocious.Common.DTOs.Contributions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meritocious.Common.DTOs.Auth
{
    public class UserProfileDto
    {
        public string Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string AvatarUrl { get; set; }
        public string DisplayName { get; set; }
        public string Bio { get; set; }
        public decimal MeritScore { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastLoginAt { get; set; }
        public List<ContributionSummaryDto> TopContributions { get; set; } = new();
    }
}
