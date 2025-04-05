using MediatR;
using Meritocious.Common.DTOs.Content;
using Meritocious.Core.Entities;
using Meritocious.Core.Extensions;
using Meritocious.Core.Features.Comments.Commands;
using Meritocious.Core.Interfaces;
using Meritocious.Core.Results;
using Meritocious.Infrastructure.Data.Repositories;

namespace Meritocious.Infrastructure.Commands
{
    public class UpdateCommentCommandHandler : IRequestHandler<UpdateCommentCommand, Result<CommentDto>>
    {
        private readonly ICommentService commentService;
        private readonly CommentRepository commentRepository;
        private readonly IMediator mediator;

        public UpdateCommentCommandHandler(
            ICommentService commentService,
            CommentRepository commentRepository,
            IMediator mediator)
        {
            this.commentService = commentService;
            this.commentRepository = commentRepository;
            this.mediator = mediator;
        }

        public async Task<Result<CommentDto>> Handle(UpdateCommentCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Verify the comment exists and the editor is the author
                var comment = await commentRepository.GetByIdAsync(request.CommentId);
                if (comment == null)
                {
                    return Result.Failure<CommentDto>($"Comment {request.CommentId} not found");
                }

                if (comment.AuthorId != request.EditorId.ToString())
                {
                    return Result.Failure<CommentDto>("Only the author can edit this comment");
                }

                // Update comment
                var updatedComment = await commentService.UpdateCommentAsync(request.CommentId, request.Content);

                // Publish event (could add a CommentUpdatedEvent)
                // await _mediator.Publish(new CommentUpdatedEvent(comment.Id, request.EditorId), cancellationToken);
                return Result.Success(updatedComment.ToDto());
            }
            catch (Exception ex)
            {
                return Result.Failure<CommentDto>($"Error updating comment: {ex.Message}");
            }
        }
    }
}
