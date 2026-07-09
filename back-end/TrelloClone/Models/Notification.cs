namespace TrelloClone.Models;

public class Notification
{
    public int Id { get; set; }
    public string RecipientUserId { get; set; } = string.Empty;
    public NotificationType Type { get; set; }
    public string ReferenceEntityType { get; set; } = string.Empty;
    public int ReferenceEntityId { get; set; }
    public string Message { get; set; } = string.Empty;
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; }

    public AppUser RecipientUser { get; set; } = null!;
}
