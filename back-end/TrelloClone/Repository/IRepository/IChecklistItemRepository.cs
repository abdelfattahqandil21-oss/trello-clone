namespace TrelloClone.Repository.IRepository;

public interface IChecklistItemRepository : IRepository<ChecklistItem>
{
    Task<IEnumerable<ChecklistItem>> GetByChecklistIdAsync(int checklistId);
}
