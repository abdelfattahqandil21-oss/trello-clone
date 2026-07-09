namespace TrelloClone.Services;

public class ChecklistService : IChecklistService
{
    private readonly IChecklistRepository _checklistRepo;

    public ChecklistService(IChecklistRepository checklistRepo)
    {
        _checklistRepo = checklistRepo;
    }

    public async Task<ChecklistResponse> GetByIdAsync(int id)
    {
        var checklist = await _checklistRepo.GetWithItemsAsync(id)
            ?? throw new KeyNotFoundException($"Checklist {id} not found");
        return MapToResponse(checklist);
    }

    public async Task<ChecklistDetailResponse> GetWithItemsAsync(int id)
    {
        var checklist = await _checklistRepo.GetWithItemsAsync(id)
            ?? throw new KeyNotFoundException($"Checklist {id} not found");

        return new ChecklistDetailResponse(
            checklist.Id, checklist.CardId, checklist.Title, checklist.CreatedAt,
            checklist.Items?.Select(i => new ChecklistItemResponse(
                i.Id, i.ChecklistId, i.Title, i.IsChecked, i.Position,
                i.AssignedUserId, i.AssignedUser?.DisplayName
            )).ToList() ?? []
        );
    }

    public async Task<IEnumerable<ChecklistResponse>> GetByCardIdAsync(int cardId)
    {
        var checklists = await _checklistRepo.GetByCardIdAsync(cardId);
        return checklists.Select(cl => MapToResponse(cl));
    }

    public async Task<ChecklistResponse> CreateAsync(int cardId, CreateChecklistRequest request)
    {
        var checklist = new Checklist
        {
            CardId = cardId,
            Title = request.Title,
            CreatedAt = DateTime.UtcNow
        };

        checklist = await _checklistRepo.AddAsync(checklist);
        await _checklistRepo.SaveChangesAsync();

        return MapToResponse(checklist);
    }

    public async Task<ChecklistResponse> UpdateAsync(int id, UpdateChecklistRequest request)
    {
        var checklist = await _checklistRepo.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Checklist {id} not found");

        if (request.Title is not null) checklist.Title = request.Title;

        _checklistRepo.Update(checklist);
        await _checklistRepo.SaveChangesAsync();

        return MapToResponse(checklist);
    }

    public async Task DeleteAsync(int id)
    {
        var checklist = await _checklistRepo.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Checklist {id} not found");

        _checklistRepo.Delete(checklist);
        await _checklistRepo.SaveChangesAsync();
    }

    private static ChecklistResponse MapToResponse(Checklist cl)
    {
        var items = cl.Items?.ToList() ?? [];
        return new ChecklistResponse(
            cl.Id, cl.CardId, cl.Title, cl.CreatedAt,
            items.Count, items.Count(i => i.IsChecked)
        );
    }
}
