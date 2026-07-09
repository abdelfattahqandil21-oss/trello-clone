namespace TrelloClone.Models;

public class ActivityLog
{
    public int Id { get; set; }

    public Board Board { get; set; } = null!;
    public int BoardId { get; set; }

    public Card? Card { get; set; }
    public int? CardId { get; set; }
    
    public AppUser ActorUser { get; set; } = null!;
    public string ActorUserId { get; set; } = string.Empty;
    
    public ActionType ActionType { get; set; }
    public string EntityType { get; set; } = string.Empty;
    public int EntityId { get; set; }
    public string? MetadataJson { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

}
