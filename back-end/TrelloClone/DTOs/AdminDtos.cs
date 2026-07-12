namespace TrelloClone.DTOs;

public record AdminUserResponse(
    string Id, string UserName, string Email, string? DisplayName,
    string AvatarUrl, bool IsEmailVerified, bool IsActive, DateTime CreatedAt
);

public record StatisticsResponse(int TotalUsers, int TotalBoards, int TotalWorkspaces, int TotalCards);
