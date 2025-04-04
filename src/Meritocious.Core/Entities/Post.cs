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
        public bool IsDeleted { get; private set; }
        public string SubstackId { get; private set; }
        public Substack Substack { get; private set; }

        private readonly List<Comment> comments;
        public IReadOnlyCollection<Comment> Comments => comments.AsReadOnly();

        private readonly List<Tag> tags;
        public IReadOnlyCollection<Tag> Tags => tags.AsReadOnly();

        private readonly List<Note> notes;
        public IReadOnlyCollection<Note> Notes => notes.AsReadOnly();

        private readonly HashSet<PostRelation> parentRelations = new();
        private readonly HashSet<PostRelation> childRelations = new();
        public IReadOnlyCollection<PostRelation> ParentRelations => parentRelations;
        public IReadOnlyCollection<PostRelation> ChildRelations => childRelations;

        private readonly Dictionary<string, decimal> meritComponents = new();
        public IReadOnlyDictionary<string, decimal> MeritComponents => meritComponents;

        // Engagement metrics (moved from RemixEngagement)
        public int ViewCount { get; private set; }
        public int UniqueViewCount { get; private set; }
        public int LikeCount { get; private set; }
        public int ShareCount { get; private set; }
        public decimal AverageTimeSpentSeconds { get; private set; }

        private readonly List<MeritScore> meritScores = new();
        public IReadOnlyCollection<MeritScore> MeritScores => meritScores.AsReadOnly();

        // Computed total merit score
        public decimal MeritScore => meritScores.CalculateTotalMeritScore();

        public void AddMeritScore(MeritScoreType scoreType, decimal score)
        {
            var existingScore = meritScores.FirstOrDefault(s => s.ScoreTypeId == scoreType.Id);
            if (existingScore != null)
            {
                existingScore.UpdateScore(score);
            }
            else
            {
                meritScores.Add(Meritocious.Core.Entities.MeritScore.Create(Id, "Post", scoreType, score));
            }

            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateMeritScores(Dictionary<Guid, decimal> scores)
        {
            foreach (var (scoreTypeId, score) in scores)
            {
                var existingScore = meritScores.FirstOrDefault(s => s.ScoreTypeId == scoreTypeId);
                if (existingScore != null)
                {
                    existingScore.UpdateScore(score);
                }
            }

            UpdatedAt = DateTime.UtcNow;
        }

        private Post()
        {
            comments = new List<Comment>();
            tags = new List<Tag>();
            notes = new List<Note>();
        }

        public void AddNote(string type, string content, List<Guid> relatedSourceIds, decimal confidence)
        {
            var note = Note.Create(this, type, content, relatedSourceIds, confidence);
            notes.Add(note);
            UpdatedAt = DateTime.UtcNow;
        }

        public static Post Create(string title, string content, User author, Post parent = null, Substack substack = null)
        {
            return new Post
            {
                Title = title,
                Content = content,
                AuthorId = Guid.Parse(author?.Id),
                Author = author,
                ParentPostId = parent?.Id,
                ParentPost = parent,
                SubstackId = substack?.Id.ToString(),
                Substack = substack,
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

        public void AddTag(Tag tag)
        {
            if (!tags.Contains(tag))
            {
                tags.Add(tag);
            }
        }

        public Post CreateFork(User author, string newTitle = null)
        {
            var fork = Create(
                newTitle ?? $"Fork: {Title}",
                Content,
                author);
            
            var relation = PostRelation.CreateFork(this, fork);
            childRelations.Add(relation);
            fork.parentRelations.Add(relation);
            
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
                source.childRelations.Add(relation);
                remix.parentRelations.Add(relation);
            }

            return remix;
        }

        public void RecordView(bool isUnique, decimal timeSpentSeconds)
        {
            ViewCount++;
            if (isUnique)
            {
                UniqueViewCount++;
            }

            // Update average time spent
            var oldTotal = AverageTimeSpentSeconds * (ViewCount - 1);
            AverageTimeSpentSeconds = (oldTotal + timeSpentSeconds) / ViewCount;

            UpdatedAt = DateTime.UtcNow;
        }

        public void IncrementLikes()
        {
            LikeCount++;
            UpdatedAt = DateTime.UtcNow;
        }

        public void DecrementLikes()
        {
            if (LikeCount > 0)
            {
                LikeCount--;
            }

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