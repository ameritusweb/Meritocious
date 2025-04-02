namespace Meritocious.Common.DTOs.Substacks;

public class SubstackDto
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Subdomain { get; set; }
    public string CustomDomain { get; set; }
    public string AuthorName { get; set; }
    public string Description { get; set; }
    public string LogoUrl { get; set; }
    public string CoverImageUrl { get; set; }
    public string TwitterHandle { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime LastUpdated { get; set; }
    public int FollowerCount { get; set; }
    public int PostCount { get; set; }
    public int ImportedPostCount { get; set; }
    public bool IsVerified { get; set; }
    public SubstackMetricsDto Metrics { get; set; }
}