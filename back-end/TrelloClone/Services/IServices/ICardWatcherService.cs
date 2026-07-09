namespace TrelloClone.Services.IServices;

public interface ICardWatcherService
{
    Task<IEnumerable<WatcherResponse>> GetByCardIdAsync(int cardId);
    Task<bool> IsWatchingAsync(int cardId, string userId);
    Task WatchAsync(int cardId, string userId);
    Task UnwatchAsync(int cardId, string userId);
}
