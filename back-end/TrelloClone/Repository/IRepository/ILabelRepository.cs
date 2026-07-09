namespace TrelloClone.Repository.IRepository;

public interface ILabelRepository : IRepository<Label>
{
    Task<IEnumerable<Label>> GetByBoardIdAsync(int boardId);
}
