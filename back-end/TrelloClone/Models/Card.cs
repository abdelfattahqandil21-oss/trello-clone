namespace TrelloClone.Models;

public class Card
{
    public int Id { get; set; }
    public int ListId { get; set; }
    public int BoardId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Position { get; set; }
    public string? CoverImageUrl { get; set; }
    public string? CoverColor { get; set; }
    public DateOnly? DueDate { get; set; }
    public DateTime? DueDateTime { get; set; }
    public bool IsDueComplete { get; set; }
    public bool IsArchived { get; set; }
    public DateTime? ArchivedAt { get; set; }
    public string CreatedByUserId { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public BoardList List { get; set; } = null!;
    public Board Board { get; set; } = null!;
    public AppUser CreatedByUser { get; set; } = null!;
    public ICollection<CardMember> Members { get; set; } = new List<CardMember>();
    public ICollection<CardLabel> Labels { get; set; } = new List<CardLabel>();
    public ICollection<Checklist> Checklists { get; set; } = new List<Checklist>();
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public ICollection<Attachment> Attachments { get; set; } = new List<Attachment>();
    public ICollection<ActivityLog> ActivityLogs { get; set; } = new List<ActivityLog>();
    public ICollection<CardWatcher> Watchers { get; set; } = new List<CardWatcher>();
}
