namespace TrelloClone.Services;

public class ActivityLogService : IActivityLogService
{
    private readonly IActivityLogRepository _logRepo;

    public ActivityLogService(IActivityLogRepository logRepo)
    {
        _logRepo = logRepo;
    }

    public async Task<IEnumerable<ActivityLogResponse>> GetByBoardIdAsync(int boardId)
    {
        var logs = await _logRepo.GetByBoardIdAsync(boardId);
        return logs.Select(MapToResponse);
    }

    public async Task<IEnumerable<ActivityLogResponse>> GetByCardIdAsync(int cardId)
    {
        var logs = await _logRepo.GetByCardIdAsync(cardId);
        return logs.Select(MapToResponse);
    }

    private static ActivityLogResponse MapToResponse(ActivityLog al) => new(
        al.Id, al.BoardId, al.CardId, al.ActorUserId,
        al.ActorUser?.DisplayName ?? al.ActorUserId,
        al.ActionType.ToString(), al.EntityType.ToString(),
        al.EntityId, al.MetadataJson, al.CreatedAt
    );
}
