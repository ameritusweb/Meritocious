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

        private readonly List<Note> _notes;
        public IReadOnlyCollection<Note> Notes => _notes.AsReadOnly();

        private readonly HashSet<PostRelation> _parentRelations = new();
        private readonly HashSet<PostRelation> _childRelations = new();
        public IReadOnlyCollection<PostRelation> ParentRelations => _parentRelations;
        public IReadOnlyCollection<PostRelation> ChildRelations => _childRelations;

        private readonly Dictionary<string, decimal> _meritComponents = new();
        public IReadOnlyDictionary<string, decimal> MeritComponents => _meritComponents;

        // Engagement metrics (moved from RemixEngagement)
        public int ViewCount { get; private set; }
        public int UniqueViewCount { get; private set; }
        public int LikeCount { get; private set; }
        public int ShareCount { get; private set; }
        public decimal AverageTimeSpentSeconds { get; private set; }

        private Post()
        {
            _comments = new List<Comment>();
            _tags = new List<Tag>();
            _notes = new List<Note>();
        }

        public void AddNote(string type, string content, List<Guid> relatedSourceIds, decimal confidence)
        {
            var note = Note.Create(this, type, content, relatedSourceIds, confidence);
            _notes.Add(note);
            UpdatedAt = DateTime.UtcNow;
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
                author
            );
            
            var relation = PostRelation.CreateFork(this, fork);
            _childRelations.Add(relation);
            fork._parentRelations.Add(relation);
            
            return fork;
        }

        public Post CreateRemix(
            User author,
            string title,
            string content,
            IEnumerable<(Post source, string role, string context)> sources)
        {
            var remix = Create(title, content, author);

            int index = 0;
            foreach (var (source, role, context) in sources)
            {
                var relation = PostRelation.CreateRemixSource(source, remix, role, index++, context);
                source._childRelations.Add(relation);
                remix._parentRelations.Add(relation);
            }

            return remix;
        }

        public void RecordView(bool isUnique, decimal timeSpentSeconds)
        {
            ViewCount++;
            if (isUnique) UniqueViewCount++;

            // Update average time spent
            var oldTotal = AverageTimeSpentSeconds * (ViewCount - 1);
            AverageTimeSpentSeconds = (oldTotal + timeSpentSeconds) / ViewCount;

            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateMeritScore(decimal score, Dictionary<string, decimal> components)
        {
            MeritScore = score;
            _meritComponents.Clear();
            foreach (var (component, value) in components)
            {
                _meritComponents[component] = value;
            }
            UpdatedAt = DateTime.UtcNow;
        }

        public void IncrementLikes()
        {
            LikeCount++;
            UpdatedAt = DateTime.UtcNow;
        }

        public void DecrementLikes()
        {
            if (LikeCount > 0) LikeCount--;
            UpdatedAt = DateTime.UtcNow;
        }

        public void IncrementShares()
        {
            ShareCount++;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Delete()
        {
            IsDeleted = true;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}