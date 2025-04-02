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
            _logger.LogInformation("Validating Substack URL: {Url}", url);

            if (!Uri.TryCreate(url, UriKind.Absolute, out var uri))
            {
                _logger.LogWarning("Invalid URL format: {Url}", url);
                return false;
            }

            // Check if it's a standard Substack domain
            if (uri.Host.EndsWith(".substack.com"))
            {
                var response = await _httpClient.GetAsync(url);
                var isValid = response.IsSuccessStatusCode;
                
                if (!isValid)
                {
                    _logger.LogWarning("Standard Substack URL not accessible: {Url}", url);
                }
                
                return isValid;
            }

            // If not a standard domain, validate as custom domain
            return await IsCustomDomain(uri.Host);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating Substack URL: {Url}", url);
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

    private async Task<bool> IsCustomDomain(string host)
    {
        try
        {
            // Construct the URL to check
            var url = $"https://{host}";
            
            // Make a request to the potential Substack site
            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Failed to access custom domain {Host}", host);
                return false;
            }

            var html = await response.Content.ReadAsStringAsync();

            // Check for Substack-specific indicators
            var isSubstack = false;
            isSubstack |= html.Contains("cdn.substack.com");
            isSubstack |= html.Contains("substackcdn.com");
            isSubstack |= html.Contains("substack-custom-domains");
            isSubstack |= response.Headers.Contains("x-substack-backend");

            // Check for Substack API endpoint
            if (isSubstack)
            {
                try
                {
                    var apiResponse = await _httpClient.GetAsync($"{url}/api/v1/archive");
                    isSubstack &= apiResponse.IsSuccessStatusCode;
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to validate Substack API endpoint for {Host}", host);
                    return false;
                }
            }

            if (!isSubstack)
            {
                _logger.LogWarning("Domain {Host} does not appear to be a valid Substack custom domain", host);
            }

            return isSubstack;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating custom domain {Host}", host);
            return false;
        }
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