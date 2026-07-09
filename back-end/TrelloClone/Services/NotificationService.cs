namespace TrelloClone.Services;

public class NotificationService : INotificationService
{
    private readonly INotificationRepository _notificationRepo;

    public NotificationService(INotificationRepository notificationRepo)
    {
        _notificationRepo = notificationRepo;
    }

    public async Task<IEnumerable<NotificationResponse>> GetByUserIdAsync(string userId)
    {
        var notifications = await _notificationRepo.GetByUserIdAsync(userId);
        return notifications.Select(MapToResponse);
    }

    public async Task<IEnumerable<NotificationResponse>> GetUnreadByUserIdAsync(string userId)
    {
        var notifications = await _notificationRepo.GetUnreadByUserIdAsync(userId);
        return notifications.Select(MapToResponse);
    }

    public async Task<int> GetUnreadCountAsync(string userId)
        => await _notificationRepo.GetUnreadCountAsync(userId);

    public async Task MarkAsReadAsync(int notificationId)
        => await _notificationRepo.MarkAsReadAsync(notificationId);

    public async Task MarkAllAsReadAsync(string userId)
        => await _notificationRepo.MarkAllAsReadAsync(userId);

    private static NotificationResponse MapToResponse(Notification n) => new(
        n.Id, n.Type.ToString(), n.ReferenceEntityType,
        n.ReferenceEntityId, n.Message, n.IsRead, n.CreatedAt
    );
}
