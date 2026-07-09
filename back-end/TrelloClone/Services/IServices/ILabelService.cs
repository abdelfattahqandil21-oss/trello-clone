namespace TrelloClone.Services.IServices;

public interface ILabelService
{
    Task<LabelResponse> GetByIdAsync(int id);
    Task<IEnumerable<LabelResponse>> GetByBoardIdAsync(int boardId);
    Task<LabelResponse> CreateAsync(int boardId, CreateLabelRequest request);
    Task<LabelResponse> UpdateAsync(int id, UpdateLabelRequest request);
    Task DeleteAsync(int id);
    Task AddToCardAsync(int cardId, int labelId);
    Task RemoveFromCardAsync(int cardId, int labelId);
}
