using MediatR;
using Meritocious.Common.DTOs.Tags;

namespace Meritocious.Core.Features.Tags.Commands;

public record AddTagSynonymCommand(
    string SourceTagId,
    string TargetTagId,
    string CreatedBy)
    : IRequest<TagSynonymDto>;