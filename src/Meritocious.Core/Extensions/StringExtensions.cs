using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meritocious.Core.Extensions
{
    public static class StringExtensions
    {
        public static bool HasValue(this string? input) => !string.IsNullOrWhiteSpace(input);

        public static bool HasValue<T>(this UlidId<T>? input) => !string.IsNullOrWhiteSpace(input);
    }
}
