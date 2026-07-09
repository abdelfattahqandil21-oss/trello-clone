namespace TrelloClone.Models;

public class CardWatcher
{
    public int Id { get; set; }
    public int CardId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public DateTime WatchedAt { get; set; }

    public Card Card { get; set; } = null!;
    public AppUser User { get; set; } = null!;
}
