using MediatR;
using Meritocious.Common.DTOs.Tags;

namespace Meritocious.Core.Features.Tags.Commands;

public record UpdateTagWikiCommand(
    string TagId,
    string Content,
    string EditedBy,
    string EditSummary
) : IRequest<TagWikiDto>;