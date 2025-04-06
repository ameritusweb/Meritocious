using Meritocious.Core.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meritocious.Core.Entities
{
    public abstract class BaseEntity<T> : IUlidEntity
    {
        private const int UlidLength = 26;

        [Key]
        public UlidId<T> UlidId { get; set; } = UlidId<T>.New();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        [NotMapped]
        public string Id
        {
            get
            {
                return UlidId.Value;
            }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException("ID cannot be null or whitespace.", nameof(value));
                }

                // Auto-truncate if longer than a ULID
                if (value.Length > UlidLength)
                {
                    value = value.Substring(0, UlidLength);
                }

                UlidId = new UlidId<T>(value);
            }
        }
    }
}