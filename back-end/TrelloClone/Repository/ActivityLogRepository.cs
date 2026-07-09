namespace TrelloClone.Repository;

public class ActivityLogRepository : Repository<ActivityLog>, IActivityLogRepository
{
    public ActivityLogRepository(AppDpContext context) : base(context) { }

    public async Task<IEnumerable<ActivityLog>> GetByCardIdAsync(int cardId)
        => await _context.ActivityLogs
            .Where(al => al.CardId == cardId)
            .Include(al => al.ActorUser)
            .OrderByDescending(al => al.CreatedAt)
            .ToListAsync();

    public async Task<IEnumerable<ActivityLog>> GetByBoardIdAsync(int boardId)
        => await _context.ActivityLogs
            .Where(al => al.BoardId == boardId)
            .Include(al => al.ActorUser)
            .OrderByDescending(al => al.CreatedAt)
            .ToListAsync();
}
