namespace TrelloClone.Repository.IRepository;

public interface IActivityLogRepository : IRepository<ActivityLog>
{
    Task<IEnumerable<ActivityLog>> GetByCardIdAsync(int cardId);
    Task<IEnumerable<ActivityLog>> GetByBoardIdAsync(int boardId);
}
