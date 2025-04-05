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
        private readonly IPostService postService;
        private readonly IMediator mediator;

        public DeletePostCommandHandler(
            IPostService postService,
            IMediator mediator)
        {
            this.postService = postService;
            this.mediator = mediator;
        }

        public async Task<Result> Handle(DeletePostCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await postService.DeletePostAsync(request.PostId);

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
