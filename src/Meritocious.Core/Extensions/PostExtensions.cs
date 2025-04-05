using Meritocious.Common.DTOs.Content;
using Meritocious.Common.DTOs.Merit;
using Meritocious.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meritocious.Core.Extensions
{
    public static class PostExtensions
    {
        public static PostDto ToDto(this Post post)
        {
            return new PostDto
            {
                Id = post.Id,
                Title = post.Title,
                Content = post.Content,
                AuthorId = post.AuthorId,
                AuthorUsername = post.Author?.UserName ?? "Unknown",
                CreatedAt = post.CreatedAt,
                UpdatedAt = post.UpdatedAt,
                ParentPostId = post.ParentPostId,
                MeritScore = post.MeritScore,
                Tags = post.Tags?.Select(t => t.Name).ToList() ?? new List<string>()
            };
        }

        public static MeritScoreDto ToMeritScoreDto(this Post post)
        {
            return new MeritScoreDto
            {
                UserId = post.AuthorId,
                CurrentScore = post.MeritScore,
                LastCalculated = post.UpdatedAt ?? DateTime.UtcNow,
                ScoreHistory = post.MeritScoreHistories.Select(h => h.ToDto()).ToList()
            };
        }

        public static List<PostDto> ToDtoList(this IEnumerable<Post> posts)
        {
            return posts.Select(p => p.ToDto()).ToList();
        }
    }
}
