using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meritocious.Core.Entities
{
    public class MeritScoreType : BaseEntity
    {
        public string Name { get; private set; }
        public string Description { get; private set; }
        public decimal Weight { get; private set; }
        public bool IsActive { get; private set; }

        private readonly List<MeritScore> _scores = new();
        public IReadOnlyCollection<MeritScore> Scores => _scores.AsReadOnly();

        private MeritScoreType() { } // For EF Core

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
