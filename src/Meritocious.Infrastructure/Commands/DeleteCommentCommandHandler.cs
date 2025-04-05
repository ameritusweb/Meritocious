using MediatR;
using Meritocious.Core.Features.Comments.Commands;
using Meritocious.Core.Interfaces;
using Meritocious.Core.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meritocious.Infrastructure.Commands
{
    public class DeleteCommentCommandHandler : IRequestHandler<DeleteCommentCommand, Result>
    {
        private readonly ICommentService commentService;

        public DeleteCommentCommandHandler(ICommentService commentService)
        {
            this.commentService = commentService;
        }

        public async Task<Result> Handle(DeleteCommentCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await commentService.DeleteCommentAsync(request.CommentId);
                return Result.Success();
            }
            catch (KeyNotFoundException ex)
            {
                return Result.Failure(ex.Message);
            }
            catch (Exception ex)
            {
                return Result.Failure($"Error deleting comment: {ex.Message}");
            }
        }
    }
}
