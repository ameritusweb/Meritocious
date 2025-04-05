using MediatR;
using Meritocious.Common.DTOs.Tags;

namespace Meritocious.Core.Features.Tags.Commands;

public class AddTagSynonymCommand : IRequest<TagSynonymDto>
{
    public string SourceTagId { get; set; }
    public string TargetTagId { get; set; }
    public string CreatedBy { get; set; }
}