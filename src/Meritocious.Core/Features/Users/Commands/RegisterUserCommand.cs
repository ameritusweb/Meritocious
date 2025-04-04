using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meritocious.Core.Features.Users.Commands
{
    using MediatR;
    using FluentValidation;
    using Meritocious.Core.Entities;
    using Meritocious.Core.Results;

    public record RegisterUserCommand : IRequest<Result<UserProfileDto>>
    {
        public string Username { get; init; }
        public string Email { get; init; }
        public string Password { get; init; }
    }

    public class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
    {
        public RegisterUserCommandValidator()
        {
            RuleFor(x => x.Username)
                .NotEmpty()
                .Length(3, 50)
                .Matches(@"^[a-zA-Z0-9_-]+$")
                .WithMessage("Username can only contain letters, numbers, underscores, and hyphens");

            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress()
                .MaximumLength(255);

            RuleFor(x => x.Password)
                .NotEmpty()
                .MinimumLength(8)
                .MaximumLength(100)
                .Matches(@"[A-Z]").WithMessage("Password must contain at least one uppercase letter")
                .Matches(@"[a-z]").WithMessage("Password must contain at least one lowercase letter")
                .Matches(@"[0-9]").WithMessage("Password must contain at least one number")
                .Matches(@"[^a-zA-Z0-9]").WithMessage("Password must contain at least one special character");
        }
    }
}
