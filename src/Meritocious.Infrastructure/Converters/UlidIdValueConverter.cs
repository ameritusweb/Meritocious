using Meritocious.Core.Extensions;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meritocious.Infrastructure.Converters
{
    public class UlidIdValueConverter<T> : ValueConverter<UlidId<T>, string>
    {
        public UlidIdValueConverter()
        : base(
            ulid => ConvertToString(ulid),
            str => ConvertFromString(str))
        {
        }

        private static string ConvertToString(UlidId<T> ulid)
        {
            Debug.WriteLine($"[TO DB] {typeof(T).Name}: {ulid}");
            return ulid.Value;
        }

        private static UlidId<T> ConvertFromString(string str)
        {
            Debug.WriteLine($"[FROM DB] {typeof(T).Name}: {str}");
            return new UlidId<T>(str);
        }
    }
}
