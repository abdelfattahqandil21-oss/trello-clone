namespace TrelloClone.Repository;

public class LabelRepository : Repository<Label>, ILabelRepository
{
    public LabelRepository(AppDpContext context) : base(context) { }

    public async Task<IEnumerable<Label>> GetByBoardIdAsync(int boardId)
        => await _context.Labels
            .Where(l => l.BoardId == boardId)
            .ToListAsync();
}
