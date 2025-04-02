using System.Text.Json.Serialization;

namespace Meritocious.Core.Features.Substacks.Models;

public class SubstackFeedPost
{
    [JsonPropertyName("id")]
    public string PostId { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; }

    [JsonPropertyName("subtitle")]
    public string Subtitle { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; }

    [JsonPropertyName("slug")]
    public string Slug { get; set; }

    [JsonPropertyName("type")]
    public string Type { get; set; }

    [JsonPropertyName("post_date")]
    public DateTime PostDate { get; set; }

    [JsonPropertyName("audience")]
    public string Audience { get; set; }

    [JsonPropertyName("canonical_url")]
    public string CanonicalUrl { get; set; }

    [JsonPropertyName("cover_image")]
    public string CoverImage { get; set; }

    [JsonPropertyName("podcast_url")]
    public string PodcastUrl { get; set; }

    [JsonPropertyName("podcast_duration")]
    public int? PodcastDuration { get; set; }

    [JsonPropertyName("write_comment_permissions")]
    public string WriteCommentPermissions { get; set; }

    [JsonPropertyName("should_send_email")]
    public bool ShouldSendEmail { get; set; }

    [JsonPropertyName("publication_id")]
    public string PublicationId { get; set; }

    [JsonPropertyName("wordcount")]
    public int WordCount { get; set; }

    [JsonPropertyName("video_upload_id")]
    public string VideoUploadId { get; set; }
}

public class SubstackPublication
{
    [JsonPropertyName("id")]
    public string PublicationId { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("subdomain")]
    public string Subdomain { get; set; }

    [JsonPropertyName("custom_domain")]
    public string CustomDomain { get; set; }

    [JsonPropertyName("author_name")]
    public string AuthorName { get; set; }

    [JsonPropertyName("logo_url")]
    public string LogoUrl { get; set; }

    [JsonPropertyName("cover_image_url")]
    public string CoverImageUrl { get; set; }

    [JsonPropertyName("twitter_handle")]
    public string TwitterHandle { get; set; }

    [JsonPropertyName("hide_intro_subtitle")]
    public bool HideIntroSubtitle { get; set; }

    [JsonPropertyName("theme_var_background_pop")]
    public string ThemeBackgroundPop { get; set; }

    [JsonPropertyName("theme_var_color_links")]
    public string ThemeColorLinks { get; set; }

    [JsonPropertyName("theme_var_color_heading")]
    public string ThemeColorHeading { get; set; }

    [JsonPropertyName("theme_var_font_family_heading")]
    public string ThemeFontFamilyHeading { get; set; }

    [JsonPropertyName("theme_var_font_family_body")]
    public string ThemeFontFamilyBody { get; set; }
}

public class SubstackFeedResponse
{
    [JsonPropertyName("posts")]
    public List<SubstackFeedPost> Posts { get; set; } = new();

    [JsonPropertyName("publication")]
    public SubstackPublication Publication { get; set; }
}

public class SubstackPostImportRequest
{
    public string PostUrl { get; set; }
    public string SubstackName { get; set; }
    public bool ImportAsRemix { get; set; }
    public string RemixNotes { get; set; }
}