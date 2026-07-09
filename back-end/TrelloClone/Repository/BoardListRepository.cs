namespace TrelloClone.Repository;

public class BoardListRepository : Repository<BoardList>, IBoardListRepository
{
    public BoardListRepository(AppDpContext context) : base(context) { }

    public async Task<IEnumerable<BoardList>> GetByBoardIdAsync(int boardId)
        => await _context.CardLists
            .Where(l => l.BoardId == boardId)
            .OrderBy(l => l.Position)
            .ToListAsync();

    public async Task<BoardList?> GetWithCardsAsync(int listId)
        => await _context.CardLists
            .Include(l => l.Cards.OrderBy(c => c.Position))
            .FirstOrDefaultAsync(l => l.Id == listId);
}
