using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meritocious.Core.Features.Comments.Commands
{
    using MediatR;
    using Meritocious.Core.Entities;
    using Meritocious.Core.Results;
    using Meritocious.Core.Interfaces;
    using Meritocious.Infrastructure.Data.Repositories;
    using Meritocious.AI.MeritScoring.Interfaces;
    using Meritocious.Core.Features.Comments.Events;
    using Meritocious.Common.DTOs.Content;
    using Meritocious.Core.Extensions;

    public class AddCommentCommandHandler : IRequestHandler<AddCommentCommand, Result<CommentDto>>
    {
        private readonly ICommentService commentService;
        private readonly UserRepository userRepository;
        private readonly PostRepository postRepository;
        private readonly IMeritScorer meritScorer;
        private readonly IMediator mediator;

        public AddCommentCommandHandler(
            ICommentService commentService,
            UserRepository userRepository,
            PostRepository postRepository,
            IMeritScorer meritScorer,
            IMediator mediator)
        {
            this.commentService = commentService;
            this.userRepository = userRepository;
            this.postRepository = postRepository;
            this.meritScorer = meritScorer;
            this.mediator = mediator;
        }

        public async Task<Result<CommentDto>> Handle(AddCommentCommand request, CancellationToken cancellationToken)
        {
            var author = await userRepository.GetByIdAsync(request.AuthorId);
            if (author == null)
            {
                return Result.Failure<CommentDto>($"User {request.AuthorId} not found");
            }

            var post = await postRepository.GetByIdAsync(request.PostId);
            if (post == null)
            {
                return Result.Failure<CommentDto>($"Post {request.PostId} not found");
            }

            // Validate content quality
            var contentScore = await meritScorer.ScoreContentAsync(request.Content);
            if (!await meritScorer.ValidateContentAsync(request.Content))
            {
                return Result.Failure<CommentDto>("Comment does not meet quality standards");
            }

            var comment = await commentService.AddCommentAsync(
                request.Content,
                request.PostId,
                author,
                request.ParentCommentId);

            // Publish event
            await mediator.Publish(
                new CommentAddedEvent(comment.Id, request.PostId, request.AuthorId),
                cancellationToken);

            return Result.Success(comment.ToDto());
        }
    }
}