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
        private readonly ICommentRepository commentRepository;
        private readonly IPostRepository postRepository;
        private readonly IMeritScorer meritScorer;
        private readonly ILogger<CommentService> logger;

        public CommentService(
            ICommentRepository commentRepository,
            IPostRepository postRepository,
            IMeritScorer meritScorer,
            ILogger<CommentService> logger)
        {
            this.commentRepository = commentRepository;
            this.postRepository = postRepository;
            this.meritScorer = meritScorer;
            this.logger = logger;
        }

        public async Task<Comment> AddCommentAsync(
            string content,
            Guid postId,
            User author,
            Guid? parentCommentId = null)
        {
            var post = await postRepository.GetByIdAsync(postId);
            if (post == null)
            {
                throw new KeyNotFoundException("Post not found");
            }

            Comment parentComment = null;
            if (parentCommentId.HasValue)
            {
                parentComment = await commentRepository.GetByIdAsync(parentCommentId.Value);
                if (parentComment == null)
                {
                    throw new KeyNotFoundException("Parent comment not found");
                }
            }

            // Validate content quality
            var contentScore = await meritScorer.ScoreContentAsync(content);
            if (!await meritScorer.ValidateContentAsync(content))
            {
                throw new InvalidOperationException("Comment does not meet quality standards");
            }

            var comment = Comment.Create(content, post, author, parentComment);
            comment.UpdateMeritScore(contentScore.FinalScore);

            await commentRepository.AddAsync(comment);
            return comment;
        }

        public async Task<Comment> UpdateCommentAsync(Guid commentId, string content)
        {
            var comment = await commentRepository.GetByIdAsync(commentId);
            if (comment == null)
            {
                throw new KeyNotFoundException("Comment not found");
            }

            // Validate content quality
            var contentScore = await meritScorer.ScoreContentAsync(content);
            if (!await meritScorer.ValidateContentAsync(content))
            {
                throw new InvalidOperationException("Comment does not meet quality standards");
            }

            comment.UpdateContent(content);
            comment.UpdateMeritScore(contentScore.FinalScore);

            await commentRepository.UpdateAsync(comment);
            return comment;
        }

        public async Task DeleteCommentAsync(Guid commentId)
        {
            var comment = await commentRepository.GetByIdAsync(commentId);
            if (comment == null)
            {
                throw new KeyNotFoundException("Comment not found");
            }

            comment.Delete();
            await commentRepository.UpdateAsync(comment);
        }

        public async Task<List<Comment>> GetCommentsByPostAsync(Guid postId)
        {
            return await commentRepository.GetCommentsByPostAsync(postId);
        }

        public async Task<List<Comment>> GetCommentsByUserAsync(Guid userId)
        {
            return await commentRepository.GetCommentsByUserAsync(userId);
        }
    }
}