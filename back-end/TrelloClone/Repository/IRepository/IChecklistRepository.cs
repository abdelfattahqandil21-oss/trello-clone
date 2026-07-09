namespace TrelloClone.Repository.IRepository;

public interface IChecklistRepository : IRepository<Checklist>
{
    Task<IEnumerable<Checklist>> GetByCardIdAsync(int cardId);
    Task<Checklist?> GetWithItemsAsync(int checklistId);
}
