using MediatR;
using Meritocious.Common.DTOs.Substacks;

namespace Meritocious.Core.Features.Substacks.Commands;

public record UpdateSubstackCommand(
    string SubstackId,
    string Name,
    string CustomDomain,
    string Description,
    string LogoUrl,
    string CoverImageUrl,
    string TwitterHandle)
    : IRequest<SubstackDto>;
