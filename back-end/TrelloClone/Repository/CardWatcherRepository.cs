namespace TrelloClone.Repository;

public class CardWatcherRepository : Repository<CardWatcher>, ICardWatcherRepository
{
    public CardWatcherRepository(AppDpContext context) : base(context) { }

    public async Task<IEnumerable<CardWatcher>> GetByCardIdAsync(int cardId)
        => await _context.CardWatchers
            .Where(cw => cw.CardId == cardId)
            .Include(cw => cw.User)
            .ToListAsync();

    public async Task<IEnumerable<CardWatcher>> GetByUserIdAsync(string userId)
        => await _context.CardWatchers
            .Where(cw => cw.UserId == userId)
            .Include(cw => cw.Card)
            .ToListAsync();

    public async Task<bool> IsWatchingAsync(int cardId, string userId)
        => await _context.CardWatchers
            .AnyAsync(cw => cw.CardId == cardId && cw.UserId == userId);
}
