using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meritocious.Core.Services
{
    using Meritocious.Core.Interfaces;
    using Meritocious.Common.DTOs.Merit;
    using Meritocious.Common.Enums;
    using Meritocious.AI.MeritScoring.Interfaces;
    using Meritocious.Infrastructure.Data.Repositories;
    using Microsoft.Extensions.Logging;
    using Meritocious.Core.Entities;

    public class MeritScoringService : IMeritScoringService
    {
        private readonly IMeritScorer meritScorer;
        private readonly IUserRepository userRepository;
        private readonly IPostRepository postRepository;
        private readonly ICommentRepository commentRepository;
        private readonly IMeritScoreHistoryRepository meritScoreHistoryRepository;
        private readonly ILogger<MeritScoringService> logger;

        public MeritScoringService(
            IMeritScorer meritScorer,
            IUserRepository userRepository,
            IPostRepository postRepository,
            ICommentRepository commentRepository,
            IMeritScoreHistoryRepository meritScoreHistoryRepository,
            ILogger<MeritScoringService> logger)
        {
            this.meritScorer = meritScorer;
            this.userRepository = userRepository;
            this.postRepository = postRepository;
            this.commentRepository = commentRepository;
            this.meritScoreHistoryRepository = meritScoreHistoryRepository;
            this.logger = logger;
        }

        public async Task<MeritScoreDto> CalculateContentScoreAsync(
            string content,
            ContentType type,
            string context = null,
            Guid? contentId = null,
            bool isRecalculation = false,
            string recalculationReason = null)
        {
            try
            {
                var score = await meritScorer.ScoreContentAsync(content, context);

                // Store score history if contentId is provided
                if (contentId.HasValue)
                {
                    var history = MeritScoreHistory.Create(
                        contentId.Value,
                        type,
                        score.FinalScore,
                        score.Components,
                        score.ModelVersion,
                        score.Explanations,
                        context,
                        isRecalculation,
                        recalculationReason);

                    await meritScoreHistoryRepository.AddAsync(history);
                }

                return score;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error calculating content score");
                throw;
            }
        }

        public async Task<decimal> CalculateUserMeritScoreAsync(Guid userId)
        {
            var user = await userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                throw new KeyNotFoundException("User not found");
            }

            // Get user's posts and comments
            var posts = await postRepository.GetPostsByUserAsync(userId);
            var comments = await commentRepository.GetCommentsByUserAsync(userId);

            // Calculate weighted average of content scores
            decimal totalScore = 0;
            int totalItems = 0;

            if (posts.Any())
            {
                totalScore += posts.Sum(p => p.MeritScore);
                totalItems += posts.Count;
            }

            if (comments.Any())
            {
                totalScore += comments.Sum(c => c.MeritScore);
                totalItems += comments.Count;
            }

            if (totalItems == 0)
            {
                return 0;
            }

            var averageScore = totalScore / totalItems;

            // Update user's merit score
            await userRepository.UpdateAsync(user);

            return averageScore;
        }

        public async Task<bool> ValidateContentQualityAsync(string content, ContentType type)
        {
            try
            {
                return await meritScorer.ValidateContentAsync(content);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error validating content quality");
                throw;
            }
        }

        public async Task<List<MeritScoreDto>> GetContentScoreHistoryAsync(Guid contentId, ContentType type)
        {
            try
            {
                var history = await meritScoreHistoryRepository.GetContentScoreHistoryAsync(contentId, type);
                
                return history.Select(h => new MeritScoreDto
                {
                    FinalScore = h.Score,
                    Components = h.Components,
                    ModelVersion = h.ModelVersion,
                    Explanations = h.Explanations,
                    Timestamp = h.EvaluatedAt,
                    ContentId = h.ContentId,
                    ContentType = h.ContentType,
                    Context = h.Context,
                    IsRecalculation = h.IsRecalculation,
                    RecalculationReason = h.RecalculationReason
                }).ToList();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error retrieving content score history");
                throw;
            }
        }

        public async Task<MeritScoreDto> CalculateContentScoreAsync(
        string content,
        ContentType type,
        string context = null)
        {
            var score = await meritScorer.ScoreContentAsync(content, context);

            return new MeritScoreDto
            {
                ContentType = type,
                Context = context,
                Timestamp = DateTime.UtcNow,
                FinalScore = score.FinalScore,
                Components = score.Components,
                ModelVersion = score.ModelVersion,
                Explanations = score.Explanations
            };
        }
    }
}