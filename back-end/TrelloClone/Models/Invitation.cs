namespace TrelloClone.Models;

public class Invitation
{
    public int Id { get; set; }
    public string InviterUserId { get; set; } = string.Empty;
    public string InviteeEmail { get; set; } = string.Empty;
    public InvitationTargetType TargetType { get; set; }
    public int TargetId { get; set; }
    public MemberRole Role { get; set; }
    public string Token { get; set; } = string.Empty;
    public InvitationStatus Status { get; set; }
    public DateTime ExpiresAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? RespondedAt { get; set; }

    public AppUser InviterUser { get; set; } = null!;
}
