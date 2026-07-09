namespace TrelloClone.Repository;

public class CardRepository : Repository<Card>, ICardRepository
{
    public CardRepository(AppDpContext context) : base(context) { }

    public async Task<IEnumerable<Card>> GetByListIdAsync(int listId)
        => await _context.Cards
            .Where(c => c.ListId == listId)
            .OrderBy(c => c.Position)
            .ToListAsync();

    public async Task<IEnumerable<Card>> GetByBoardIdAsync(int boardId)
        => await _context.Cards
            .Where(c => c.BoardId == boardId)
            .OrderBy(c => c.Position)
            .ToListAsync();

    public async Task<IEnumerable<Card>> GetByMemberIdAsync(string userId)
        => await _context.CardMembers
            .Where(cm => cm.UserId == userId)
            .Include(cm => cm.Card)
            .Select(cm => cm.Card)
            .ToListAsync();

    public async Task<Card?> GetWithDetailsAsync(int cardId)
        => await _context.Cards
            .Include(c => c.Members)
                .ThenInclude(cm => cm.User)
            .Include(c => c.Labels)
                .ThenInclude(cl => cl.Label)
            .Include(c => c.Checklists)
                .ThenInclude(cl => cl.Items)
            .Include(c => c.Comments)
                .ThenInclude(co => co.Author)
            .Include(c => c.Attachments)
            .Include(c => c.Watchers)
            .FirstOrDefaultAsync(c => c.Id == cardId);
}
