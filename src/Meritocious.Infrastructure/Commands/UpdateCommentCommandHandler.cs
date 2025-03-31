﻿using MediatR;
using Meritocious.Core.Entities;
using Meritocious.Core.Features.Comments.Commands;
using Meritocious.Core.Interfaces;
using Meritocious.Core.Results;
using Meritocious.Infrastructure.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meritocious.Infrastructure.Commands
{
    public class UpdateCommentCommandHandler : IRequestHandler<UpdateCommentCommand, Result<Comment>>
    {
        private readonly ICommentService _commentService;
        private readonly CommentRepository _commentRepository;
        private readonly IMediator _mediator;

        public UpdateCommentCommandHandler(
            ICommentService commentService,
            CommentRepository commentRepository,
            IMediator mediator)
        {
            _commentService = commentService;
            _commentRepository = commentRepository;
            _mediator = mediator;
        }

        public async Task<Result<Comment>> Handle(UpdateCommentCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Verify the comment exists and the editor is the author
                var comment = await _commentRepository.GetByIdAsync(request.CommentId);
                if (comment == null)
                    return Result.Failure<Comment>($"Comment {request.CommentId} not found");

                if (comment.AuthorId != request.EditorId)
                    return Result.Failure<Comment>("Only the author can edit this comment");

                // Update comment
                var updatedComment = await _commentService.UpdateCommentAsync(request.CommentId, request.Content);

                // Publish event (could add a CommentUpdatedEvent)
                // await _mediator.Publish(new CommentUpdatedEvent(comment.Id, request.EditorId), cancellationToken);

                return Result.Success(updatedComment);
            }
            catch (Exception ex)
            {
                return Result.Failure<Comment>($"Error updating comment: {ex.Message}");
            }
        }
    }
}
