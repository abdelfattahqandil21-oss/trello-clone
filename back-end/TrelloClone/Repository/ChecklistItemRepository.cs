namespace TrelloClone.Repository;

public class ChecklistItemRepository : Repository<ChecklistItem>, IChecklistItemRepository
{
    public ChecklistItemRepository(AppDpContext context) : base(context) { }

    public async Task<IEnumerable<ChecklistItem>> GetByChecklistIdAsync(int checklistId)
        => await _context.ChecklistItems
            .Where(ci => ci.ChecklistId == checklistId)
            .OrderBy(ci => ci.Position)
            .ToListAsync();
}
