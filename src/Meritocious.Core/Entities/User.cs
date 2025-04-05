using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Meritocious.Core.Entities
{
    public class User : IdentityUser
    {
        public decimal MeritScore { get; private set; }
        public DateTime? LastCalculated { get; private set; }
        public DateTime? LastLoginAt { get; private set; }
        public bool IsActive { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime UpdatedAt { get; private set; }
        public string AvatarUrl { get; set; }
        public string DisplayName { get; set; }
        public string Bio { get; set; }
        public bool EmailNotificationsEnabled { get; set; }
        public bool PublicProfile { get; set; }
        public List<string> PreferredTags { get; set; } = new();
        public string TimeZone { get; set; }
        public string Language { get; set; }
        public bool IsLocked { get; set; }
        public bool IsEmailVerified { get; set; }
        public DateTime? LastActivityAt { get; set; }
        public DateTime? LastActiveAt { get; set; }
        public bool TwoFactorRequired { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public virtual ICollection<Tag> FollowedTags { get; set; } = new List<Tag>();

        private readonly List<Post> posts;
        public IReadOnlyCollection<Post> Posts => posts.AsReadOnly();

        private readonly List<Comment> comments;
        public IReadOnlyCollection<Comment> Comments => comments.AsReadOnly();
        private readonly List<MeritScoreHistory> meritScoreHistories;
        public IReadOnlyCollection<MeritScoreHistory> MeritScoreHistories  => meritScoreHistories.AsReadOnly();

        private readonly List<Substack> followedSubstacks = new();
        public IReadOnlyCollection<Substack> FollowedSubstacks => followedSubstacks.AsReadOnly();

        public void FollowSubstack(Substack substack)
        {
            if (!followedSubstacks.Contains(substack))
            {
                followedSubstacks.Add(substack);
            }
        }

        public bool UnfollowSubstack(Substack substack)
        {
            return followedSubstacks.Remove(substack);
        }

        private User()
        {
            posts = new List<Post>();
            comments = new List<Comment>();
            followedSubstacks = new List<Substack>();
        }

        public static User Create(string username, string email, string passwordHash)
        {
            return new User
            {
                UserName = username,
                Email = email,
                PasswordHash = passwordHash,
                MeritScore = 0,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
        }

        public void UpdateMeritScore(decimal newScore)
        {
            MeritScore = newScore;
            UpdatedAt = DateTime.UtcNow;
        }

        public void RecordLogin()
        {
            LastLoginAt = DateTime.UtcNow;
        }
    }
}