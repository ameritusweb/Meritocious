using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meritocious.Core.Entities
{
    public class MeritScoreType : BaseEntity<MeritScoreType>
    {
        public string Name { get; internal set; }
        public string Description { get; internal set; }
        public decimal Weight { get; internal set; }
        public bool IsActive { get; internal set; }

        private readonly List<MeritScore> scores = new();
        public IReadOnlyCollection<MeritScore> Scores => scores.AsReadOnly();

        internal MeritScoreType()
        {
        } // For EF Core

        public static MeritScoreType Create(string name, string description, decimal weight)
        {
            return new MeritScoreType
            {
                Name = name,
                Description = description,
                Weight = weight,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
        }

        public void UpdateWeight(decimal newWeight)
        {
            Weight = newWeight;
            UpdatedAt = DateTime.UtcNow;
        }

        public void SetActive(bool isActive)
        {
            IsActive = isActive;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
