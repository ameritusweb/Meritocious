using Meritocious.Common.Constants;
using Meritocious.Common.DTOs.Merit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meritocious.Common.Extensions
{
    public static class MeritScoreExtensions
    {
        public static decimal CalculateWeightedScore(this MeritScoreDto score)
        {
            return (score.ClarityScore * MeritScoreConstants.ClarityWeight) +
                   (score.NoveltyScore * MeritScoreConstants.NoveltyWeight) +
                   (score.ContributionScore * MeritScoreConstants.ContributionWeight) +
                   (score.CivilityScore * MeritScoreConstants.CivilityWeight) +
                   (score.RelevanceScore * MeritScoreConstants.RelevanceWeight);
        }

        public static string GetMeritLevel(this decimal score)
        {
            if (score >= MeritScoreConstants.Thresholds.Exceptional)
            {
                return "Exceptional";
            }

            if (score >= MeritScoreConstants.Thresholds.HighValue)
            {
                return "High Value";
            }

            if (score >= MeritScoreConstants.Thresholds.SolidContribution)
            {
                return "Solid";
            }

            if (score >= MeritScoreConstants.Thresholds.BasicMerit)
            {
                return "Basic";
            }

            return "Low Signal";
        }
    }
}