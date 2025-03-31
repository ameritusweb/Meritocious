using MediatR;
using Meritocious.Core.Features.Posts.Commands;
using Meritocious.Core.Interfaces;
using Meritocious.Core.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meritocious.Infrastructure.Commands
{
    public class DeletePostCommandHandler : IRequestHandler<DeletePostCommand, Result>
    {
        private readonly IPostService _postService;
        private readonly IMediator _mediator;

        public DeletePostCommandHandler(
            IPostService postService,
            IMediator mediator)
        {
            _postService = postService;
            _mediator = mediator;
        }

        public async Task<Result> Handle(DeletePostCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _postService.DeletePostAsync(request.PostId);

                // Could publish a PostDeletedEvent here
                // if (request.DeletedByUserId.HasValue)
                // {
                //     await _mediator.Publish(new PostDeletedEvent(request.PostId, request.DeletedByUserId.Value), cancellationToken);
                // }

                return Result.Success();
            }
            catch (KeyNotFoundException ex)
            {
                return Result.Failure($"Post not found: {ex.Message}");
            }
            catch (Exception ex)
            {
                return Result.Failure($"Error deleting post: {ex.Message}");
            }
        }
    }
}
