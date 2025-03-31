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
        private readonly PostRepository _postRepository;

        public GetTopPostsQueryHandler(PostRepository postRepository)
        {
            _postRepository = postRepository;
        }

        public async Task<List<Post>> Handle(GetTopPostsQuery request, CancellationToken cancellationToken)
        {
            return request.SortBy switch
            {
                "merit" => await _postRepository.GetTopPostsByMeritAsync(request.Count),
                "date" => await _postRepository.GetLatestPostsAsync(request.Count),
                "activity" => await _postRepository.GetMostActivePostsAsync(request.Count),
                _ => await _postRepository.GetTopPostsByMeritAsync(request.Count)
            };
        }
    }
}