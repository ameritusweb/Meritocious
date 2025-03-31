using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meritocious.Core.Features.Moderation.Commands
{
    using MediatR;
    using Meritocious.Common.Enums;
    using Meritocious.Core.Results;

    public record ModerateContentCommand : IRequest<Result<ModerationResult>>
    {
        public Guid ContentId { get; init; }
        public ContentType ContentType { get; init; }
        public bool IsAutomated { get; init; } = true;
        public Guid? ModeratorId { get; init; }
    }

    public class ModerationResult
    {
        public ModerationAction Action { get; set; }
        public string Reason { get; set; }
        public decimal CivilityScore { get; set; }
        public List<string> ViolatedPolicies { get; set; } = new();
    }
}