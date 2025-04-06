using Meritocious.Core.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meritocious.Core.Entities
{
    public class Comment : BaseEntity<Comment>
    {
        public string Content { get; private set; }
        [ForeignKey("FK_PostId")]
        public UlidId<Post> PostId { get; private set; }
        public Post Post { get; private set; }
        public string AuthorId { get; private set; }
        public User Author { get; private set; }
        [ForeignKey("FK_ParentCommentId")]
        public UlidId<Comment>? ParentCommentId { get; private set; }
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
                AuthorId = author.Id,
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