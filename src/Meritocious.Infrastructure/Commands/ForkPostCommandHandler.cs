using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meritocious.Core.Features.Posts.Commands
{
    using MediatR;
    using Meritocious.Core.Entities;
    using Meritocious.Core.Events;
    using Meritocious.Core.Results;
    using Meritocious.Infrastructure.Data.Repositories;

    public class ForkPostCommandHandler : IRequestHandler<ForkPostCommand, Result<Post>>
    {
        private readonly PostRepository _postRepository;
        private readonly UserRepository _userRepository;
        private readonly IMediator _mediator;

        public ForkPostCommandHandler(
            PostRepository postRepository,
            UserRepository userRepository,
            IMediator mediator)
        {
            _postRepository = postRepository;
            _userRepository = userRepository;
            _mediator = mediator;
        }

        public async Task<Result<Post>> Handle(ForkPostCommand request, CancellationToken cancellationToken)
        {
            var originalPost = await _postRepository.GetByIdAsync(request.OriginalPostId);
            if (originalPost == null)
                return Result.Failure<Post>($"Original post {request.OriginalPostId} not found");

            var newAuthor = await _userRepository.GetByIdAsync(request.NewAuthorId);
            if (newAuthor == null)
                return Result.Failure<Post>($"User {request.NewAuthorId} not found");

            var forkedPost = originalPost.CreateFork(newAuthor, request.NewTitle);
            await _postRepository.AddAsync(forkedPost);

            await _mediator.Publish(
                new PostForkedEvent(originalPost.Id, forkedPost.Id, newAuthor.Id),
                cancellationToken);

            return Result.Success(forkedPost);
        }
    }
}