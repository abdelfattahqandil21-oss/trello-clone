namespace TrelloClone.Services.IServices;

public interface IUserService
{
    Task<UserProfileResponse> GetProfileAsync(string userId);
    Task<UserProfileResponse> UpdateProfileAsync(string userId, UpdateUserProfileRequest request);
    Task<UserProfileResponse> UpdateAvatarAsync(string userId, UpdateAvatarRequest request);
    Task DeleteAccountAsync(string userId);
    Task<IEnumerable<UserSearchResult>> SearchAsync(string query, string currentUserId);

    Task<IEnumerable<AdminUserResponse>> GetAllUsersAsync();
    Task<AdminUserResponse> GetUserByIdAsync(string id);
    Task DeactivateUserAsync(string id);
    Task ActivateUserAsync(string id);
    Task HardDeleteUserAsync(string id);
}
