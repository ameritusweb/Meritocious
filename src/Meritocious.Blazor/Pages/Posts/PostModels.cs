using Meritocious.Common.DTOs.Content;

namespace Meritocious.Blazor.Pages.Posts
{
    public class PostModel
    {
        public string Title { get; set; } = "";
        public string Content { get; set; } = "";
        public List<string> Tags { get; set; } = new();
        public VisibilityLevel Visibility { get; set; } = VisibilityLevel.Public;
        public bool IsFork { get; set; }
    }

    public class RestoreVersionModel
    {
        public string Reason { get; set; } = "";
    }

    public class ForkModel
    {
        public string Title { get; set; } = "";
        public string Content { get; set; } = "";
        public Guid TargetSubstackId { get; set; }
        public List<string> Tags { get; set; } = new();
        public PostRelationType RelationType { get; set; }
        public string Description { get; set; } = "";
        public List<PostDto> SourcePosts { get; set; } = new();
    }

    public enum PostRelationType
    {
        Extension,
        Alternative,
        Critique,
        Synthesis,
        Comparison
    }
}
