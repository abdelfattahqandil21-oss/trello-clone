namespace TrelloClone.Services.IServices;

public interface INotificationService
{
    Task<IEnumerable<NotificationResponse>> GetByUserIdAsync(string userId);
    Task<IEnumerable<NotificationResponse>> GetUnreadByUserIdAsync(string userId);
    Task<int> GetUnreadCountAsync(string userId);
    Task MarkAsReadAsync(int notificationId);
    Task MarkAllAsReadAsync(string userId);
}
