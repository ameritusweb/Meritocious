using MediatR;
using Meritocious.Common.DTOs.Tags;

namespace Meritocious.Core.Features.Tags.Commands;

public class CreateTagRelationshipCommand : IRequest<TagRelationshipDto>
{
    public string ParentTagId { get; set; }
    public string ChildTagId { get; set; }
    public int RelationType { get; set; }
    public string CreatedBy { get; set; }
}