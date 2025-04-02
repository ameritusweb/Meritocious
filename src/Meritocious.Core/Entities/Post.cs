using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Meritocious.Core.Entities
{
    public class Post : BaseEntity
    {
        public string Title { get; private set; }
        public string Content { get; private set; }
        public Guid AuthorId { get; private set; }
        public User Author { get; private set; }
        public Guid? ParentPostId { get; private set; }
        public Post ParentPost { get; private set; }
        public decimal MeritScore { get; private set; }
        public bool IsDeleted { get; private set; }
        public string SubstackId { get; private set; }
        public Substack Substack { get; private set; }

        private readonly List<Comment> _comments;
        public IReadOnlyCollection<Comment> Comments => _comments.AsReadOnly();

        private readonly List<Tag> _tags;
        public IReadOnlyCollection<Tag> Tags => _tags.AsReadOnly();

        private readonly List<Post> _forks;
        public IReadOnlyCollection<Post> Forks => _forks.AsReadOnly();

        private Post()
        {
            _comments = new List<Comment>();
            _tags = new List<Tag>();
            _forks = new List<Post>();
        }

        public static Post Create(string title, string content, User author, Post parent = null, Substack substack = null)
        {
            return new Post
            {
                Title = title,
                Content = content,
                AuthorId = author.Id,
                Author = author,
                ParentPostId = parent?.Id,
                ParentPost = parent,
                SubstackId = substack?.Id,
                Substack = substack,
                MeritScore = 0,
                IsDeleted = false,
                CreatedAt = DateTime.UtcNow
            };
        }

        public void UpdateContent(string newTitle, string newContent)
        {
            Title = newTitle;
            Content = newContent;
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateMeritScore(decimal newScore)
        {
            MeritScore = newScore;
            UpdatedAt = DateTime.UtcNow;
        }

        public void AddTag(Tag tag)
        {
            if (!_tags.Contains(tag))
            {
                _tags.Add(tag);
            }
        }

        public Post CreateFork(User author, string newTitle = null)
        {
            var fork = Create(
                newTitle ?? $"Fork: {Title}",
                Content,
                author,
                this
            );
            _forks.Add(fork);
            return fork;
        }

        public void Delete()
        {
            IsDeleted = true;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}