namespace TrelloClone.Repository;

public class BoardRepository : Repository<Board>, IBoardRepository
{
    public BoardRepository(AppDpContext context) : base(context) { }

    public async Task<IEnumerable<Board>> GetByWorkspaceIdAsync(int workspaceId)
        => await _context.Boards
            .Where(b => b.WorkspaceId == workspaceId)
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync();

    public async Task<IEnumerable<Board>> GetByMemberIdAsync(string userId)
        => await _context.BoardMembers
            .Where(bm => bm.UserId == userId)
            .Include(bm => bm.Board)
            .Select(bm => bm.Board)
            .ToListAsync();

    public async Task<Board?> GetWithListsAsync(int boardId)
        => await _context.Boards
            .Include(b => b.Lists)
            .FirstOrDefaultAsync(b => b.Id == boardId);

    public async Task<Board?> GetWithMembersAsync(int boardId)
        => await _context.Boards
            .Include(b => b.Members)
            .ThenInclude(bm => bm.User)
            .FirstOrDefaultAsync(b => b.Id == boardId);

    public async Task<Board?> GetFullAsync(int boardId)
        => await _context.Boards
            .Include(b => b.Lists.OrderBy(l => l.Position))
                .ThenInclude(l => l.Cards.OrderBy(c => c.Position))
            .Include(b => b.Members)
                .ThenInclude(bm => bm.User)
            .Include(b => b.Labels)
            .FirstOrDefaultAsync(b => b.Id == boardId);
}
