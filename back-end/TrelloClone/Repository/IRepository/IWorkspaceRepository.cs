namespace TrelloClone.Repository.IRepository;

public interface IWorkspaceRepository : IRepository<Workspace>
{
    Task<IEnumerable<Workspace>> GetByOwnerIdAsync(string ownerId);
    Task<Workspace?> GetWithMembersAsync(int workspaceId);
    Task<Workspace?> GetWithBoardsAsync(int workspaceId);
}
