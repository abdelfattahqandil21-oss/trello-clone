namespace TrelloClone.Services;

public class CardWatcherService : ICardWatcherService
{
    private readonly ICardWatcherRepository _watcherRepo;

    public CardWatcherService(ICardWatcherRepository watcherRepo)
    {
        _watcherRepo = watcherRepo;
    }

    public async Task<IEnumerable<WatcherResponse>> GetByCardIdAsync(int cardId)
    {
        var watchers = await _watcherRepo.GetByCardIdAsync(cardId);
        return watchers.Select(w => new WatcherResponse(
            w.UserId, w.User?.DisplayName ?? w.UserId,
            w.User?.Email ?? "", w.WatchedAt
        ));
    }

    public async Task<bool> IsWatchingAsync(int cardId, string userId)
        => await _watcherRepo.IsWatchingAsync(cardId, userId);

    public async Task WatchAsync(int cardId, string userId)
    {
        if (await _watcherRepo.IsWatchingAsync(cardId, userId))
            return;

        var watcher = new CardWatcher
        {
            CardId = cardId,
            UserId = userId,
            WatchedAt = DateTime.UtcNow
        };

        await _watcherRepo.AddAsync(watcher);
        await _watcherRepo.SaveChangesAsync();
    }

    public async Task UnwatchAsync(int cardId, string userId)
    {
        var watchers = await _watcherRepo.GetByCardIdAsync(cardId);
        var watcher = watchers.FirstOrDefault(w => w.UserId == userId);
        if (watcher is null) return;

        _watcherRepo.Delete(watcher);
        await _watcherRepo.SaveChangesAsync();
    }
}
