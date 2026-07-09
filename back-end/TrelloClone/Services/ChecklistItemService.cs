namespace TrelloClone.Services;

public class ChecklistItemService : IChecklistItemService
{
    private readonly IChecklistItemRepository _itemRepo;

    public ChecklistItemService(IChecklistItemRepository itemRepo)
    {
        _itemRepo = itemRepo;
    }

    public async Task<ChecklistItemResponse> GetByIdAsync(int id)
    {
        var item = await _itemRepo.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Checklist item {id} not found");
        return MapToResponse(item);
    }

    public async Task<ChecklistItemResponse> CreateAsync(int checklistId, CreateChecklistItemRequest request)
    {
        var existing = await _itemRepo.GetByChecklistIdAsync(checklistId);
        var maxPosition = existing.Any() ? existing.Max(i => i.Position) : 0;

        var item = new ChecklistItem
        {
            ChecklistId = checklistId,
            Title = request.Title,
            Position = maxPosition + 1,
            IsChecked = false
        };

        item = await _itemRepo.AddAsync(item);
        await _itemRepo.SaveChangesAsync();

        return MapToResponse(item);
    }

    public async Task<ChecklistItemResponse> UpdateAsync(int id, UpdateChecklistItemRequest request)
    {
        var item = await _itemRepo.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Checklist item {id} not found");

        if (request.Title is not null) item.Title = request.Title;
        if (request.IsChecked.HasValue) item.IsChecked = request.IsChecked.Value;
        if (request.AssignedUserId is not null) item.AssignedUserId = request.AssignedUserId;

        _itemRepo.Update(item);
        await _itemRepo.SaveChangesAsync();

        return MapToResponse(item);
    }

    public async Task ToggleAsync(int id)
    {
        var item = await _itemRepo.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Checklist item {id} not found");

        item.IsChecked = !item.IsChecked;
        await _itemRepo.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var item = await _itemRepo.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Checklist item {id} not found");

        _itemRepo.Delete(item);
        await _itemRepo.SaveChangesAsync();
    }

    private static ChecklistItemResponse MapToResponse(ChecklistItem i) => new(
        i.Id, i.ChecklistId, i.Title, i.IsChecked, i.Position,
        i.AssignedUserId, i.AssignedUser?.DisplayName
    );
}
