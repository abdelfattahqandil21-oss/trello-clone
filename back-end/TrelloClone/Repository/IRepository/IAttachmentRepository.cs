namespace TrelloClone.Repository.IRepository;

public interface IAttachmentRepository : IRepository<Attachment>
{
    Task<IEnumerable<Attachment>> GetByCardIdAsync(int cardId);
}
