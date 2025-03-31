using MediatR;
using Meritocious.Common.Enums;
using Meritocious.Core.Events;
using Meritocious.Core.Features.Merit.Commands;
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
    public class RecalculateMeritScoreCommandHandler : IRequestHandler<RecalculateMeritScoreCommand, Result<decimal>>
    {
        private readonly IMeritScoringService _meritScoringService;
        private readonly PostRepository _postRepository;
        private readonly CommentRepository _commentRepository;
        private readonly IMediator _mediator;

        public RecalculateMeritScoreCommandHandler(
            IMeritScoringService meritScoringService,
            PostRepository postRepository,
            CommentRepository commentRepository,
            IMediator mediator)
        {
            _meritScoringService = meritScoringService;
            _postRepository = postRepository;
            _commentRepository = commentRepository;
            _mediator = mediator;
        }

        public async Task<Result<decimal>> Handle(RecalculateMeritScoreCommand request, CancellationToken cancellationToken)
        {
            try
            {
                string content;
                string context = null;

                switch (request.ContentType)
                {
                    case ContentType.Post:
                        var post = await _postRepository.GetByIdAsync(request.ContentId);
                        if (post == null)
                            return Result.Failure<decimal>($"Post {request.ContentId} not found");
                        content = post.Content;
                        break;

                    case ContentType.Comment:
                        var comment = await _commentRepository.GetByIdAsync(request.ContentId);
                        if (comment == null)
                            return Result.Failure<decimal>($"Comment {request.ContentId} not found");
                        content = comment.Content;
                        context = comment.Post?.Content;
                        break;

                    default:
                        return Result.Failure<decimal>($"Unsupported content type: {request.ContentType}");
                }

                var score = await _meritScoringService.CalculateContentScoreAsync(content, request.ContentType, context);

                // Update the content's merit score
                switch (request.ContentType)
                {
                    case ContentType.Post:
                        var post = await _postRepository.GetByIdAsync(request.ContentId);
                        post.UpdateMeritScore(score.FinalScore);
                        await _postRepository.UpdateAsync(post);
                        break;

                    case ContentType.Comment:
                        var comment = await _commentRepository.GetByIdAsync(request.ContentId);
                        comment.UpdateMeritScore(score.FinalScore);
                        await _commentRepository.UpdateAsync(comment);
                        break;
                }

                // Publish event
                await _mediator.Publish(new ContentScoredEvent(request.ContentId, request.ContentType, score));

                return Result.Success(score.FinalScore);
            }
            catch (Exception ex)
            {
                return Result.Failure<decimal>($"Error calculating merit score: {ex.Message}");
            }
        }
    }
}
