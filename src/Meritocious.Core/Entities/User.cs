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
        public DateTime? LastLoginAt { get; private set; }
        public bool IsActive { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime UpdatedAt { get; private set; }
        public string AvatarUrl { get; set; }

        private readonly List<Post> posts;
        public IReadOnlyCollection<Post> Posts => posts.AsReadOnly();

        private readonly List<Comment> comments;
        public IReadOnlyCollection<Comment> Comments => comments.AsReadOnly();

        private readonly List<Substack> followedSubstacks;
        public IReadOnlyCollection<Substack> FollowedSubstacks => followedSubstacks.AsReadOnly();

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