namespace TrelloClone.Repository.IRepository;

public interface IBoardListRepository : IRepository<BoardList>
{
    Task<IEnumerable<BoardList>> GetByBoardIdAsync(int boardId);
    Task<BoardList?> GetWithCardsAsync(int listId);
}
