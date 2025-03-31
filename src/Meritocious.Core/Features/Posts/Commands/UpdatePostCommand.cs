﻿using System;
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

    public record UpdatePostCommand : IRequest<Result<Post>>
    {
        public Guid PostId { get; init; }
        public Guid EditorId { get; init; }
        public string Title { get; init; }
        public string Content { get; init; }
        public List<string> Tags { get; init; } = new();
    }

    public class UpdatePostCommandValidator : AbstractValidator<UpdatePostCommand>
    {
        public UpdatePostCommandValidator()
        {
            RuleFor(x => x.PostId).NotEmpty();
            RuleFor(x => x.EditorId).NotEmpty();
            RuleFor(x => x.Title).NotEmpty().Length(5, 255);
            RuleFor(x => x.Content).NotEmpty().MinimumLength(20);
            RuleForEach(x => x.Tags).NotEmpty().MaximumLength(50);
        }
    }
}