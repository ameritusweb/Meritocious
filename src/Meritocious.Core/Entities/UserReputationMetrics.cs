using Meritocious.Core.Entities;
using Meritocious.Core.Extensions;
using Meritocious.Core.Features.Reputation.Models;

public class UserReputationMetrics : BaseEntity<UserReputationMetrics>
{
    public UlidId<User> UserId { get; private set; }
    public User User { get; private set; }
    public decimal OverallMeritScore { get; private set; }
    public Dictionary<string, decimal> CategoryScores { get; private set; }
    public Dictionary<string, int> ContributionCounts { get; private set; }
    public Dictionary<string, decimal> TopicExpertise { get; private set; }
    public ReputationLevel Level { get; private set; }
    public int TotalContributions { get; private set; }
    public int PositiveInteractions { get; private set; }
    public int NegativeInteractions { get; private set; }
    public decimal ContentQualityAverage { get; private set; }
    public decimal CommunityImpact { get; private set; }
    public Dictionary<string, decimal> BadgeProgress { get; private set; }

    private UserReputationMetrics()
    {
        CategoryScores = new Dictionary<string, decimal>();
        ContributionCounts = new Dictionary<string, int>();
        TopicExpertise = new Dictionary<string, decimal>();
        BadgeProgress = new Dictionary<string, decimal>();
    }

    public static UserReputationMetrics Create(User user)
    {
        return new UserReputationMetrics
        {
            UserId = user.Id,
            User = user,
            OverallMeritScore = 0,
            Level = ReputationLevel.Newcomer,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void UpdateMetrics(
        decimal overallScore,
        Dictionary<string, decimal> categoryScores,
        Dictionary<string, int> contributionCounts,
        Dictionary<string, decimal> topicExpertise,
        int positiveInteractions,
        int negativeInteractions,
        decimal contentQualityAvg,
        decimal communityImpact)
    {
        OverallMeritScore = overallScore;
        CategoryScores = categoryScores;
        ContributionCounts = contributionCounts;
        TopicExpertise = topicExpertise;
        PositiveInteractions = positiveInteractions;
        NegativeInteractions = negativeInteractions;
        ContentQualityAverage = contentQualityAvg;
        CommunityImpact = communityImpact;
        TotalContributions = contributionCounts.Values.Sum();

        UpdateReputationLevel();
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateBadgeProgress(Dictionary<string, decimal> progress)
    {
        BadgeProgress = progress;
        UpdatedAt = DateTime.UtcNow;
    }

    private void UpdateReputationLevel()
    {
        Level = (OverallMeritScore, TotalContributions) switch
        {
            (>= 0.9m, >= 1000) => ReputationLevel.Expert,
            (>= 0.8m, >= 500) => ReputationLevel.Master,
            (>= 0.7m, >= 250) => ReputationLevel.Senior,
            (>= 0.6m, >= 100) => ReputationLevel.Established,
            (>= 0.5m, >= 50) => ReputationLevel.Rising,
            (>= 0.4m, >= 25) => ReputationLevel.Active,
            (>= 0.3m, >= 10) => ReputationLevel.Learning,
            _ => ReputationLevel.Newcomer
        };
    }
}