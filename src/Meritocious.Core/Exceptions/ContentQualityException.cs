using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meritocious.Core.Exceptions
{
    public class ContentQualityException : Exception
    {
        public ContentQualityException() : base("Content does not meet quality standards")
        {
        }

        public ContentQualityException(string message) : base(message)
        {
        }

        public ContentQualityException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}