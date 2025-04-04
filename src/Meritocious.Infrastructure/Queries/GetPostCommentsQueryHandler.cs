using MediatR;
using Meritocious.Core.Entities;
using Meritocious.Core.Features.Comments.Queries;
using Meritocious.Core.Results;
using Meritocious.Infrastructure.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class GetPostCommentsQueryHandler : IRequestHandler<GetPostCommentsQuery, Result<List<Comment>>>
{
    private readonly CommentRepository commentRepository;
    private readonly PostRepository postRepository;

    public GetPostCommentsQueryHandler(
        CommentRepository commentRepository,
        PostRepository postRepository)
    {
        this.commentRepository = commentRepository;
        this.postRepository = postRepository;
    }

    public async Task<Result<List<Comment>>> Handle(GetPostCommentsQuery request, CancellationToken cancellationToken)
    {
        var post = await postRepository.GetByIdAsync(request.PostId);
        if (post == null)
        {
            return Result.Failure<List<Comment>>($"Post {request.PostId} not found");
        }

        var comments = request.SortBy switch
        {
            "merit" => await commentRepository.GetCommentsByPostOrderedByMeritAsync(
                request.PostId, request.Page, request.PageSize),
            "date" => await commentRepository.GetCommentsByPostOrderedByDateAsync(
                request.PostId, request.Page, request.PageSize),
            "thread" => await commentRepository.GetCommentsByPostThreadedAsync(
                request.PostId, request.Page, request.PageSize),
            _ => await commentRepository.GetCommentsByPostOrderedByMeritAsync(
                request.PostId, request.Page, request.PageSize)
        };

        return Result.Success(comments);
    }
}