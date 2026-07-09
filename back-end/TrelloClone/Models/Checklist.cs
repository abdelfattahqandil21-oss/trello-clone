namespace TrelloClone.Models;

public class Checklist
{
    public int Id { get; set; }
    public int CardId { get; set; }
    public string Title { get; set; } = string.Empty;
    public int Position { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public Card Card { get; set; } = null!;
    public ICollection<ChecklistItem> Items { get; set; } = new List<ChecklistItem>();
}
