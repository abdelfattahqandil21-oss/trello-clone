namespace TrelloClone.Models;

public class Label
{
    public int Id { get; set; }
    public int BoardId { get; set; }
    public string? Name { get; set; }
    public string Color { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }

    public Board Board { get; set; } = null!;
    public ICollection<CardLabel> CardLabels { get; set; } = new List<CardLabel>();
}
