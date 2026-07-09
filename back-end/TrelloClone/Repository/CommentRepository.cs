namespace TrelloClone.Repository;

public class CommentRepository : Repository<Comment>, ICommentRepository
{
    public CommentRepository(AppDpContext context) : base(context) { }

    public async Task<IEnumerable<Comment>> GetByCardIdAsync(int cardId)
        => await _context.Comments
            .Where(c => c.CardId == cardId)
            .Include(c => c.Author)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();
}
