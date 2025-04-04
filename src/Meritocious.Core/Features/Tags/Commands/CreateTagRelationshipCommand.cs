using MediatR;
using Meritocious.Common.DTOs.Tags;

namespace Meritocious.Core.Features.Tags.Commands;

public record CreateTagRelationshipCommand(string ParentTagId, string ChildTagId, string RelationType, string CreatedBy)
    : IRequest<TagRelationshipDto>;