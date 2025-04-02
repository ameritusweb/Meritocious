using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Meritocious.Common.DTOs.Content;
using Meritocious.Core.Features.Posts.Queries;
using Meritocious.Infrastructure.Data;

namespace Meritocious.Infrastructure.Queries
{
    public class GetPostHistoryQueryHandler : IRequestHandler<GetPostHistoryQuery, List<PostVersionDto>>
    {
        private readonly MeritociousDbContext _context;
        private readonly ILogger<GetPostHistoryQueryHandler> _logger;

        public GetPostHistoryQueryHandler(
            MeritociousDbContext context,
            ILogger<GetPostHistoryQueryHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<PostVersionDto>> Handle(
            GetPostHistoryQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                var query = _context.ContentVersions
                    .Include(v => v.CreatedByUser)
                    .Include(v => v.Tags)
                    .Where(v => v.PostId == request.PostId);

                if (request.StartVersion.HasValue)
                {
                    query = query.Where(v => v.VersionNumber >= request.StartVersion.Value);
                }

                if (request.EndVersion.HasValue)
                {
                    query = query.Where(v => v.VersionNumber <= request.EndVersion.Value);
                }

                var versions = await query
                    .OrderByDescending(v => v.VersionNumber)
                    .Select(v => new PostVersionDto
                    {
                        Id = v.Id,
                        PostId = v.PostId,
                        VersionNumber = v.VersionNumber,
                        Title = v.Title,
                        Content = request.IncludeContent ? v.Content : string.Empty,
                        Tags = v.Tags.Select(t => t.Name).ToList(),
                        ChangeDescription = v.ChangeDescription,
                        CreatedAt = v.CreatedAt,
                        CreatedBy = v.CreatedByUser.Username,
                        MeritScore = v.MeritScore,
                        ParentVersionId = v.ParentVersionId,
                        NextVersionId = v.NextVersionId,
                        Changes = v.Changes,
                        AddedLines = v.AddedLines,
                        RemovedLines = v.RemovedLines,
                        ModifiedLines = v.ModifiedLines
                    })
                    .ToListAsync(cancellationToken);

                return versions;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting post history for post {PostId}", request.PostId);
                throw;
            }
        }
    }
}