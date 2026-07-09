namespace TrelloClone.Repository;

public class NotificationRepository : Repository<Notification>, INotificationRepository
{
    public NotificationRepository(AppDpContext context) : base(context) { }

    public async Task<IEnumerable<Notification>> GetByUserIdAsync(string userId)
        => await _context.Notifications
            .Where(n => n.RecipientUserId == userId)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();

    public async Task<IEnumerable<Notification>> GetUnreadByUserIdAsync(string userId)
        => await _context.Notifications
            .Where(n => n.RecipientUserId == userId && !n.IsRead)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();

    public async Task<int> GetUnreadCountAsync(string userId)
        => await _context.Notifications
            .CountAsync(n => n.RecipientUserId == userId && !n.IsRead);

    public async Task MarkAsReadAsync(int notificationId)
    {
        var notification = await _context.Notifications.FindAsync(notificationId);
        if (notification is not null)
        {
            notification.IsRead = true;
        }
    }

    public async Task MarkAllAsReadAsync(string userId)
    {
        var unread = await _context.Notifications
            .Where(n => n.RecipientUserId == userId && !n.IsRead)
            .ToListAsync();

        foreach (var notification in unread)
        {
            notification.IsRead = true;
        }
    }
}
