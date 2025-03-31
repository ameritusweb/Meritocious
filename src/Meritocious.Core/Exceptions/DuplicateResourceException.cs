using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meritocious.Core.Exceptions
{
    public class DuplicateResourceException : Exception
    {
        public DuplicateResourceException() : base("Resource already exists")
        {
        }

        public DuplicateResourceException(string message) : base(message)
        {
        }

        public DuplicateResourceException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}