using MediatR;
using Meritocious.Common.DTOs.Merit;

namespace Meritocious.Core.Features.Merit.Queries;

public record GetUserMeritScoreQuery(string UserId)
    : IRequest<MeritScoreDto>;