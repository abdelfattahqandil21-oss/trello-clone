namespace TrelloClone.Services.IServices;

public interface IActivityLogService
{
    Task<IEnumerable<ActivityLogResponse>> GetByBoardIdAsync(int boardId);
    Task<IEnumerable<ActivityLogResponse>> GetByCardIdAsync(int cardId);
}
