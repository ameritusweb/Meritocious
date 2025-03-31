using MediatR;
using Meritocious.Core.Exceptions;
using Meritocious.Core.Features.Discovery.Queries;
using Meritocious.Core.Interfaces;
using Meritocious.Infrastructure.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class GetRecommendedPostsQueryHandler : IRequestHandler<GetRecommendedPostsQuery, List<PostRecommendationDto>>
{
    private readonly IRecommendationService _recommendationService;
    private readonly PostRepository _postRepository;
    private readonly UserRepository _userRepository;

    public GetRecommendedPostsQueryHandler(
        IRecommendationService recommendationService,
        PostRepository postRepository,
        UserRepository userRepository)
    {
        _recommendationService = recommendationService;
        _postRepository = postRepository;
        _userRepository = userRepository;
    }

    public async Task<List<PostRecommendationDto>> Handle(
        GetRecommendedPostsQuery request,
        CancellationToken cancellationToken)
    {
        // Get user's activity history
        var user = await _userRepository.GetByIdAsync(request.UserId);
        if (user == null)
            throw new ResourceNotFoundException($"User {request.UserId} not found");

        // Get user's recent interactions
        var userHistory = await _postRepository.GetUserInteractionHistoryAsync(request.UserId);

        // Get recommendations
        var recommendations = await _recommendationService.GetRecommendationsAsync(
            request.UserId,
            userHistory,
            request.Count,
            request.ExcludedPostIds);

        var results = new List<PostRecommendationDto>();
        foreach (var rec in recommendations)
        {
            var post = await _postRepository.GetByIdAsync(rec.ContentId);
            if (post != null)
            {
                results.Add(new PostRecommendationDto
                {
                    PostId = post.Id,
                    Title = post.Title,
                    AuthorUsername = post.Author.Username,
                    MeritScore = post.MeritScore,
                    Tags = post.Tags.Select(t => t.Name).ToList(),
                    RecommendationReason = rec.Reason,
                    RelevanceScore = rec.RelevanceScore
                });
            }
        }

        return results;
    }
}