using System;
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
    using Meritocious.Common.DTOs.Content;

    public record AddCommentCommand : IRequest<Result<CommentDto>>
    {
        public string PostId { get; init; }
        public string AuthorId { get; init; }
        public string Content { get; init; }
        public string? ParentCommentId { get; init; }
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