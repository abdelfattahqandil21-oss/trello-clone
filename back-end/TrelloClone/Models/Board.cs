namespace TrelloClone.Models
{
    public class Board
    {
        public int Id { get; set; }

        public int WorkspaceId { get; set; }

        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public string? BackgroundColor { get; set; }

        public string? BackgroundImageUrl { get; set; }

        public Visibility Visibility { get; set; }

        public bool IsArchived { get; set; }

        public DateTime? ArchivedAt { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public Workspace Workspace { get; set; } = null!;

        public ICollection<BoardList> Lists { get; set; } = new List<BoardList>();

        public ICollection<BoardMember> Members { get; set; } = new List<BoardMember>();

        public ICollection<Label> Labels { get; set; } = new List<Label>();

        public ICollection<ActivityLog> ActivityLogs { get; set; } = new List<ActivityLog>();
    }
}
