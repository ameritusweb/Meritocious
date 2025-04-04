using Meritocious.Core.Features.Recommendations.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Meritocious.Core.Entities;

public class Substack : BaseEntity
{
    [Required]
    public string Name { get; set; }

    [Required]
    public string Subdomain { get; set; }

    public string CustomDomain { get; set; }

    [Required]
    public string AuthorName { get; set; }

    public string Description { get; set; }

    public string LogoUrl { get; set; }

    public string CoverImageUrl { get; set; }

    public string TwitterHandle { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime LastUpdated { get; set; }

    public DateTime LastPostDate { get; set; }

    public int FollowerCount { get; set; }

    public int PostCount { get; set; }

    public int ImportedPostCount { get; set; }

    public int TotalRemixes { get; set; }

    public int TotalComments { get; set; }

    public int TotalViews { get; set; }

    public int UniqueViewers { get; set; }

    public int AvgPostLength { get; set; }

    public int AvgCommentLength { get; set; }

    public double AvgMeritScore { get; set; }

    public int PostsLastWeek { get; set; }

    public int PostsLastMonth { get; set; }

    public int EngagementRate { get; set; }

    public double GrowthRate { get; set; }

    public bool IsVerified { get; set; }

    // Navigation properties
    public virtual ICollection<User> Followers { get; set; } = new List<User>();
    public virtual ICollection<Post> Posts { get; set; } = new List<Post>();
    public virtual ICollection<ContentTopic> Topics { get; set; } = new List<ContentTopic>();
}