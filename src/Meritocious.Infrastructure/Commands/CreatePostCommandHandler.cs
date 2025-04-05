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
    using Meritocious.Core.Extensions;
    using Meritocious.Core.Interfaces;
    using Meritocious.Infrastructure.Data.Repositories;

    public class CreatePostCommandHandler : IRequestHandler<CreatePostCommand, Post>
    {
        private readonly IPostService postService;
        private readonly UserRepository userRepository;
        private readonly ITagService tagService;
        private readonly IMediator mediator;

        public CreatePostCommandHandler(
            IPostService postService,
            UserRepository userRepository,
            ITagService tagService,
            IMediator mediator)
        {
            this.postService = postService;
            this.userRepository = userRepository;
            this.tagService = tagService;
            this.mediator = mediator;
        }

        public async Task<Post> Handle(CreatePostCommand request, CancellationToken cancellationToken)
        {
            var author = await userRepository.GetByIdAsync(request.AuthorId)
                ?? throw new ResourceNotFoundException($"User {request.AuthorId} not found");

            Post parent = null;
            if (request.ParentPostId.HasValue())
            {
                parent = await postService.GetPostByIdAsync(request.ParentPostId)
                    ?? throw new ResourceNotFoundException($"Parent post {request.ParentPostId} not found");
            }

            var post = await postService.CreatePostAsync(request.Title, request.Content, author, parent);

            // Add tags
            foreach (var tagNameAndCategory in request.Tags.Zip(request.TagCategories))
            {
                await tagService.AddTagToPostAsync(post.Id, tagNameAndCategory.First, tagNameAndCategory.Second);
            }

            // Publish domain event
            await mediator.Publish(new PostCreatedEvent(post.Id, author.Id), cancellationToken);

            return post;
        }
    }
}
