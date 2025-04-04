using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.AspNetCore.Identity;

namespace Meritocious.Core.Entities
{
    public class User : IdentityUser
    {
        public decimal MeritScore { get; private set; }
        public DateTime? LastLoginAt { get; private set; }
        public bool IsActive { get; private set; }
        public DateTime CreatedAt { get; private set; }

        private readonly List<Post> _posts;
        public IReadOnlyCollection<Post> Posts => _posts.AsReadOnly();

        private readonly List<Comment> _comments;
        public IReadOnlyCollection<Comment> Comments => _comments.AsReadOnly();

        private readonly List<Substack> _followedSubstacks;
        public IReadOnlyCollection<Substack> FollowedSubstacks => _followedSubstacks.AsReadOnly();

        private User()
        {
            _posts = new List<Post>();
            _comments = new List<Comment>();
            _followedSubstacks = new List<Substack>();
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