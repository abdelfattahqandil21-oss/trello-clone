namespace TrelloClone.Services.IServices;

public interface IBoardListService
{
    Task<ListResponse> GetByIdAsync(int id);
    Task<ListWithCardsResponse> GetWithCardsAsync(int id);
    Task<IEnumerable<ListResponse>> GetByBoardIdAsync(int boardId);
    Task<ListResponse> CreateAsync(int boardId, CreateListRequest request);
    Task<ListResponse> UpdateAsync(int id, UpdateListRequest request);
    Task ReorderAsync(int id, decimal newPosition);
    Task ArchiveAsync(int id);
    Task DeleteAsync(int id);
}
