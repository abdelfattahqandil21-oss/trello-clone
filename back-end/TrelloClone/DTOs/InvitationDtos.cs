namespace TrelloClone.DTOs;

public record CreateInvitationRequest(string InviteeEmail, InvitationTargetType TargetType, int TargetId, MemberRole Role);
public record InvitationResponse(int Id, string InviterUserId, string InviterName, string InviteeEmail, string TargetType, int TargetId, string Role, string Status, DateTime CreatedAt, DateTime ExpiresAt);
