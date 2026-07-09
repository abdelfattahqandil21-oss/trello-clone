namespace TrelloClone.Repository;

public class AttachmentRepository : Repository<Attachment>, IAttachmentRepository
{
    public AttachmentRepository(AppDpContext context) : base(context) { }

    public async Task<IEnumerable<Attachment>> GetByCardIdAsync(int cardId)
        => await _context.Attachments
            .Where(a => a.CardId == cardId)
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync();
}
