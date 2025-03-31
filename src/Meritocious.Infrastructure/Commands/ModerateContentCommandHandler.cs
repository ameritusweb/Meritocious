using MediatR;
using Meritocious.AI.Moderation.Interfaces;
using Meritocious.Common.DTOs.Moderation;
using Meritocious.Common.Enums;
using Meritocious.Core.Features.Moderation.Commands;
using Meritocious.Core.Features.Moderation.Events;
using Meritocious.Core.Results;
using Meritocious.Infrastructure.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meritocious.Infrastructure.Commands
{
    public class ModerateContentCommandHandler : IRequestHandler<ModerateContentCommand, Result<ModerationResult>>
    {
        private readonly IContentModerator _contentModerator;
        private readonly PostRepository _postRepository;
        private readonly CommentRepository _commentRepository;
        private readonly IMediator _mediator;

        public ModerateContentCommandHandler(
            IContentModerator contentModerator,
            PostRepository postRepository,
            CommentRepository commentRepository,
            IMediator mediator)
        {
            _contentModerator = contentModerator;
            _postRepository = postRepository;
            _commentRepository = commentRepository;
            _mediator = mediator;
        }

        public async Task<Result<ModerationResult>> Handle(
            ModerateContentCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                string content;
                switch (request.ContentType)
                {
                    case ContentType.Post:
                        var post = await _postRepository.GetByIdAsync(request.ContentId);
                        if (post == null)
                            return Result.Failure<ModerationResult>($"Post {request.ContentId} not found");
                        content = post.Content;
                        break;

                    case ContentType.Comment:
                        var comment = await _commentRepository.GetByIdAsync(request.ContentId);
                        if (comment == null)
                            return Result.Failure<ModerationResult>($"Comment {request.ContentId} not found");
                        content = comment.Content;
                        break;

                    default:
                        return Result.Failure<ModerationResult>($"Unsupported content type: {request.ContentType}");
                }

                // Get moderation decision
                var moderationAction = await _contentModerator.EvaluateContentAsync(content);
                var toxicityScores = await _contentModerator.GetToxicityScoresAsync(content);

                var result = new ModerationResult
                {
                    Action = moderationAction,
                    CivilityScore = 1 - (decimal)toxicityScores["toxicity"],
                    ViolatedPolicies = toxicityScores
                        .Where(s => s.Value > 0.7m)
                        .Select(s => s.Key)
                        .ToList()
                };

                // Set reason based on violations
                result.Reason = result.ViolatedPolicies.Any()
                    ? $"Content violates community standards: {string.Join(", ", result.ViolatedPolicies)}"
                    : "Content meets community standards";

                // Apply moderation action
                switch (request.ContentType)
                {
                    case ContentType.Post:
                        var post = await _postRepository.GetByIdAsync(request.ContentId);
                        if (moderationAction.ActionType == ModerationActionType.Delete)
                        {
                            post.Delete();
                            await _postRepository.UpdateAsync(post);
                        }
                        break;

                    case ContentType.Comment:
                        var comment = await _commentRepository.GetByIdAsync(request.ContentId);
                        if (moderationAction.ActionType == ModerationActionType.Delete)
                        {
                            comment.Delete();
                            await _commentRepository.UpdateAsync(comment);
                        }
                        break;
                }

                // Publish event
                await _mediator.Publish(new ContentModeratedEvent(
                    request.ContentId,
                    request.ContentType,
                    moderationAction,
                    result.Reason,
                    request.IsAutomated,
                    request.ModeratorId));

                return Result.Success(result);
            }
            catch (Exception ex)
            {
                return Result.Failure<ModerationResult>($"Error moderating content: {ex.Message}");
            }
        }
    }
}
