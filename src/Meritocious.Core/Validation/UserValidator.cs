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

    public class UserValidator : AbstractValidator<User>
    {
        public UserValidator()
        {
            RuleFor(u => u.UserName)
                .NotEmpty()
                .Length(3, 50)
                .Matches(new Regex("^[a-zA-Z0-9_-]+$"))
                .WithMessage("Username can only contain letters, numbers, underscores, and hyphens");

            RuleFor(u => u.Email)
                .NotEmpty()
                .EmailAddress()
                .MaximumLength(255)
                .WithMessage("Invalid email address");

            RuleFor(u => u.PasswordHash)
                .NotEmpty()
                .WithMessage("Password hash is required");

            RuleFor(u => u.MeritScore)
                .InclusiveBetween(0, 1)
                .WithMessage("Merit score must be between 0 and 1");
        }
    }
}
