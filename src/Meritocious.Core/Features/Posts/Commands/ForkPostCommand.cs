using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meritocious.Core.Features.Posts.Commands
{
    using MediatR;
    using FluentValidation;
    using Meritocious.Core.Entities;
    using Meritocious.Core.Results;

    public record ForkPostCommand : IRequest<Result<Post>>
    {
        public string OriginalPostId { get; init; }
        public string NewAuthorId { get; init; }
        public string NewTitle { get; init; }
    }

    public class ForkPostCommandValidator : AbstractValidator<ForkPostCommand>
    {
        public ForkPostCommandValidator()
        {
            RuleFor(x => x.OriginalPostId).NotEmpty();
            RuleFor(x => x.NewAuthorId).NotEmpty();
            RuleFor(x => x.NewTitle).NotEmpty().Length(5, 255);
        }
    }
}