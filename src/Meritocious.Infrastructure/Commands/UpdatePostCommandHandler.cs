using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meritocious.Core.Features.Posts.Commands
{
    using MediatR;
    using Meritocious.Core.Entities;
    using Meritocious.Core.Results;
    using Meritocious.Core.Interfaces;
    using Meritocious.Infrastructure.Data.Repositories;
    using Meritocious.AI.MeritScoring.Interfaces;
    using Meritocious.Core.Events;

    public class UpdatePostCommandHandler : IRequestHandler<UpdatePostCommand, Result<Post>>
    {
        private readonly PostRepository _postRepository;
        private readonly ITagService _tagService;
        private readonly IMeritScorer _meritScorer;
        private readonly IMediator _mediator;

        public UpdatePostCommandHandler(
            PostRepository postRepository,
            ITagService tagService,
            IMeritScorer meritScorer,
            IMediator mediator)
        {
            _postRepository = postRepository;
            _tagService = tagService;
            _meritScorer = meritScorer;
            _mediator = mediator;
        }

        public async Task<Result<Post>> Handle(UpdatePostCommand request, CancellationToken cancellationToken)
        {
            var post = await _postRepository.GetByIdAsync(request.PostId);
            if (post == null)
                return Result.Failure<Post>($"Post {request.PostId} not found");

            if (post.AuthorId != request.EditorId)
                return Result.Failure<Post>("Only the author can edit this post");

            // Validate content quality
            var contentScore = await _meritScorer.ScoreContentAsync(request.Content);
            if (!await _meritScorer.ValidateContentAsync(request.Content))
                return Result.Failure<Post>("Content does not meet quality standards");

            // Update post
            post.UpdateContent(request.Title, request.Content);
            post.UpdateMeritScore(contentScore.FinalScore);

            // Update tags
            foreach (var tagName in request.Tags)
            {
                await _tagService.AddTagToPostAsync(post.Id, tagName);
            }

            await _postRepository.UpdateAsync(post);

            // Publish event
            await _mediator.Publish(new PostUpdatedEvent(post.Id, request.EditorId), cancellationToken);

            return Result.Success(post);
        }
    }
}