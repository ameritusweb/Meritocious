using MediatR;
using Meritocious.Common.DTOs.Tags;
using Meritocious.Core.Results;

namespace Meritocious.Core.Features.Tags.Commands;

public class UpdateTagWikiCommand : IRequest<TagWikiDto>
{
    public string TagId { get; set; }
    public string Content { get; set; }
    public string EditorId { get; set; }
    public string EditReason { get; set; }
}