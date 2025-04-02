namespace Meritocious.Common.DTOs.Substacks;

public class SubstackMetricsDto
{
    public int TotalPosts { get; set; }
    public int TotalImportedPosts { get; set; }
    public int TotalRemixes { get; set; }
    public int TotalComments { get; set; }
    public int TotalViews { get; set; }
    public int UniqueViewers { get; set; }
    public int AvgPostLength { get; set; }
    public int AvgCommentLength { get; set; }
    public double AvgMeritScore { get; set; }
    public DateTime LastPostDate { get; set; }
    public int PostsLastWeek { get; set; }
    public int PostsLastMonth { get; set; }
    public int EngagementRate { get; set; }
    public double GrowthRate { get; set; }
}