using System.Net.Http.Headers;

namespace Meritocious.Blazor.Services.Auth;

public class AuthHeaderHandler : DelegatingHandler
{
    private readonly ITokenManager _tokenManager;
    private readonly ILogger<AuthHeaderHandler> _logger;

    public AuthHeaderHandler(
        ITokenManager tokenManager,
        ILogger<AuthHeaderHandler> logger)
    {
        _tokenManager = tokenManager;
        _logger = logger;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var token = await _tokenManager.GetAccessTokenAsync();
        
        if (!string.IsNullOrEmpty(token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        try
        {
            var response = await base.SendAsync(request, cancellationToken);

            // Handle 401 Unauthorized responses
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                _logger.LogWarning("Received 401 response. Clearing authentication state.");
                await _tokenManager.ClearTokensAsync();
            }

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending HTTP request");
            throw;
        }
    }
}