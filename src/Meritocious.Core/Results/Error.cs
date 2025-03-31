using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meritocious.Core.Results
{
    public class Error
    {
        public string Code { get; }
        public string Message { get; }

        public Error(string code, string message)
        {
            Code = code;
            Message = message;
        }

        public static class Codes
        {
            public const string NotFound = "NotFound";
            public const string ValidationFailed = "ValidationFailed";
            public const string Unauthorized = "Unauthorized";
            public const string Forbidden = "Forbidden";
            public const string DuplicateResource = "DuplicateResource";
            public const string QualityStandardsNotMet = "QualityStandardsNotMet";
            public const string SystemError = "SystemError";
        }

        public static class Messages
        {
            public const string ResourceNotFound = "The requested resource was not found";
            public const string ValidationFailed = "One or more validation errors occurred";
            public const string Unauthorized = "Authentication is required";
            public const string Forbidden = "You do not have permission to perform this action";
            public const string DuplicateResource = "A resource with the same identifier already exists";
            public const string QualityStandardsNotMet = "The content does not meet quality standards";
            public const string SystemError = "An unexpected error occurred";
        }

        public static Error NotFound(string message = null) =>
            new Error(Codes.NotFound, message ?? Messages.ResourceNotFound);

        public static Error ValidationFailed(string message = null) =>
            new Error(Codes.ValidationFailed, message ?? Messages.ValidationFailed);

        public static Error Unauthorized(string message = null) =>
            new Error(Codes.Unauthorized, message ?? Messages.Unauthorized);

        public static Error Forbidden(string message = null) =>
            new Error(Codes.Forbidden, message ?? Messages.Forbidden);

        public static Error DuplicateResource(string message = null) =>
            new Error(Codes.DuplicateResource, message ?? Messages.DuplicateResource);

        public static Error QualityStandardsNotMet(string message = null) =>
            new Error(Codes.QualityStandardsNotMet, message ?? Messages.QualityStandardsNotMet);

        public static Error SystemError(string message = null) =>
            new Error(Codes.SystemError, message ?? Messages.SystemError);
    }
}