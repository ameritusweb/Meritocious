using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meritocious.Core.Commands
{
    using MediatR;
    using Meritocious.Core.Entities;
    using Meritocious.Core.Events;
    using Meritocious.Core.Exceptions;
    using Meritocious.Core.Interfaces;
    using Meritocious.Infrastructure.Data.Repositories;

    public class CreatePostCommandHandler : IRequestHandler<CreatePostCommand, Post>
    {
        private readonly IPostService _postService;
        private readonly UserRepository _userRepository;
        private readonly ITagService _tagService;
        private readonly IMediator _mediator;

        public CreatePostCommandHandler(
            IPostService postService,
            UserRepository userRepository,
            ITagService tagService,
            IMediator mediator)
        {
            _postService = postService;
            _userRepository = userRepository;
            _tagService = tagService;
            _mediator = mediator;
        }

        public async Task<Post> Handle(CreatePostCommand request, CancellationToken cancellationToken)
        {
            var author = await _userRepository.GetByIdAsync(request.AuthorId)
                ?? throw new ResourceNotFoundException($"User {request.AuthorId} not found");

            Post parent = null;
            if (request.ParentPostId.HasValue)
            {
                parent = await _postService.GetPostByIdAsync(request.ParentPostId.Value)
                    ?? throw new ResourceNotFoundException($"Parent post {request.ParentPostId} not found");
            }

            var post = await _postService.CreatePostAsync(request.Title, request.Content, author, parent);

            // Add tags
            foreach (var tagName in request.Tags)
            {
                await _tagService.AddTagToPostAsync(post.Id, tagName);
            }

            // Publish domain event
            await _mediator.Publish(new PostCreatedEvent(post.Id, author.Id), cancellationToken);

            return post;
        }
    }
}
