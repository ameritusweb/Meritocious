using Meritocious.Core.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meritocious.Infrastructure.Data.Resolvers
{
    public interface IMeritScoreTypeResolver
    {
        MeritScoreType GetByName(string name); // Or return null if not found
    }

    public class MeritScoreTypeResolver : IMeritScoreTypeResolver
    {
        private readonly Dictionary<string, MeritScoreType> cache;

        public MeritScoreTypeResolver(MeritociousDbContext context)
        {
            cache = context.MeritScoreTypes
                .AsNoTracking()
                .ToDictionary(x => x.Name, x => x);
        }

        public MeritScoreType GetByName(string name)
        {
            return cache.TryGetValue(name, out var type) ? type : null!;
        }
    }
}
