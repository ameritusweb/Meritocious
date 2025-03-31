using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meritocious.Core.Features.Posts.Queries
{
    using MediatR;
    using Meritocious.Core.Entities;
    using Meritocious.Core.Results;

    public record GetPostQuery : IRequest<Result<Post>>
    {
        public Guid PostId { get; init; }
    }

    public class GetPostQueryHandler : IRequestHandler<GetPostQuery, Result<Post>>
    {
        private readonly PostRepository _postRepository;

        public GetPostQueryHandler(PostRepository postRepository)
        {
            _postRepository = postRepository;
        }

        public async Task<Result<Post>> Handle(GetPostQuery request, CancellationToken cancellationToken)
        {
            var post = await _postRepository.GetByIdAsync(request.PostId);
            if (post == null)
                return Result.Failure<Post>($"Post {request.PostId} not found");

            return Result.Success(post);
        }
    }
}