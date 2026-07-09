namespace TrelloClone.Services.IServices;

public interface IChecklistService
{
    Task<ChecklistResponse> GetByIdAsync(int id);
    Task<ChecklistDetailResponse> GetWithItemsAsync(int id);
    Task<IEnumerable<ChecklistResponse>> GetByCardIdAsync(int cardId);
    Task<ChecklistResponse> CreateAsync(int cardId, CreateChecklistRequest request);
    Task<ChecklistResponse> UpdateAsync(int id, UpdateChecklistRequest request);
    Task DeleteAsync(int id);
}
