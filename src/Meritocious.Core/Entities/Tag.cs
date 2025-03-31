using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meritocious.Core.Entities
{
    public class Tag : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }

        private readonly List<Post> _posts;
        public IReadOnlyCollection<Post> Posts => _posts.AsReadOnly();

        public Tag()
        {
            _posts = new List<Post>();
        }

        public static Tag Create(string name, string description = null)
        {
            return new Tag
            {
                Name = name,
                Description = description,
                CreatedAt = DateTime.UtcNow
            };
        }

        public void UpdateDescription(string newDescription)
        {
            Description = newDescription;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}