namespace TrelloClone.Repository.IRepository;

public interface ICardWatcherRepository : IRepository<CardWatcher>
{
    Task<IEnumerable<CardWatcher>> GetByCardIdAsync(int cardId);
    Task<IEnumerable<CardWatcher>> GetByUserIdAsync(string userId);
    Task<bool> IsWatchingAsync(int cardId, string userId);
}
