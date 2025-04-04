namespace Meritocious.Core.Features.Search.Queries
{
    using MediatR;
    using Meritocious.Common.DTOs.Auth;
    using Meritocious.Common.DTOs.Contributions;
    using Meritocious.Core.Features.Users.Queries;
    using Meritocious.Core.Results;
    using Meritocious.Infrastructure.Data.Extensions;
    using Meritocious.Infrastructure.Data.Repositories;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class GetUserProfileQueryHandler : IRequestHandler<GetUserProfileQuery, Result<UserProfileDto>>
    {
        private readonly UserRepository userRepository;
        private readonly PostRepository postRepository;
        private readonly CommentRepository commentRepository;

        public GetUserProfileQueryHandler(
            UserRepository userRepository,
            PostRepository postRepository,
            CommentRepository commentRepository)
        {
            this.userRepository = userRepository;
            this.postRepository = postRepository;
            this.commentRepository = commentRepository;
        }

        public async Task<Result<UserProfileDto>> Handle(GetUserProfileQuery request, CancellationToken cancellationToken)
        {
            var user = await userRepository.GetByIdAsync(request.UserId);
            if (user == null)
            {
                return Result.Failure<UserProfileDto>($"User {request.UserId} not found");
            }

            // Get user's top contributions
            var topPosts = await postRepository.GetTopPostsByUserAsync(request.UserId, 5);
            var topComments = await commentRepository.GetTopCommentsByUserAsync(request.UserId, 5);

            var profile = new UserProfileDto
            {
                Id = Guid.Parse(user.Id),
                Username = user.UserName,
                Email = user.Email,
                MeritScore = user.MeritScore,
                CreatedAt = user.CreatedAt,
                LastLoginAt = user.LastLoginAt,
                TopContributions = topPosts.Select(p => new ContributionSummaryDto
                {
                    Id = p.Id,
                    Type = "Post",
                    Title = p.Title,
                    MeritScore = p.MeritScore,
                    CreatedAt = p.CreatedAt
                })
                .Concat(topComments.Select(c => new ContributionSummaryDto
                {
                    Id = c.Id,
                    Type = "Comment",
                    Title = $"Comment on {c.Post.Title}",
                    MeritScore = c.MeritScore,
                    CreatedAt = c.CreatedAt
                }))
                .OrderByDescending(c => c.MeritScore)
                .ToList()
            };

            return Result.Success(profile);
        }
    }
}