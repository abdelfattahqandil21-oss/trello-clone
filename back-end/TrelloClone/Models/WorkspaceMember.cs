namespace TrelloClone.Models;

public class WorkspaceMember
{
    public int Id { get; set; }
    public int WorkspaceId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public MemberRole Role { get; set; }
    public DateTime JoinedAt { get; set; }

    public Workspace Workspace { get; set; } = null!;
    public AppUser User { get; set; } = null!;
}
