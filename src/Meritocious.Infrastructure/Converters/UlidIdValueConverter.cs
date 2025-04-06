using Meritocious.Core.Extensions;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meritocious.Infrastructure.Converters
{
    public class UlidIdValueConverter<T> : ValueConverter<UlidId<T>, string>
    {
        public UlidIdValueConverter()
            : base(
                id => id.Value,
                str => new UlidId<T>(str))
        {
        }
    }
}
