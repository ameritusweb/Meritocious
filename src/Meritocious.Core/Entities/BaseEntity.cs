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
        [Key]
        public UlidId<T> UlidId { get; set; } = UlidId<T>.New();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        [NotMapped]
        public string Id
        {
            get => UlidId.Value;
            set => UlidId = new UlidId<T>(value);
        }
    }
}