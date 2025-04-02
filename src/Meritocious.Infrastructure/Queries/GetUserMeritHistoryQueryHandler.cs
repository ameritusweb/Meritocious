using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Meritocious.Core.Entities;
using Meritocious.Core.Features.Merit.Queries;
using Meritocious.Core.Results;
using Meritocious.Infrastructure.Data;

namespace Meritocious.Infrastructure.Queries
{
    public class GetUserMeritHistoryQueryHandler : IRequestHandler<GetUserMeritHistoryQuery, Result<List<ReputationSnapshot>>>
    {
        private readonly ReputationSnapshotRepository _snapshotRepository;

        public GetUserMeritHistoryQueryHandler(ReputationSnapshotRepository snapshotRepository)
        {
            _snapshotRepository = snapshotRepository;
        }

        public async Task<Result<List<ReputationSnapshot>>> Handle(GetUserMeritHistoryQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var snapshots = await _snapshotRepository.GetUserSnapshotsAsync(
                    request.UserId,
                    request.TimeFrame,
                    request.StartDate,
                    request.EndDate);

                return Result<List<ReputationSnapshot>>.Success(snapshots);
            }
            catch (Exception ex)
            {
                return Result<List<ReputationSnapshot>>.Failure(new[] { "Failed to retrieve merit history." });
            }
        }
    }
}