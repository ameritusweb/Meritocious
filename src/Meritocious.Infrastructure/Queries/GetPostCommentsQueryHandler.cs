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
    private readonly CommentRepository _commentRepository;
    private readonly PostRepository _postRepository;

    public GetPostCommentsQueryHandler(
        CommentRepository commentRepository,
        PostRepository postRepository)
    {
        _commentRepository = commentRepository;
        _postRepository = postRepository;
    }

    public async Task<Result<List<Comment>>> Handle(GetPostCommentsQuery request, CancellationToken cancellationToken)
    {
        var post = await _postRepository.GetByIdAsync(request.PostId);
        if (post == null)
            return Result.Failure<List<Comment>>($"Post {request.PostId} not found");

        var comments = request.SortBy switch
        {
            "merit" => await _commentRepository.GetCommentsByPostOrderedByMeritAsync(
                request.PostId, request.Page, request.PageSize),
            "date" => await _commentRepository.GetCommentsByPostOrderedByDateAsync(
                request.PostId, request.Page, request.PageSize),
            "thread" => await _commentRepository.GetCommentsByPostThreadedAsync(
                request.PostId, request.Page, request.PageSize),
            _ => await _commentRepository.GetCommentsByPostOrderedByMeritAsync(
                request.PostId, request.Page, request.PageSize)
        };

        return Result.Success(comments);
    }
}