using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meritocious.Core.Extensions
{
    public struct UlidId<T> : IEquatable<UlidId<T>>
    {
        private string? value;

        public string Value
        {
            get
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    value = Ulid.NewUlid().ToString();
                }

                return value.Length > 26 ? value[..26] : value;
            }
        }

        public UlidId(string? value)
        {
            this.value = string.IsNullOrWhiteSpace(value) ? null : value;
        }

        public static UlidId<T> New() => new(Ulid.NewUlid().ToString());

        public override string ToString() => Value;

        public override int GetHashCode() => Value.GetHashCode();

        public override bool Equals(object? obj)
            => obj is UlidId<T> other && Equals(other);

        public bool Equals(UlidId<T> other)
            => Value == other.Value;

        public static bool operator ==(UlidId<T> left, UlidId<T> right) => left.Equals(right);
        public static bool operator !=(UlidId<T> left, UlidId<T> right) => !left.Equals(right);

        public static implicit operator string(UlidId<T> id) => id.Value;
        public static implicit operator UlidId<T>(string value) => new(value);
    }
}