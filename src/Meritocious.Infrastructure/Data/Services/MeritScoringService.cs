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

    public class MeritScoringService : IMeritScoringService
    {
        private readonly IMeritScorer _meritScorer;
        private readonly UserRepository _userRepository;
        private readonly PostRepository _postRepository;
        private readonly CommentRepository _commentRepository;
        private readonly ILogger<MeritScoringService> _logger;

        public MeritScoringService(
            IMeritScorer meritScorer,
            UserRepository userRepository,
            PostRepository postRepository,
            CommentRepository commentRepository,
            ILogger<MeritScoringService> logger)
        {
            _meritScorer = meritScorer;
            _userRepository = userRepository;
            _postRepository = postRepository;
            _commentRepository = commentRepository;
            _logger = logger;
        }

        public async Task<MeritScoreDto> CalculateContentScoreAsync(
            string content,
            ContentType type,
            string context = null)
        {
            try
            {
                var score = await _meritScorer.ScoreContentAsync(content, context);

                // Store the score history (implement in future)
                // await _meritScoreRepository.AddScoreHistoryAsync(contentId, type, score);

                return score;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating content score");
                throw;
            }
        }

        public async Task<decimal> CalculateUserMeritScoreAsync(Guid userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new KeyNotFoundException("User not found");

            // Get user's posts and comments
            var posts = await _postRepository.GetPostsByUserAsync(userId);
            var comments = await _commentRepository.GetCommentsByUserAsync(userId);

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
                return 0;

            var averageScore = totalScore / totalItems;

            // Update user's merit score
            await _userRepository.UpdateAsync(user);

            return averageScore;
        }

        public async Task<bool> ValidateContentQualityAsync(string content, ContentType type)
        {
            try
            {
                return await _meritScorer.ValidateContentAsync(content);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating content quality");
                throw;
            }
        }
    }
}