namespace TrelloClone.Models;

public class Comment
{
    public int Id { get; set; }
    public int CardId { get; set; }
    public string AuthorId { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public bool IsEdited { get; set; }
    public DateTime? EditedAt { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime CreatedAt { get; set; }

    public Card Card { get; set; } = null!;
    public AppUser Author { get; set; } = null!;
}
