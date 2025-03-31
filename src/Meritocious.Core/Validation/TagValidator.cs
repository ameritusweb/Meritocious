using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meritocious.Core.Validation
{
    using FluentValidation;
    using Meritocious.Core.Entities;
    using System.Text.RegularExpressions;

    public class TagValidator : AbstractValidator<Tag>
    {
        public TagValidator()
        {
            RuleFor(t => t.Name)
                .NotEmpty()
                .Length(2, 50)
                .Matches(new Regex("^[a-zA-Z0-9-]+$"))
                .WithMessage("Tag name can only contain letters, numbers, and hyphens");

            RuleFor(t => t.Description)
                .MaximumLength(255)
                .When(t => t.Description != null);
        }
    }
}