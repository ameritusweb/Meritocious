using MediatR;
using Meritocious.Common.DTOs.Content;
using Meritocious.Core.Entities;
using Meritocious.Core.Extensions;
using Meritocious.Core.Features.Posts.Queries;
using Meritocious.Core.Results;
using Meritocious.Infrastructure.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meritocious.Infrastructure.Queries
{
    public class GetPostsByUserQueryHandler : IRequestHandler<GetPostsByUserQuery, Result<List<PostDto>>>
    {
        private readonly PostRepository _postRepository;
        private readonly UserRepository _userRepository;

        public GetPostsByUserQueryHandler(
            PostRepository postRepository,
            UserRepository userRepository)
        {
            _postRepository = postRepository;
            _userRepository = userRepository;
        }

        public async Task<Result<List<PostDto>>> Handle(GetPostsByUserQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Verify the user exists
                var user = await _userRepository.GetByIdAsync(request.UserId);
                if (user == null)
                    return Result.Failure<List<PostDto>>($"User {request.UserId} not found");

                // Get posts by user
                var posts = await _postRepository.GetPostsByUserAsync(request.UserId);

                // Apply sorting
                posts = request.SortBy switch
                {
                    "merit" => posts.OrderByDescending(p => p.MeritScore).ToList(),
                    _ => posts.OrderByDescending(p => p.CreatedAt).ToList() // Default is date
                };

                // Apply pagination if needed
                if (request.Page.HasValue && request.PageSize.HasValue)
                {
                    int skip = (request.Page.Value - 1) * request.PageSize.Value;
                    posts = posts.Skip(skip).Take(request.PageSize.Value).ToList();
                }

                return Result.Success(posts.ToDtoList());
            }
            catch (Exception ex)
            {
                return Result.Failure<List<PostDto>>($"Error retrieving posts: {ex.Message}");
            }
        }
    }
}
