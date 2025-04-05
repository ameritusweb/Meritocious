using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meritocious.Core.Extensions
{
    public readonly record struct UlidId<T>(string Value)
    {
        public static UlidId<T> New() => new(Ulid.NewUlid().ToString());
        public override string ToString() => Value;

        public static implicit operator string(UlidId<T> id) => id.Value;
        public static implicit operator UlidId<T>(string value) => new(value);
    }
}
