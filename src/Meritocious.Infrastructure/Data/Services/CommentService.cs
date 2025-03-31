using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meritocious.Core.Services
{
    using Meritocious.Core.Entities;
    using Meritocious.Core.Interfaces;
    using Meritocious.Infrastructure.Data.Repositories;
    using Meritocious.AI.MeritScoring.Interfaces;
    using Microsoft.Extensions.Logging;

    public class CommentService : ICommentService
    {
        private readonly CommentRepository _commentRepository;
        private readonly PostRepository _postRepository;
        private readonly IMeritScorer _meritScorer;
        private readonly ILogger<CommentService> _logger;

        public CommentService(
            CommentRepository commentRepository,
            PostRepository postRepository,
            IMeritScorer meritScorer,
            ILogger<CommentService> logger)
        {
            _commentRepository = commentRepository;
            _postRepository = postRepository;
            _meritScorer = meritScorer;
            _logger = logger;
        }

        public async Task<Comment> AddCommentAsync(
            string content,
            Guid postId,
            User author,
            Guid? parentCommentId = null)
        {
            var post = await _postRepository.GetByIdAsync(postId);
            if (post == null)
                throw new KeyNotFoundException("Post not found");

            Comment parentComment = null;
            if (parentCommentId.HasValue)
            {
                parentComment = await _commentRepository.GetByIdAsync(parentCommentId.Value);
                if (parentComment == null)
                    throw new KeyNotFoundException("Parent comment not found");
            }

            // Validate content quality
            var contentScore = await _meritScorer.ScoreContentAsync(content);
            if (!await _meritScorer.ValidateContentAsync(content))
            {
                throw new InvalidOperationException("Comment does not meet quality standards");
            }

            var comment = Comment.Create(content, post, author, parentComment);
            comment.UpdateMeritScore(contentScore.FinalScore);

            await _commentRepository.AddAsync(comment);
            return comment;
        }

        public async Task<Comment> UpdateCommentAsync(Guid commentId, string content)
        {
            var comment = await _commentRepository.GetByIdAsync(commentId);
            if (comment == null)
                throw new KeyNotFoundException("Comment not found");

            // Validate content quality
            var contentScore = await _meritScorer.ScoreContentAsync(content);
            if (!await _meritScorer.ValidateContentAsync(content))
            {
                throw new InvalidOperationException("Comment does not meet quality standards");
            }

            comment.UpdateContent(content);
            comment.UpdateMeritScore(contentScore.FinalScore);

            await _commentRepository.UpdateAsync(comment);
            return comment;
        }

        public async Task DeleteCommentAsync(Guid commentId)
        {
            var comment = await _commentRepository.GetByIdAsync(commentId);
            if (comment == null)
                throw new KeyNotFoundException("Comment not found");

            comment.Delete();
            await _commentRepository.UpdateAsync(comment);
        }

        public async Task<List<Comment>> GetCommentsByPostAsync(Guid postId)
        {
            return await _commentRepository.GetCommentsByPostAsync(postId);
        }

        public async Task<List<Comment>> GetCommentsByUserAsync(Guid userId)
        {
            return await _commentRepository.GetCommentsByUserAsync(userId);
        }
    }
}