namespace TrelloClone.Services;

public class WorkspaceService : IWorkspaceService
{
    private readonly IWorkspaceRepository _workspaceRepo;
    private readonly IRepository<WorkspaceMember> _memberRepo;

    public WorkspaceService(IWorkspaceRepository workspaceRepo, IRepository<WorkspaceMember> memberRepo)
    {
        _workspaceRepo = workspaceRepo;
        _memberRepo = memberRepo;
    }

    public async Task<WorkspaceResponse> GetByIdAsync(int id)
    {
        var workspace = await _workspaceRepo.GetWithMembersAsync(id)
            ?? throw new KeyNotFoundException($"Workspace {id} not found");

        return MapToResponse(workspace);
    }

    public async Task<IEnumerable<WorkspaceResponse>> GetByOwnerIdAsync(string ownerId)
    {
        var workspaces = await _workspaceRepo.GetByOwnerIdAsync(ownerId);
        return workspaces.Select(MapToResponse);
    }

    public async Task<WorkspaceResponse> CreateAsync(CreateWorkspaceRequest request, string ownerId)
    {
        var workspace = new Workspace
        {
            Name = request.Name,
            Description = request.Description,
            Visibility = request.Visibility,
            OwnerId = ownerId,
            CreatedAt = DateTime.UtcNow
        };

        workspace = await _workspaceRepo.AddAsync(workspace);
        await _workspaceRepo.SaveChangesAsync();

        return MapToResponse(workspace);
    }

    public async Task<WorkspaceResponse> UpdateAsync(int id, UpdateWorkspaceRequest request)
    {
        var workspace = await _workspaceRepo.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Workspace {id} not found");

        if (request.Name is not null) workspace.Name = request.Name;
        if (request.Description is not null) workspace.Description = request.Description;
        if (request.Visibility.HasValue) workspace.Visibility = request.Visibility.Value;

        workspace.UpdatedAt = DateTime.UtcNow;
        _workspaceRepo.Update(workspace);
        await _workspaceRepo.SaveChangesAsync();

        return MapToResponse(workspace);
    }

    public async Task DeleteAsync(int id)
    {
        var workspace = await _workspaceRepo.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Workspace {id} not found");

        _workspaceRepo.Delete(workspace);
        await _workspaceRepo.SaveChangesAsync();
    }

    public async Task<IEnumerable<WorkspaceMember>> GetMembersAsync(int workspaceId)
    {
        var workspace = await _workspaceRepo.GetWithMembersAsync(workspaceId)
            ?? throw new KeyNotFoundException($"Workspace {workspaceId} not found");

        return workspace.Members;
    }

    public async Task AddMemberAsync(int workspaceId, string userId, WorkspaceRole role)
    {
        var workspace = await _workspaceRepo.GetByIdAsync(workspaceId)
            ?? throw new KeyNotFoundException($"Workspace {workspaceId} not found");

        workspace.Members.Add(new WorkspaceMember
        {
            WorkspaceId = workspaceId,
            UserId = userId,
            Role = role,
            JoinedAt = DateTime.UtcNow
        });

        await _workspaceRepo.SaveChangesAsync();
    }

    public async Task RemoveMemberAsync(int workspaceId, string userId)
    {
        var workspace = await _workspaceRepo.GetWithMembersAsync(workspaceId)
            ?? throw new KeyNotFoundException($"Workspace {workspaceId} not found");

        var member = workspace.Members.FirstOrDefault(m => m.UserId == userId)
            ?? throw new KeyNotFoundException($"Member {userId} not found in workspace");

        _memberRepo.Delete(member);
        await _memberRepo.SaveChangesAsync();
    }

    public async Task UpdateMemberRoleAsync(int workspaceId, string userId, WorkspaceRole role)
    {
        var workspace = await _workspaceRepo.GetWithMembersAsync(workspaceId)
            ?? throw new KeyNotFoundException($"Workspace {workspaceId} not found");

        var member = workspace.Members.FirstOrDefault(m => m.UserId == userId)
            ?? throw new KeyNotFoundException($"Member {userId} not found in workspace");

        member.Role = role;
        await _workspaceRepo.SaveChangesAsync();
    }

    private static WorkspaceResponse MapToResponse(Workspace w) => new(
        w.Id, w.Name, w.Description, w.Visibility,
        w.OwnerId, w.Owner?.DisplayName ?? w.OwnerId,
        w.CreatedAt, w.Members?.Count ?? 0
    );
}
