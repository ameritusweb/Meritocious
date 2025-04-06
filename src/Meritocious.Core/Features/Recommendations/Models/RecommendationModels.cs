using System;
using System.ComponentModel.DataAnnotations.Schema;
using Meritocious.Common.Enums;
using Meritocious.Core.Entities;
using Meritocious.Core.Extensions;

namespace Meritocious.Core.Features.Recommendations.Models
{
    public class UserContentInteraction : BaseEntity<UserContentInteraction>
    {
        [ForeignKey("FK_UserId")]
        public UlidId<User> UserId { get; private set; }
        public User User { get; private set; }
        public string ContentId { get; private set; }
        public ContentType ContentType { get; private set; }
        public string InteractionType { get; private set; }
        public decimal EngagementScore { get; private set; }
        public DateTime InteractedAt { get; private set; }

        private UserContentInteraction()
        {
        }

        public static UserContentInteraction Create(
            User user,
            string contentId,
            ContentType contentType,
            string interactionType,
            decimal engagementScore)
        {
            return new UserContentInteraction
            {
                UserId = user.Id,
                User = user,
                ContentId = contentId,
                ContentType = contentType,
                InteractionType = interactionType,
                EngagementScore = engagementScore,
                InteractedAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            };
        }

        public void UpdateEngagement(decimal newScore)
        {
            EngagementScore = newScore;
            UpdatedAt = DateTime.UtcNow;
        }
    }

    public class ContentTopic : BaseEntity<ContentTopic>
    {
        public string ContentId { get; private set; }
        public ContentType ContentType { get; private set; }
        public string Topic { get; private set; }
        public decimal Relevance { get; private set; }
        public DateTime ExtractedAt { get; private set; }

        private ContentTopic()
        {
        }

        public static ContentTopic Create(
            string contentId,
            ContentType contentType,
            string topic,
            decimal relevance)
        {
            return new ContentTopic
            {
                ContentId = contentId,
                ContentType = contentType,
                Topic = topic,
                Relevance = relevance,
                ExtractedAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            };
        }

        public virtual ICollection<Substack> Substacks { get; set; } = new List<Substack>();
    }

    public class UserTopicPreference : BaseEntity<UserTopicPreference>
    {
        [ForeignKey("FK_UserId")]
        public UlidId<User> UserId { get; private set; }
        public User User { get; private set; }
        public string Topic { get; private set; }
        public decimal Weight { get; private set; }
        public DateTime LastUpdated { get; private set; }

        private UserTopicPreference()
        {
        }

        public static UserTopicPreference Create(
            User user,
            string topic,
            decimal weight)
        {
            return new UserTopicPreference
            {
                UserId = user.Id,
                User = user,
                Topic = topic,
                Weight = weight,
                LastUpdated = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            };
        }

        public void UpdateWeight(decimal newWeight)
        {
            Weight = newWeight;
            LastUpdated = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }
    }

    public class TrendingContent : BaseEntity<TrendingContent>
    {
        public string ContentId { get; private set; }
        public ContentType ContentType { get; private set; }
        public decimal TrendingScore { get; private set; }
        public int ViewCount { get; private set; }
        public int InteractionCount { get; private set; }
        public decimal AverageMeritScore { get; private set; }
        public DateTime WindowStart { get; private set; }
        public DateTime WindowEnd { get; private set; }

        private TrendingContent()
        {
        }

        public static TrendingContent Create(
            string contentId,
            ContentType contentType,
            TimeSpan windowSize)
        {
            var now = DateTime.UtcNow;
            return new TrendingContent
            {
                ContentId = contentId,
                ContentType = contentType,
                TrendingScore = 0,
                ViewCount = 0,
                InteractionCount = 0,
                AverageMeritScore = 0,
                WindowStart = now.Subtract(windowSize),
                WindowEnd = now,
                CreatedAt = now
            };
        }

        public void UpdateMetrics(
            int views,
            int interactions,
            decimal meritScore)
        {
            ViewCount = views;
            InteractionCount = interactions;
            AverageMeritScore = meritScore;

            // Calculate trending score using a weighted formula
            TrendingScore = (ViewCount * 0.3m +
                           InteractionCount * 0.4m +
                           AverageMeritScore * 0.3m) /
                           Math.Max(1, (DateTime.UtcNow - WindowStart).Days);

            UpdatedAt = DateTime.UtcNow;
        }
    }
}