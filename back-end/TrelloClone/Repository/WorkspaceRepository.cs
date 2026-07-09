namespace TrelloClone.Repository;

public class WorkspaceRepository : Repository<Workspace>, IWorkspaceRepository
{
    public WorkspaceRepository(AppDpContext context) : base(context) { }

    public async Task<IEnumerable<Workspace>> GetByOwnerIdAsync(string ownerId)
        => await _context.Workspaces
            .Where(w => w.OwnerId == ownerId)
            .OrderByDescending(w => w.CreatedAt)
            .ToListAsync();

    public async Task<Workspace?> GetWithMembersAsync(int workspaceId)
        => await _context.Workspaces
            .Include(w => w.Members)
            .ThenInclude(wm => wm.User)
            .FirstOrDefaultAsync(w => w.Id == workspaceId);

    public async Task<Workspace?> GetWithBoardsAsync(int workspaceId)
        => await _context.Workspaces
            .Include(w => w.Boards)
            .ThenInclude(b => b.Members)
            .FirstOrDefaultAsync(w => w.Id == workspaceId);
}
