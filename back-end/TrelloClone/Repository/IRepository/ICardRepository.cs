namespace TrelloClone.Repository.IRepository;

public interface ICardRepository : IRepository<Card>
{
    Task<IEnumerable<Card>> GetByListIdAsync(int listId);
    Task<IEnumerable<Card>> GetByBoardIdAsync(int boardId);
    Task<IEnumerable<Card>> GetByMemberIdAsync(string userId);
    Task<Card?> GetWithDetailsAsync(int cardId);
}
