using FluentValidation;
using Meritocious.Common.DTOs.Auth;

namespace Meritocious.Core.Validation
{
    public class AuditLogQueryParamsValidator : AbstractValidator<AuditLogQueryParams>
    {
        public AuditLogQueryParamsValidator()
        {
            RuleFor(x => x.Page)
                .GreaterThan(0)
                .WithMessage("Page must be greater than 0");

            RuleFor(x => x.PageSize)
                .InclusiveBetween(1, 100)
                .WithMessage("Page size must be between 1 and 100");

            RuleFor(x => x.StartDate)
                .LessThanOrEqualTo(x => x.EndDate)
                .When(x => x.StartDate.HasValue && x.EndDate.HasValue)
                .WithMessage("Start date must be before or equal to end date");

            RuleFor(x => x.IpAddress)
                .Matches(@"^(?:[0-9]{1,3}\.){3}[0-9]{1,3}$")
                .When(x => !string.IsNullOrEmpty(x.IpAddress))
                .WithMessage("Invalid IP address format");

            RuleFor(x => x.Category)
                .MaximumLength(50)
                .When(x => !string.IsNullOrEmpty(x.Category))
                .WithMessage("Category cannot exceed 50 characters");

            RuleFor(x => x.Username)
                .MaximumLength(100)
                .When(x => !string.IsNullOrEmpty(x.Username))
                .WithMessage("Username cannot exceed 100 characters");
        }
    }

    public class SecurityEventValidator : AbstractValidator<SecurityAuditLogDto>
    {
        public SecurityEventValidator()
        {
            RuleFor(x => x.EventType)
                .NotEmpty()
                .MaximumLength(100)
                .WithMessage("Event type is required and cannot exceed 100 characters");

            RuleFor(x => x.Severity)
                .NotEmpty()
                .Must(x => new[] { "low", "medium", "high" }.Contains(x.ToLower()))
                .WithMessage("Severity must be one of: low, medium, high");

            RuleFor(x => x.Description)
                .NotEmpty()
                .MaximumLength(1000)
                .WithMessage("Description is required and cannot exceed 1000 characters");

            RuleFor(x => x.IpAddress)
                .NotEmpty()
                .Matches(@"^(?:[0-9]{1,3}\.){3}[0-9]{1,3}$")
                .WithMessage("Invalid IP address format");

            RuleFor(x => x.Username)
                .MaximumLength(100)
                .When(x => !string.IsNullOrEmpty(x.Username))
                .WithMessage("Username cannot exceed 100 characters");

            RuleFor(x => x.UserAgent)
                .MaximumLength(500)
                .When(x => !string.IsNullOrEmpty(x.UserAgent))
                .WithMessage("User agent cannot exceed 500 characters");
        }
    }

    public class LoginAttemptValidator : AbstractValidator<LoginAttemptDto>
    {
        public LoginAttemptValidator()
        {
            RuleFor(x => x.Username)
                .NotEmpty()
                .MaximumLength(100)
                .WithMessage("Username is required and cannot exceed 100 characters");

            RuleFor(x => x.IpAddress)
                .NotEmpty()
                .Matches(@"^(?:[0-9]{1,3}\.){3}[0-9]{1,3}$")
                .WithMessage("Invalid IP address format");

            RuleFor(x => x.UserAgent)
                .MaximumLength(500)
                .When(x => !string.IsNullOrEmpty(x.UserAgent))
                .WithMessage("User agent cannot exceed 500 characters");

            RuleFor(x => x.Location)
                .MaximumLength(200)
                .When(x => !string.IsNullOrEmpty(x.Location))
                .WithMessage("Location cannot exceed 200 characters");

            RuleFor(x => x.Device)
                .MaximumLength(200)
                .When(x => !string.IsNullOrEmpty(x.Device))
                .WithMessage("Device info cannot exceed 200 characters");
        }
    }

    public class ApiUsageLogValidator : AbstractValidator<ApiUsageLogDto>
    {
        public ApiUsageLogValidator()
        {
            RuleFor(x => x.Endpoint)
                .NotEmpty()
                .MaximumLength(500)
                .WithMessage("Endpoint is required and cannot exceed 500 characters");

            RuleFor(x => x.Method)
                .NotEmpty()
                .Must(x => new[] { "GET", "POST", "PUT", "DELETE", "PATCH" }.Contains(x.ToUpper()))
                .WithMessage("Invalid HTTP method");

            RuleFor(x => x.StatusCode)
                .InclusiveBetween(100, 599)
                .WithMessage("Invalid HTTP status code");

            RuleFor(x => x.IpAddress)
                .NotEmpty()
                .Matches(@"^(?:[0-9]{1,3}\.){3}[0-9]{1,3}$")
                .WithMessage("Invalid IP address format");

            RuleFor(x => x.DurationMs)
                .GreaterThan(0)
                .WithMessage("Duration must be greater than 0");

            RuleFor(x => x.ResponseSize)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Response size must be non-negative");
        }
    }
}