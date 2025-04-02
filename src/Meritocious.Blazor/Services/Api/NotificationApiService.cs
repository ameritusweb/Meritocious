using Meritocious.Common.DTOs.Notifications;

namespace Meritocious.Blazor.Services.Api;

public interface INotificationApiService
{
    Task<List<NotificationDto>> GetNotificationsAsync(bool unreadOnly = false);
    Task MarkAsReadAsync(Guid notificationId);
    Task MarkAllAsReadAsync();
    Task<int> GetUnreadCountAsync();
}

public class NotificationApiService : INotificationApiService
{
    private readonly ApiClient _apiClient;
    private readonly ILogger<NotificationApiService> _logger;

    public NotificationApiService(
        ApiClient apiClient,
        ILogger<NotificationApiService> logger)
    {
        _apiClient = apiClient;
        _logger = logger;
    }

    public async Task<List<NotificationDto>> GetNotificationsAsync(bool unreadOnly = false)
    {
        try
        {
            var endpoint = $"api/notifications?unreadOnly={unreadOnly}";
            return await _apiClient.GetAsync<List<NotificationDto>>(endpoint);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting notifications");
            throw;
        }
    }

    public async Task MarkAsReadAsync(Guid notificationId)
    {
        try
        {
            var endpoint = $"api/notifications/{notificationId}/read";
            await _apiClient.PostAsync<Unit>(endpoint, new {});
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking notification {NotificationId} as read", notificationId);
            throw;
        }
    }

    public async Task MarkAllAsReadAsync()
    {
        try
        {
            var endpoint = "api/notifications/mark-all-read";
            await _apiClient.PostAsync<Unit>(endpoint, new {});
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking all notifications as read");
            throw;
        }
    }

    public async Task<int> GetUnreadCountAsync()
    {
        try
        {
            var endpoint = "api/notifications/unread-count";
            return await _apiClient.GetAsync<int>(endpoint);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting unread notification count");
            throw;
        }
    }
}