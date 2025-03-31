﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meritocious.Core.Features.Comments.Commands
{
    using MediatR;
    using FluentValidation;
    using Meritocious.Core.Entities;
    using Meritocious.Core.Results;

    public record AddCommentCommand : IRequest<Result<Comment>>
    {
        public Guid PostId { get; init; }
        public Guid AuthorId { get; init; }
        public string Content { get; init; }
        public Guid? ParentCommentId { get; init; }
    }

    public class AddCommentCommandValidator : AbstractValidator<AddCommentCommand>
    {
        public AddCommentCommandValidator()
        {
            RuleFor(x => x.PostId).NotEmpty();
            RuleFor(x => x.AuthorId).NotEmpty();
            RuleFor(x => x.Content).NotEmpty().MinimumLength(10);
        }
    }
}