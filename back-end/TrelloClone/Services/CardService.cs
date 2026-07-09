namespace TrelloClone.Services;

public class CardService : ICardService
{
    private readonly ICardRepository _cardRepo;
    private readonly IRepository<CardMember> _memberRepo;

    public CardService(ICardRepository cardRepo, IRepository<CardMember> memberRepo)
    {
        _cardRepo = cardRepo;
        _memberRepo = memberRepo;
    }

    public async Task<CardResponse> GetByIdAsync(int id)
    {
        var card = await _cardRepo.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Card {id} not found");
        return MapToResponse(card);
    }

    public async Task<CardDetailResponse> GetWithDetailsAsync(int id)
    {
        var card = await _cardRepo.GetWithDetailsAsync(id)
            ?? throw new KeyNotFoundException($"Card {id} not found");

        return new CardDetailResponse(
            card.Id, card.ListId, card.BoardId, card.Title, card.Description,
            card.Position, card.CoverImageUrl, card.CoverColor,
            card.DueDate, card.DueDateTime, card.IsDueComplete, card.IsArchived,
            card.CreatedByUserId, card.CreatedAt, card.UpdatedAt,
            card.Members?.Select(m => new CardMemberResponse(
                m.UserId, m.User?.DisplayName ?? m.UserId,
                m.User?.Email ?? "", m.AssignedByUserId, m.AssignedAt
            )).ToList() ?? [],
            card.Labels?.Select(cl => new CardLabelResponse(
                cl.LabelId, cl.Label?.Name ?? "", cl.Label?.Color ?? "", cl.CardId
            )).ToList() ?? [],
            card.Checklists?.Select(cl => new ChecklistDetailResponse(
                cl.Id, cl.CardId, cl.Title, cl.CreatedAt,
                cl.Items?.Select(i => new ChecklistItemResponse(
                    i.Id, i.ChecklistId, i.Title, i.IsChecked, i.Position,
                    i.AssignedUserId, i.AssignedUser?.DisplayName
                )).ToList() ?? []
            )).ToList() ?? [],
            card.Comments?.Select(c => new CommentResponse(
                c.Id, c.CardId, c.Content, c.AuthorId,
                c.Author?.DisplayName ?? c.AuthorId,
                c.CreatedAt, c.EditedAt ?? c.CreatedAt, c.IsEdited
            )).ToList() ?? [],
            card.Attachments?.Select(a => new AttachmentResponse(
                a.Id, a.CardId, a.FileName, a.StorageUrl, a.FileSizeBytes, a.FileType,
                a.UploadedByUserId, a.UploadedByUser?.DisplayName ?? a.UploadedByUserId,
                a.CreatedAt
            )).ToList() ?? [],
            card.Watchers?.Select(w => new WatcherResponse(
                w.UserId, w.User?.DisplayName ?? w.UserId,
                w.User?.Email ?? "", w.WatchedAt
            )).ToList() ?? []
        );
    }

    public async Task<IEnumerable<CardResponse>> GetByListIdAsync(int listId)
    {
        var cards = await _cardRepo.GetByListIdAsync(listId);
        return cards.Select(MapToResponse);
    }

    public async Task<IEnumerable<CardResponse>> GetByBoardIdAsync(int boardId)
    {
        var cards = await _cardRepo.GetByBoardIdAsync(boardId);
        return cards.Select(MapToResponse);
    }

    public async Task<IEnumerable<CardResponse>> GetByMemberIdAsync(string userId)
    {
        var cards = await _cardRepo.GetByMemberIdAsync(userId);
        return cards.Select(MapToResponse);
    }

    public async Task<CardResponse> CreateAsync(int listId, int boardId, string userId, CreateCardRequest request)
    {
        var existing = await _cardRepo.GetByListIdAsync(listId);
        var maxPosition = existing.Any() ? existing.Max(c => c.Position) : 0;

        var card = new Card
        {
            ListId = listId,
            BoardId = boardId,
            Title = request.Title,
            Description = request.Description,
            Position = request.Position ?? maxPosition + 65536,
            DueDate = request.DueDate,
            DueDateTime = request.DueDateTime,
            CreatedByUserId = userId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        card = await _cardRepo.AddAsync(card);
        await _cardRepo.SaveChangesAsync();

        return MapToResponse(card);
    }

    public async Task<CardResponse> UpdateAsync(int id, UpdateCardRequest request)
    {
        var card = await _cardRepo.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Card {id} not found");

        if (request.Title is not null) card.Title = request.Title;
        if (request.Description is not null) card.Description = request.Description;
        if (request.Position.HasValue) card.Position = request.Position.Value;
        if (request.CoverImageUrl is not null) card.CoverImageUrl = request.CoverImageUrl;
        if (request.CoverColor is not null) card.CoverColor = request.CoverColor;
        if (request.DueDate.HasValue) card.DueDate = request.DueDate.Value;
        if (request.DueDateTime.HasValue) card.DueDateTime = request.DueDateTime.Value;
        if (request.IsDueComplete.HasValue) card.IsDueComplete = request.IsDueComplete.Value;

        card.UpdatedAt = DateTime.UtcNow;
        _cardRepo.Update(card);
        await _cardRepo.SaveChangesAsync();

        return MapToResponse(card);
    }

    public async Task<CardResponse> MoveAsync(int id, MoveCardRequest request)
    {
        var card = await _cardRepo.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Card {id} not found");

        card.ListId = request.NewListId;
        card.Position = request.NewPosition;
        card.UpdatedAt = DateTime.UtcNow;

        await _cardRepo.SaveChangesAsync();
        return MapToResponse(card);
    }

    public async Task ArchiveAsync(int id)
    {
        var card = await _cardRepo.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Card {id} not found");

        card.IsArchived = true;
        card.ArchivedAt = DateTime.UtcNow;
        await _cardRepo.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var card = await _cardRepo.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Card {id} not found");

        _cardRepo.Delete(card);
        await _cardRepo.SaveChangesAsync();
    }

    public async Task AssignMemberAsync(int cardId, string userId, string assignedByUserId)
    {
        var card = await _cardRepo.GetWithDetailsAsync(cardId)
            ?? throw new KeyNotFoundException($"Card {cardId} not found");

        if (card.Members.Any(m => m.UserId == userId))
            return;

        card.Members.Add(new CardMember
        {
            CardId = cardId,
            UserId = userId,
            AssignedByUserId = assignedByUserId,
            AssignedAt = DateTime.UtcNow
        });

        await _cardRepo.SaveChangesAsync();
    }

    public async Task RemoveMemberAsync(int cardId, string userId)
    {
        var card = await _cardRepo.GetWithDetailsAsync(cardId)
            ?? throw new KeyNotFoundException($"Card {cardId} not found");

        var member = card.Members.FirstOrDefault(m => m.UserId == userId);
        if (member is null) return;

        _memberRepo.Delete(member);
        await _memberRepo.SaveChangesAsync();
    }

    private static CardResponse MapToResponse(Card c) => new(
        c.Id, c.ListId, c.BoardId, c.Title, c.Description,
        c.Position, c.CoverImageUrl, c.CoverColor,
        c.DueDate, c.IsDueComplete, c.IsArchived,
        c.CreatedByUserId, c.CreatedAt,
        c.Members?.Count ?? 0, c.Comments?.Count ?? 0
    );
}
