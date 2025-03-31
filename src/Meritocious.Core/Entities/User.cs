using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Meritocious.Core.Entities
{
    public class User : BaseEntity
    {
        public string Username { get; private set; }
        public string Email { get; private set; }
        public string PasswordHash { get; private set; }
        public decimal MeritScore { get; private set; }
        public DateTime? LastLoginAt { get; private set; }
        public bool IsActive { get; private set; }

        private readonly List<Post> _posts;
        public IReadOnlyCollection<Post> Posts => _posts.AsReadOnly();

        private readonly List<Comment> _comments;
        public IReadOnlyCollection<Comment> Comments => _comments.AsReadOnly();

        private User()
        {
            _posts = new List<Post>();
            _comments = new List<Comment>();
        }

        public static User Create(string username, string email, string passwordHash)
        {
            return new User
            {
                Username = username,
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