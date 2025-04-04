using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meritocious.Core.Features.Posts.Queries
{
    using MediatR;
    using Meritocious.Core.Entities;
    using Meritocious.Infrastructure.Data.Extensions;
    using Meritocious.Infrastructure.Data.Repositories;

    public record GetTopPostsQuery : IRequest<List<Post>>
    {
        public int Count { get; init; } = 10;
        public string SortBy { get; init; } = "merit"; // merit, date, activity
    }

    public class GetTopPostsQueryHandler : IRequestHandler<GetTopPostsQuery, List<Post>>
    {
        private readonly PostRepository postRepository;

        public GetTopPostsQueryHandler(PostRepository postRepository)
        {
            this.postRepository = postRepository;
        }

        public async Task<List<Post>> Handle(GetTopPostsQuery request, CancellationToken cancellationToken)
        {
            return request.SortBy switch
            {
                "merit" => await postRepository.GetTopPostsByMeritAsync(request.Count),
                "date" => await postRepository.GetLatestPostsAsync(request.Count),
                "activity" => await postRepository.GetMostActivePostsAsync(request.Count),
                _ => await postRepository.GetTopPostsByMeritAsync(request.Count)
            };
        }
    }
}