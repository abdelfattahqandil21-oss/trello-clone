namespace TrelloClone.Services.IServices;

public interface IChecklistItemService
{
    Task<ChecklistItemResponse> GetByIdAsync(int id);
    Task<ChecklistItemResponse> CreateAsync(int checklistId, CreateChecklistItemRequest request);
    Task<ChecklistItemResponse> UpdateAsync(int id, UpdateChecklistItemRequest request);
    Task ToggleAsync(int id);
    Task DeleteAsync(int id);
}
