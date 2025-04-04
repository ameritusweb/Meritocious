using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meritocious.Core.Entities
{
    public class Comment : BaseEntity
    {
        public string Content { get; private set; }
        public Guid PostId { get; private set; }
        public Post Post { get; private set; }
        public Guid AuthorId { get; private set; }
        public User Author { get; private set; }
        public Guid? ParentCommentId { get; private set; }
        public Comment ParentComment { get; private set; }
        public decimal MeritScore { get; private set; }
        public bool IsDeleted { get; private set; }

        private readonly List<Comment> replies;
        public IReadOnlyCollection<Comment> Replies => replies.AsReadOnly();

        private Comment()
        {
            replies = new List<Comment>();
        }

        public static Comment Create(string content, Post post, User author, Comment parent = null)
        {
            return new Comment
            {
                Content = content,
                PostId = post.Id,
                Post = post,
                AuthorId = Guid.Parse(author.Id),
                Author = author,
                ParentCommentId = parent?.Id,
                ParentComment = parent,
                MeritScore = 0,
                IsDeleted = false,
                CreatedAt = DateTime.UtcNow
            };
        }

        public void UpdateContent(string newContent)
        {
            Content = newContent;
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateMeritScore(decimal newScore)
        {
            MeritScore = newScore;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Delete()
        {
            IsDeleted = true;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}