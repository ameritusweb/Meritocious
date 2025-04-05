using Microsoft.AspNetCore.Diagnostics;
using Microsoft.Extensions.Logging;
using System.Net;

namespace Meritocious.Web.ExceptionHandling
{
    public class SecurityExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<SecurityExceptionHandler> logger;

        public SecurityExceptionHandler(ILogger<SecurityExceptionHandler> logger)
        {
            this.logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(
            HttpContext context,
            Exception exception,
            CancellationToken cancellationToken)
        {
            var errorResponse = exception switch
            {
                UnauthorizedAccessException => new ErrorResponse
                {
                    StatusCode = (int)HttpStatusCode.Unauthorized,
                    Message = "You are not authorized to perform this action",
                    ErrorCode = "UNAUTHORIZED"
                },
                SecurityValidationException e => new ErrorResponse
                {
                    StatusCode = (int)HttpStatusCode.BadRequest,
                    Message = e.Message,
                    ErrorCode = "VALIDATION_ERROR",
                    Details = e.ValidationErrors
                },
                SecurityRateLimitException e => new ErrorResponse
                {
                    StatusCode = (int)HttpStatusCode.TooManyRequests,
                    Message = "Rate limit exceeded",
                    ErrorCode = "RATE_LIMIT_EXCEEDED",
                    RetryAfter = e.RetryAfter
                },
                _ => new ErrorResponse
                {
                    StatusCode = (int)HttpStatusCode.InternalServerError,
                    Message = "An unexpected error occurred",
                    ErrorCode = "INTERNAL_ERROR"
                }
            };

            var eventId = new EventId(exception.HResult);
            logger.LogError(
                eventId,
                exception,
                "Security error {ErrorCode}: {Message}",
                errorResponse.ErrorCode,
                exception.Message);

            context.Response.StatusCode = errorResponse.StatusCode;
            await context.Response.WriteAsJsonAsync(errorResponse, cancellationToken);
            return true;
        }
    }

    public class ErrorResponse
    {
        public int StatusCode { get; set; }
        public string Message { get; set; } = string.Empty;
        public string ErrorCode { get; set; } = string.Empty;
        public object? Details { get; set; }
        public TimeSpan? RetryAfter { get; set; }
    }

    public class SecurityValidationException : Exception
    {
        public Dictionary<string, string[]> ValidationErrors { get; }

        public SecurityValidationException(string message, Dictionary<string, string[]> validationErrors)
            : base(message)
        {
            ValidationErrors = validationErrors;
        }
    }

    public class SecurityRateLimitException : Exception
    {
        public TimeSpan RetryAfter { get; }

        public SecurityRateLimitException(string message, TimeSpan retryAfter)
            : base(message)
        {
            RetryAfter = retryAfter;
        }
    }
}