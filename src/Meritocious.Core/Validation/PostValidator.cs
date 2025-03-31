using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meritocious.Core.Validation
{
    using FluentValidation;
    using Meritocious.Core.Entities;

    public class PostValidator : AbstractValidator<Post>
    {
        public PostValidator()
        {
            RuleFor(p => p.Title)
                .NotEmpty()
                .Length(5, 255)
                .WithMessage("Title must be between 5 and 255 characters");

            RuleFor(p => p.Content)
                .NotEmpty()
                .MinimumLength(20)
                .WithMessage("Content must be at least 20 characters long");

            RuleFor(p => p.AuthorId)
                .NotEmpty()
                .WithMessage("Author is required");

            RuleFor(p => p.MeritScore)
                .InclusiveBetween(0, 1)
                .WithMessage("Merit score must be between 0 and 1");
        }
    }
}