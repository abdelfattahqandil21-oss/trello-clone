namespace TrelloClone.Models;

public class CardMember
{
    public int Id { get; set; }
    public int CardId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public DateTime AssignedAt { get; set; }
    public string AssignedByUserId { get; set; } = string.Empty;

    public Card Card { get; set; } = null!;
    public AppUser User { get; set; } = null!;
    public AppUser AssignedByUser { get; set; } = null!;
}
