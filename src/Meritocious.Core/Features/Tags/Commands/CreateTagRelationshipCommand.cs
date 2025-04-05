using MediatR;
using Meritocious.Common.DTOs.Tags;

namespace Meritocious.Core.Features.Tags.Commands;

public record CreateTagRelationshipCommand(string ParentTagId, string ChildTagId, int RelationType, string CreatedBy)
    : IRequest<TagRelationshipDto>;