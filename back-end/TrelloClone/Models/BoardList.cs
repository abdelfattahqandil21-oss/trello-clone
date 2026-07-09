namespace TrelloClone.Models;

public class BoardList
{
    public int Id { get; set; }
    public int BoardId { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Position { get; set; }
    public bool IsArchived { get; set; }
    public DateTime? ArchivedAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; }

    public Board Board { get; set; } = null!;
    public ICollection<Card> Cards { get; set; } = new List<Card>();
}
