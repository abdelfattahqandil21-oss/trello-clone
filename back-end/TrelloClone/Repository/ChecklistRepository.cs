namespace TrelloClone.Repository;

public class ChecklistRepository : Repository<Checklist>, IChecklistRepository
{
    public ChecklistRepository(AppDpContext context) : base(context) { }

    public async Task<IEnumerable<Checklist>> GetByCardIdAsync(int cardId)
        => await _context.Checklists
            .Where(cl => cl.CardId == cardId)
            .ToListAsync();

    public async Task<Checklist?> GetWithItemsAsync(int checklistId)
        => await _context.Checklists
            .Include(cl => cl.Items)
            .FirstOrDefaultAsync(cl => cl.Id == checklistId);
}
