namespace TrelloClone.Repository.IRepository;

public interface IBoardRepository : IRepository<Board>
{
    Task<IEnumerable<Board>> GetByWorkspaceIdAsync(int workspaceId);
    Task<IEnumerable<Board>> GetByMemberIdAsync(string userId);
    Task<Board?> GetWithListsAsync(int boardId);
    Task<Board?> GetWithMembersAsync(int boardId);
    Task<Board?> GetFullAsync(int boardId);
}
