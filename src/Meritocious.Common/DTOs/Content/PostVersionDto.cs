namespace Meritocious.Common.DTOs.Content
{
    public class PostVersionDto
    {        
        public Guid Id { get; set; }
        public Guid PostId { get; set; }
        public int VersionNumber { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public List<string> Tags { get; set; } = new();
        public string ChangeDescription { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public decimal MeritScore { get; set; }
        
        // For comparing versions
        public Dictionary<string, string> Changes { get; set; } = new();
        public int AddedLines { get; set; }
        public int RemovedLines { get; set; }
        public int ModifiedLines { get; set; }
        
        // Navigation properties
        public Guid? ParentVersionId { get; set; }
        public Guid? NextVersionId { get; set; }
    }

    public class PostVersionComparisonDto
    {        
        public PostVersionDto OldVersion { get; set; } = null!;
        public PostVersionDto NewVersion { get; set; } = null!;
        public List<VersionDiffDto> Differences { get; set; } = new();
        
        public class VersionDiffDto
        {            
            public string Field { get; set; } = string.Empty;
            public string OldValue { get; set; } = string.Empty;
            public string NewValue { get; set; } = string.Empty;
            public string DiffType { get; set; } = string.Empty; // Added, Removed, Modified
            public Dictionary<string, object> Metadata { get; set; } = new();
        }
    }
}