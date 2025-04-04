using MediatR;
using Meritocious.Common.DTOs.Substacks;

namespace Meritocious.Core.Features.Substacks.Commands;

public record CreateSubstackCommand(
    string Name,
    string Subdomain,
    string CustomDomain,
    string AuthorName,
    string Description,
    string LogoUrl,
    string CoverImageUrl,
    string TwitterHandle)
    : IRequest<SubstackDto>;
