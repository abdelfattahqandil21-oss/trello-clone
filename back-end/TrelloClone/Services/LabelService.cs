namespace TrelloClone.Services;

public class LabelService : ILabelService
{
    private readonly ILabelRepository _labelRepo;
    private readonly ICardRepository _cardRepo;
    private readonly IRepository<CardLabel> _cardLabelRepo;

    public LabelService(ILabelRepository labelRepo, ICardRepository cardRepo, IRepository<CardLabel> cardLabelRepo)
    {
        _labelRepo = labelRepo;
        _cardRepo = cardRepo;
        _cardLabelRepo = cardLabelRepo;
    }

    public async Task<LabelResponse> GetByIdAsync(int id)
    {
        var label = await _labelRepo.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Label {id} not found");
        return MapToResponse(label);
    }

    public async Task<IEnumerable<LabelResponse>> GetByBoardIdAsync(int boardId)
    {
        var labels = await _labelRepo.GetByBoardIdAsync(boardId);
        return labels.Select(MapToResponse);
    }

    public async Task<LabelResponse> CreateAsync(int boardId, CreateLabelRequest request)
    {
        var label = new Label
        {
            BoardId = boardId,
            Name = request.Name,
            Color = request.Color
        };

        label = await _labelRepo.AddAsync(label);
        await _labelRepo.SaveChangesAsync();

        return MapToResponse(label);
    }

    public async Task<LabelResponse> UpdateAsync(int id, UpdateLabelRequest request)
    {
        var label = await _labelRepo.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Label {id} not found");

        if (request.Name is not null) label.Name = request.Name;
        if (request.Color is not null) label.Color = request.Color;

        _labelRepo.Update(label);
        await _labelRepo.SaveChangesAsync();

        return MapToResponse(label);
    }

    public async Task DeleteAsync(int id)
    {
        var label = await _labelRepo.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Label {id} not found");

        _labelRepo.Delete(label);
        await _labelRepo.SaveChangesAsync();
    }

    public async Task AddToCardAsync(int cardId, int labelId)
    {
        var card = await _cardRepo.GetWithDetailsAsync(cardId)
            ?? throw new KeyNotFoundException($"Card {cardId} not found");

        if (card.Labels.Any(cl => cl.LabelId == labelId))
            return;

        card.Labels.Add(new CardLabel
        {
            CardId = cardId,
            LabelId = labelId
        });

        await _cardRepo.SaveChangesAsync();
    }

    public async Task RemoveFromCardAsync(int cardId, int labelId)
    {
        var card = await _cardRepo.GetWithDetailsAsync(cardId)
            ?? throw new KeyNotFoundException($"Card {cardId} not found");

        var cardLabel = card.Labels.FirstOrDefault(cl => cl.LabelId == labelId);
        if (cardLabel is null) return;

        _cardLabelRepo.Delete(cardLabel);
        await _cardLabelRepo.SaveChangesAsync();
    }

    private static LabelResponse MapToResponse(Label l) => new(l.Id, l.BoardId, l.Name ?? "", l.Color);
}
