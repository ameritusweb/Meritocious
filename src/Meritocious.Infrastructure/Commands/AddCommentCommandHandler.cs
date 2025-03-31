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

    public class AddCommentCommandHandler : IRequestHandler<AddCommentCommand, Result<Comment>>
    {
        private readonly ICommentService _commentService;
        private readonly UserRepository _userRepository;
        private readonly PostRepository _postRepository;
        private readonly IMeritScorer _meritScorer;
        private readonly IMediator _mediator;

        public AddCommentCommandHandler(
            ICommentService commentService,
            UserRepository userRepository,
            PostRepository postRepository,
            IMeritScorer meritScorer,
            IMediator mediator)
        {
            _commentService = commentService;
            _userRepository = userRepository;
            _postRepository = postRepository;
            _meritScorer = meritScorer;
            _mediator = mediator;
        }

        public async Task<Result<Comment>> Handle(AddCommentCommand request, CancellationToken cancellationToken)
        {
            var author = await _userRepository.GetByIdAsync(request.AuthorId);
            if (author == null)
                return Result.Failure<Comment>($"User {request.AuthorId} not found");

            var post = await _postRepository.GetByIdAsync(request.PostId);
            if (post == null)
                return Result.Failure<Comment>($"Post {request.PostId} not found");

            // Validate content quality
            var contentScore = await _meritScorer.ScoreContentAsync(request.Content);
            if (!await _meritScorer.ValidateContentAsync(request.Content))
                return Result.Failure<Comment>("Comment does not meet quality standards");

            var comment = await _commentService.AddCommentAsync(
                request.Content,
                request.PostId,
                author,
                request.ParentCommentId);

            // Publish event
            await _mediator.Publish(
                new CommentAddedEvent(comment.Id, request.PostId, request.AuthorId),
                cancellationToken);

            return Result.Success(comment);
        }
    }
}