using System.Net.Http;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Web;
using Microsoft.Extensions.Logging;
using Meritocious.Core.Features.Substacks.Models;

namespace Meritocious.Core.Features.Substacks.Services;

public interface ISubstackFeedService
{
    Task<SubstackFeedResponse> GetPublicationFeedAsync(string substackUrl);
    Task<string> GetPostContentAsync(string postUrl);
    Task<bool> ValidateSubstackUrlAsync(string url);
    string ExtractSubstackName(string url);
    string NormalizeSubstackUrl(string url);
}

public class SubstackFeedService : ISubstackFeedService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<SubstackFeedService> _logger;
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public SubstackFeedService(
        HttpClient httpClient,
        ILogger<SubstackFeedService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<SubstackFeedResponse> GetPublicationFeedAsync(string substackUrl)
    {
        try
        {
            var normalizedUrl = NormalizeSubstackUrl(substackUrl);
            var feedUrl = $"{normalizedUrl}/api/v1/archive?sort=new&search=&offset=0&limit=50";
            
            var response = await _httpClient.GetAsync(feedUrl);
            response.EnsureSuccessStatusCode();
            
            var content = await response.Content.ReadAsStringAsync();
            var feed = JsonSerializer.Deserialize<SubstackFeedResponse>(content, _jsonOptions);
            
            if (feed == null || feed.Posts == null)
                throw new InvalidOperationException("Failed to parse Substack feed");
                
            return feed;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching Substack feed from {Url}", substackUrl);
            throw;
        }
    }

    public async Task<string> GetPostContentAsync(string postUrl)
    {
        try
        {
            var response = await _httpClient.GetAsync(postUrl);
            response.EnsureSuccessStatusCode();
            
            var html = await response.Content.ReadAsStringAsync();
            return ExtractPostContent(html);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching Substack post content from {Url}", postUrl);
            throw;
        }
    }

    public async Task<bool> ValidateSubstackUrlAsync(string url)
    {
        try
        {
            if (!Uri.TryCreate(url, UriKind.Absolute, out var uri))
                return false;

            // Check if it's a Substack domain
            if (!uri.Host.EndsWith(".substack.com") && !IsCustomDomain(uri.Host))
                return false;

            var response = await _httpClient.GetAsync(url);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    public string ExtractSubstackName(string url)
    {
        if (Uri.TryCreate(url, UriKind.Absolute, out var uri))
        {
            if (uri.Host.EndsWith(".substack.com"))
            {
                return uri.Host.Replace(".substack.com", "");
            }
            return uri.Host;
        }
        throw new ArgumentException("Invalid Substack URL");
    }

    public string NormalizeSubstackUrl(string url)
    {
        if (!Uri.TryCreate(url, UriKind.Absolute, out var uri))
            throw new ArgumentException("Invalid URL format");

        var scheme = uri.Scheme;
        var host = uri.Host;

        // Remove any paths and query parameters
        return $"{scheme}://{host}";
    }

    private bool IsCustomDomain(string host)
    {
        // TODO: Implement more comprehensive custom domain validation
        // For now, we'll just check if it resolves and returns Substack content
        return true;
    }

    private string ExtractPostContent(string html)
    {
        // We'll need to parse the post content from the HTML
        // This is a basic implementation - you might want to use a proper HTML parser
        var match = Regex.Match(html, @"<div class=""post-content"".*?>(.*?)</div>", 
            RegexOptions.Singleline);
            
        if (match.Success)
        {
            var content = match.Groups[1].Value;
            content = Regex.Replace(content, @"<script.*?</script>", "", 
                RegexOptions.Singleline);
            return HttpUtility.HtmlDecode(content);
        }
        
        throw new InvalidOperationException("Could not extract post content");
    }
}