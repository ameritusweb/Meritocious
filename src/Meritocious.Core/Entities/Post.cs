﻿using Meritocious.Common.DTOs.Merit;
using Meritocious.Common.Enums;
using Meritocious.Core.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Meritocious.Core.Entities
{
    public partial class Post : BaseEntity<Post>
    {
        public string Title { get; internal set; }
        public string Content { get; internal set; }
        [ForeignKey("FK_AuthorId")]
        public UlidId<User> AuthorId { get; internal set; }
        public User Author { get; internal set; }
        [ForeignKey("FK_ParentPostId")]
        public UlidId<Post>? ParentPostId { get; internal set; }
        public Post? ParentPost { get; internal set; }
        public bool IsDeleted { get; internal set; }
        public bool IsDraft { get; internal set; }
        [ForeignKey("FK_SubstackId")]
        public UlidId<Substack> SubstackId { get; internal set; }
        public Substack Substack { get; internal set; }
        public decimal EngagementScore { get; internal set; }

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
        private readonly List<MeritScoreHistory> meritScoreHistories;
        public IReadOnlyCollection<MeritScoreHistory> MeritScoreHistories => meritScoreHistories.AsReadOnly();
        public decimal AverageTimeSpentSeconds { get; internal set; }

        public DateTime? PublishedAt { get; internal set; }
        public string SynthesisMap { get; internal set; }
        private readonly List<ContentVersion> versions = new();
        public IReadOnlyCollection<ContentVersion> Versions => versions.AsReadOnly();

        public void UpdateSynthesisMap(string synthesisMap)
        {
            SynthesisMap = synthesisMap;
            UpdatedAt = DateTime.UtcNow;
        }

        public void AddRelation(PostRelation relation)
        {
            // TODO: Add relation
            // if (relation.RelationType == "remix")
            // {
            //    ParentRelations.Add(relation);
            // }
            // else if (relation.RelationType == "fork")
            // {
            //    ChildRelations.Add(relation);
            // }
            UpdatedAt = DateTime.UtcNow;
        }

        private readonly List<MeritScore> meritScores = new();
        private decimal meritScore;
        public IReadOnlyCollection<MeritScore> MeritScores => meritScores.AsReadOnly();

        // Computed total merit score
        public decimal MeritScore => meritScores.CalculateTotalMeritScore();

        public MeritScoreHistory UpdateMeritScore(MeritScoreDto scoreDto, Func<string, MeritScoreType> meritScoreTypes)
        {
            // Update final score and components
            meritScore = scoreDto.FinalScore;
            meritComponents.Clear();
            foreach (var component in scoreDto.Components)
            {
                meritComponents[component.Key] = component.Value;
            }

            // Convert component scores to MeritScore entities
            foreach (var component in scoreDto.Components)
            {
                var scoreType = meritScoreTypes(component.Key);
                if (scoreType != null)
                {
                    AddMeritScore(scoreType, component.Value);
                }
            }

            // Create history entry
            var history = MeritScoreHistory.Create(
                Id,
                ContentType.Post,
                scoreDto.FinalScore,
                new Dictionary<string, decimal>(scoreDto.Components),
                scoreDto.ModelVersion,
                scoreDto.Explanations,
                scoreDto.Context,
                scoreDto.IsRecalculation,
                scoreDto.RecalculationReason);

            UpdatedAt = DateTime.UtcNow;

            return history;
        }

        public void AddMeritScore(MeritScoreType scoreType, decimal score)
        {
            var existingScore = meritScores.FirstOrDefault(s => s.ScoreTypeId == scoreType.Id);
            if (existingScore != null)
            {
                existingScore.UpdateScore(score);
            }
            else
            {
                // TODO: Add a merit score
                // meritScores.Add(MeritScore.Create(Id, "Post", scoreType, score));
            }

            // Also update components dictionary for consistency
            meritComponents[scoreType.Name] = score;

            // Recalculate final merit score based on weighted components
            RecalculateFinalScore();

            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateMeritScores(Dictionary<string, decimal> scores)
        {
            foreach (var (scoreTypeId, score) in scores)
            {
                var existingScore = meritScores.FirstOrDefault(s => s.ScoreTypeId == scoreTypeId);
                if (existingScore != null)
                {
                    existingScore.UpdateScore(score);

                    // Update components dictionary
                    var scoreType = existingScore.ScoreType;
                    if (scoreType != null)
                    {
                        meritComponents[scoreType.Name] = score;
                    }
                }
            }

            // Recalculate final merit score
            RecalculateFinalScore();

            UpdatedAt = DateTime.UtcNow;
        }

        private void RecalculateFinalScore()
        {
            // Calculate weighted average based on score types
            if (meritScores.Any())
            {
                meritScore = meritScores
                    .Where(s => s.ScoreType?.IsActive == true)
                    .Sum(s => s.Score * (s.ScoreType?.Weight ?? 0));
            }
        }

        // Additional helper methods
        public decimal GetComponentScore(string component)
        {
            return meritComponents.TryGetValue(component, out var score) ? score : 0m;
        }

        public MeritScore GetMeritScore(string scoreTypeId)
        {
            return meritScores.FirstOrDefault(s => s.ScoreTypeId == scoreTypeId);
        }

        public bool HasMeritScore(string scoreTypeId)
        {
            return meritScores.Any(s => s.ScoreTypeId == scoreTypeId);
        }

        internal Post()
        {
            comments = new List<Comment>();
            tags = new List<Tag>();
            notes = new List<Note>();
        }

        public void AddNote(string type, string content, List<string> relatedSourceIds, decimal confidence)
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
                AuthorId = author.Id,
                Author = author,
                ParentPostId = parent?.Id,
                ParentPost = parent,
                SubstackId = substack?.Id ?? string.Empty,
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
            Engagement.Views++;
            if (isUnique)
            {
                Engagement.UniqueViews++;
            }

            // Update average time spent
            var oldTotal = AverageTimeSpentSeconds * (ViewCount - 1);
            AverageTimeSpentSeconds = (oldTotal + timeSpentSeconds) / ViewCount;

            UpdatedAt = DateTime.UtcNow;
        }

        public void IncrementLikes()
        {
            Engagement.Likes++;
            UpdatedAt = DateTime.UtcNow;
        }

        public void DecrementLikes()
        {
            if (LikeCount > 0)
            {
                Engagement.Likes--;
            }

            UpdatedAt = DateTime.UtcNow;
        }

        public void IncrementShares()
        {
            Engagement.Shares++;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Delete()
        {
            IsDeleted = true;
            UpdatedAt = DateTime.UtcNow;
        }
    }

    public class PostTag : BaseEntity<PostTag>
    {
        [ForeignKey("PostId")]
        public UlidId<Post> PostId { get; set; }
        public Post Post { get; set; }

        [ForeignKey("TagId")]
        public UlidId<Tag> TagId { get; set; }
        public Tag Tag { get; set; }
    }

    public static class PostExtensions
    {
        public static string GetPostType(this Post post)
        {
            // Determine type based on relationships
            if (post.ParentRelations.Any(r => r.RelationType == "remix"))
            {
                return "remix";
            }

            if (post.ParentRelations.Any(r => r.RelationType == "fork"))
            {
                return "fork";
            }

            // Check if this post has been remixed or forked
            if (post.ChildRelations.Any(r => r.RelationType == "remix" || r.RelationType == "fork"))
            {
                return "source";
            }

            return "original";
        }
    }
}