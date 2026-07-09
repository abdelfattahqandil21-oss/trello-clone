namespace TrelloClone.Models;

public class CardLabel
{
    public int Id { get; set; }
    public int CardId { get; set; }
    public int LabelId { get; set; }
    public DateTime AppliedAt { get; set; } = DateTime.UtcNow;

    public Card Card { get; set; } = null!;
    public Label Label { get; set; } = null!;
}
