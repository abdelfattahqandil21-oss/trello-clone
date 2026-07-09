namespace TrelloClone.Services;

public class BoardListService : IBoardListService
{
    private readonly IBoardListRepository _listRepo;

    public BoardListService(IBoardListRepository listRepo)
    {
        _listRepo = listRepo;
    }

    public async Task<ListResponse> GetByIdAsync(int id)
    {
        var list = await _listRepo.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"List {id} not found");
        return MapToResponse(list);
    }

    public async Task<ListWithCardsResponse> GetWithCardsAsync(int id)
    {
        var list = await _listRepo.GetWithCardsAsync(id)
            ?? throw new KeyNotFoundException($"List {id} not found");

        return new ListWithCardsResponse(
            list.Id, list.BoardId, list.Name, list.Position,
            list.IsArchived, list.CreatedAt,
            list.Cards?.Select(c => new CardResponse(
                c.Id, c.ListId, c.BoardId, c.Title, c.Description,
                c.Position, c.CoverImageUrl, c.CoverColor,
                c.DueDate, c.IsDueComplete, c.IsArchived,
                c.CreatedByUserId, c.CreatedAt,
                c.Members?.Count ?? 0, c.Comments?.Count ?? 0
            )).ToList() ?? []
        );
    }

    public async Task<IEnumerable<ListResponse>> GetByBoardIdAsync(int boardId)
    {
        var lists = await _listRepo.GetByBoardIdAsync(boardId);
        return lists.Select(MapToResponse);
    }

    public async Task<ListResponse> CreateAsync(int boardId, CreateListRequest request)
    {
        var existing = await _listRepo.GetByBoardIdAsync(boardId);
        var maxPosition = existing.Any() ? existing.Max(l => l.Position) : 0;

        var list = new BoardList
        {
            BoardId = boardId,
            Name = request.Name,
            Position = maxPosition + 65536,
            CreatedAt = DateTime.UtcNow
        };

        list = await _listRepo.AddAsync(list);
        await _listRepo.SaveChangesAsync();

        return MapToResponse(list);
    }

    public async Task<ListResponse> UpdateAsync(int id, UpdateListRequest request)
    {
        var list = await _listRepo.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"List {id} not found");

        if (request.Name is not null) list.Name = request.Name;
        if (request.Position.HasValue) list.Position = request.Position.Value;

        list.UpdatedAt = DateTime.UtcNow;
        _listRepo.Update(list);
        await _listRepo.SaveChangesAsync();

        return MapToResponse(list);
    }

    public async Task ReorderAsync(int id, decimal newPosition)
    {
        var list = await _listRepo.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"List {id} not found");

        list.Position = newPosition;
        list.UpdatedAt = DateTime.UtcNow;
        await _listRepo.SaveChangesAsync();
    }

    public async Task ArchiveAsync(int id)
    {
        var list = await _listRepo.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"List {id} not found");

        list.IsArchived = true;
        list.ArchivedAt = DateTime.UtcNow;
        await _listRepo.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var list = await _listRepo.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"List {id} not found");

        _listRepo.Delete(list);
        await _listRepo.SaveChangesAsync();
    }

    private static ListResponse MapToResponse(BoardList l) => new(
        l.Id, l.BoardId, l.Name, l.Position,
        l.IsArchived, l.CreatedAt, l.Cards?.Count ?? 0
    );
}
