namespace TrelloClone.DTOs;

public record UpdateUserProfileRequest(string? DisplayName, string? UserName, string? Email);
public record UpdateAvatarRequest(string AvatarUrl);
public record UserProfileResponse(
    string Id, string UserName, string Email, string? DisplayName,
    string AvatarUrl, bool IsEmailVerified, DateTime CreatedAt
);
public record UserSearchResult(string Id, string UserName, string Email, string? DisplayName, string AvatarUrl);
