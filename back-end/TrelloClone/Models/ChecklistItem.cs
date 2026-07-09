namespace TrelloClone.Models;

public class ChecklistItem
{
    public int Id { get; set; }
    public int ChecklistId { get; set; }
    public string Title { get; set; } = string.Empty;
    public bool IsChecked { get; set; }
    public int Position { get; set; }
    public string? AssignedUserId { get; set; }
    public DateTime? DueDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public Checklist Checklist { get; set; } = null!;
    public AppUser? AssignedUser { get; set; }
}
