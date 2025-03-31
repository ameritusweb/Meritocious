using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meritocious.Core.Validation
{
    using FluentValidation;
    using Meritocious.Core.Entities;

    public class CommentValidator : AbstractValidator<Comment>
    {
        public CommentValidator()
        {
            RuleFor(c => c.Content)
                .NotEmpty()
                .MinimumLength(10)
                .WithMessage("Comment must be at least 10 characters long");

            RuleFor(c => c.AuthorId)
                .NotEmpty()
                .WithMessage("Author is required");

            RuleFor(c => c.PostId)
                .NotEmpty()
                .WithMessage("Post ID is required");

            RuleFor(c => c.MeritScore)
                .InclusiveBetween(0, 1)
                .WithMessage("Merit score must be between 0 and 1");
        }
    }
}