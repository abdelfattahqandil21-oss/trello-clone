namespace TrelloClone.Services.IServices;

public interface IWorkspaceService
{
    Task<WorkspaceResponse> GetByIdAsync(int id);
    Task<IEnumerable<WorkspaceResponse>> GetByOwnerIdAsync(string ownerId);
    Task<WorkspaceResponse> CreateAsync(CreateWorkspaceRequest request, string ownerId);
    Task<WorkspaceResponse> UpdateAsync(int id, UpdateWorkspaceRequest request);
    Task DeleteAsync(int id);
    Task<IEnumerable<WorkspaceMember>> GetMembersAsync(int workspaceId);
    Task AddMemberAsync(int workspaceId, string userId, WorkspaceRole role);
    Task RemoveMemberAsync(int workspaceId, string userId);
    Task UpdateMemberRoleAsync(int workspaceId, string userId, WorkspaceRole role);

    Task<IEnumerable<WorkspaceResponse>> GetAllWorkspacesAsync();
}
