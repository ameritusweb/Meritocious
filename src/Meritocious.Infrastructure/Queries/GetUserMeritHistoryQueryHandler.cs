using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Meritocious.Core.Entities;
using Meritocious.Core.Features.Merit.Queries;
using Meritocious.Core.Results;
using Meritocious.Infrastructure.Data;
using Meritocious.Infrastructure.Data.Repositories;

namespace Meritocious.Infrastructure.Queries
{
    public class GetUserMeritHistoryQueryHandler : IRequestHandler<GetUserMeritHistoryQuery, Result<List<ReputationSnapshot>>>
    {
        private readonly ReputationSnapshotRepository snapshotRepository;

        public GetUserMeritHistoryQueryHandler(ReputationSnapshotRepository snapshotRepository)
        {
            this.snapshotRepository = snapshotRepository;
        }

        public async Task<Result<List<ReputationSnapshot>>> Handle(GetUserMeritHistoryQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var snapshots = await snapshotRepository.GetUserSnapshotsAsync(
                    request.UserId,
                    request.TimeFrame,
                    request.StartDate,
                    request.EndDate);

                return Result<List<ReputationSnapshot>>.Success(snapshots);
            }
            catch (Exception ex)
            {
                return Result<List<ReputationSnapshot>>.Failure("Failed to retrieve merit history.");
            }
        }
    }
}