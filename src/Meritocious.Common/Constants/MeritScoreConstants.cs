using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meritocious.Common.Constants
{
    public static class MeritScoreConstants
    {
        public const decimal MinimumScore = 0.00m;
        public const decimal MaximumScore = 1.00m;

        public const decimal ClarityWeight = 0.25m;
        public const decimal NoveltyWeight = 0.25m;
        public const decimal ContributionWeight = 0.20m;
        public const decimal CivilityWeight = 0.15m;
        public const decimal RelevanceWeight = 0.15m;

        public static class Thresholds
        {
            public const decimal LowSignal = 0.29m;
            public const decimal BasicMerit = 0.59m;
            public const decimal SolidContribution = 0.79m;
            public const decimal HighValue = 0.89m;
            public const decimal Exceptional = 0.90m;
        }
    }
}