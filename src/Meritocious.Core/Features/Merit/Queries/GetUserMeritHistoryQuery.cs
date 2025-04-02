using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Meritocious.Core.Entities;
using Meritocious.Core.Results;

namespace Meritocious.Core.Features.Merit.Queries
{
    public class GetUserMeritHistoryQuery : IRequest<Result<List<ReputationSnapshot>>>
    {
        public Guid UserId { get; set; }
        public string TimeFrame { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}