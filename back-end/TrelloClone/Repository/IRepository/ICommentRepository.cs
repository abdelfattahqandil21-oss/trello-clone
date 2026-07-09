namespace TrelloClone.Repository.IRepository;

public interface ICommentRepository : IRepository<Comment>
{
    Task<IEnumerable<Comment>> GetByCardIdAsync(int cardId);
}
