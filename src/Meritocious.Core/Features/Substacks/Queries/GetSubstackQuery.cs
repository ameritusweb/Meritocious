using MediatR;
using Meritocious.Common.DTOs.Substacks;

namespace Meritocious.Core.Features.Substacks.Queries;

public record GetSubstackQuery(string SubstackId) : IRequest<SubstackDto>;
