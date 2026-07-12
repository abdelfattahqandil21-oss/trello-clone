using Microsoft.AspNetCore.Identity;

namespace TrelloClone.Services;

public class UserService : IUserService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IWorkspaceRepository _workspaceRepo;

    public UserService(UserManager<AppUser> userManager, IWorkspaceRepository workspaceRepo)
    {
        _userManager = userManager;
        _workspaceRepo = workspaceRepo;
    }

    public async Task<UserProfileResponse> GetProfileAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId)
            ?? throw new KeyNotFoundException("User not found");

        return MapToProfile(user);
    }

    public async Task<UserProfileResponse> UpdateProfileAsync(string userId, UpdateUserProfileRequest request)
    {
        var user = await _userManager.FindByIdAsync(userId)
            ?? throw new KeyNotFoundException("User not found");

        if (request.UserName is not null)
        {
            var existingUser = await _userManager.FindByNameAsync(request.UserName);
            if (existingUser is not null && existingUser.Id != userId)
                throw new InvalidOperationException("Username is already taken");
            user.UserName = request.UserName;
        }

        if (request.Email is not null)
        {
            var existingUser = await _userManager.FindByEmailAsync(request.Email);
            if (existingUser is not null && existingUser.Id != userId)
                throw new InvalidOperationException("Email is already in use");
            user.Email = request.Email;
            user.EmailConfirmed = false;
        }

        if (request.DisplayName is not null)
            user.DisplayName = request.DisplayName;

        user.UpdatedAt = DateTime.UtcNow;

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
            throw new InvalidOperationException(
                string.Join("; ", result.Errors.Select(e => e.Description))
            );

        return MapToProfile(user);
    }

    public async Task<UserProfileResponse> UpdateAvatarAsync(string userId, UpdateAvatarRequest request)
    {
        var user = await _userManager.FindByIdAsync(userId)
            ?? throw new KeyNotFoundException("User not found");

        user.AvatarUrl = request.AvatarUrl;
        user.UpdatedAt = DateTime.UtcNow;

        await _userManager.UpdateAsync(user);

        return MapToProfile(user);
    }

    public async Task DeleteAccountAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId)
            ?? throw new KeyNotFoundException("User not found");

        user.IsActive = false;
        user.UpdatedAt = DateTime.UtcNow;

        await _userManager.UpdateAsync(user);
    }

    public async Task<IEnumerable<UserSearchResult>> SearchAsync(string query, string currentUserId)
    {
        var workspaces = await _workspaceRepo.GetByOwnerIdAsync(currentUserId);
        var workspaceIds = workspaces.Select(w => w.Id).ToList();

        var workspaceMemberships = new List<WorkspaceMember>();
        foreach (var wsId in workspaceIds)
        {
            var ws = await _workspaceRepo.GetWithMembersAsync(wsId);
            if (ws?.Members is not null)
                workspaceMemberships.AddRange(ws.Members);
        }

        var memberUserIds = workspaceMemberships
            .Select(m => m.UserId)
            .Distinct()
            .ToHashSet();
        memberUserIds.Add(currentUserId);

        var users = _userManager.Users
            .Where(u => memberUserIds.Contains(u.Id))
            .Where(u => u.UserName!.Contains(query) || (u.Email != null && u.Email.Contains(query)) || (u.DisplayName != null && u.DisplayName.Contains(query)))
            .Select(u => new UserSearchResult(
                u.Id, u.UserName!, u.Email!, u.DisplayName, u.AvatarUrl
            ))
            .ToList();

        return users;
    }

    public async Task<IEnumerable<AdminUserResponse>> GetAllUsersAsync()
    {
        var users = await _userManager.Users
            .OrderByDescending(u => u.CreatedAt)
            .ToListAsync();

        return users.Select(u => new AdminUserResponse(
            u.Id, u.UserName!, u.Email!, u.DisplayName,
            u.AvatarUrl, u.IsEmailVerified, u.IsActive, u.CreatedAt
        ));
    }

    public async Task<AdminUserResponse> GetUserByIdAsync(string id)
    {
        var user = await _userManager.FindByIdAsync(id)
            ?? throw new KeyNotFoundException("User not found");

        return new AdminUserResponse(
            user.Id, user.UserName!, user.Email!, user.DisplayName,
            user.AvatarUrl, user.IsEmailVerified, user.IsActive, user.CreatedAt
        );
    }

    public async Task DeactivateUserAsync(string id)
    {
        var user = await _userManager.FindByIdAsync(id)
            ?? throw new KeyNotFoundException("User not found");

        user.IsActive = false;
        user.UpdatedAt = DateTime.UtcNow;

        await _userManager.UpdateAsync(user);
    }

    public async Task ActivateUserAsync(string id)
    {
        var user = await _userManager.FindByIdAsync(id)
            ?? throw new KeyNotFoundException("User not found");

        user.IsActive = true;
        user.UpdatedAt = DateTime.UtcNow;

        await _userManager.UpdateAsync(user);
    }

    public async Task HardDeleteUserAsync(string id)
    {
        var user = await _userManager.FindByIdAsync(id)
            ?? throw new KeyNotFoundException("User not found");

        var result = await _userManager.DeleteAsync(user);
        if (!result.Succeeded)
            throw new InvalidOperationException(
                string.Join("; ", result.Errors.Select(e => e.Description))
            );
    }

    private static UserProfileResponse MapToProfile(AppUser user) => new(
        user.Id, user.UserName!, user.Email!, user.DisplayName,
        user.AvatarUrl, user.IsEmailVerified, user.CreatedAt
    );
}
