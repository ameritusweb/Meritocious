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
    using Meritocious.Infrastructure.Data.Repositories;

    public record GetPostQuery : IRequest<Result<Post>>
    {
        public string PostId { get; init; }
    }

    public class GetPostQueryHandler : IRequestHandler<GetPostQuery, Result<Post>>
    {
        private readonly PostRepository postRepository;

        public GetPostQueryHandler(PostRepository postRepository)
        {
            this.postRepository = postRepository;
        }

        public async Task<Result<Post>> Handle(GetPostQuery request, CancellationToken cancellationToken)
        {
            var post = await postRepository.GetByIdAsync(request.PostId);
            if (post == null)
            {
                return Result.Failure<Post>($"Post {request.PostId} not found");
            }

            return Result.Success(post);
        }
    }
}