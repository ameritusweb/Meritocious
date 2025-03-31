using MediatR;
using Meritocious.Core.Entities;
using Meritocious.Core.Features.Comments.Queries;
using Meritocious.Core.Results;
using Meritocious.Infrastructure.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meritocious.Infrastructure.Queries
{
    public class GetCommentRepliesQueryHandler : IRequestHandler<GetCommentRepliesQuery, Result<List<Comment>>>
    {
        private readonly CommentRepository _commentRepository;

        public GetCommentRepliesQueryHandler(CommentRepository commentRepository)
        {
            _commentRepository = commentRepository;
        }

        public async Task<Result<List<Comment>>> Handle(GetCommentRepliesQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Verify the parent comment exists
                var parentComment = await _commentRepository.GetByIdAsync(request.CommentId);
                if (parentComment == null)
                    return Result.Failure<List<Comment>>($"Comment {request.CommentId} not found");

                // Get replies
                var replies = await _commentRepository.GetRepliesAsync(request.CommentId);

                // Apply sorting if needed
                if (request.SortBy == "date")
                {
                    replies = replies.OrderByDescending(r => r.CreatedAt).ToList();
                }
                // Default sorting by merit is already applied in the repository

                // Apply pagination if needed
                if (request.Page.HasValue && request.PageSize.HasValue)
                {
                    int skip = (request.Page.Value - 1) * request.PageSize.Value;
                    replies = replies.Skip(skip).Take(request.PageSize.Value).ToList();
                }

                return Result.Success(replies);
            }
            catch (Exception ex)
            {
                return Result.Failure<List<Comment>>($"Error retrieving comment replies: {ex.Message}");
            }
        }
    }
}
