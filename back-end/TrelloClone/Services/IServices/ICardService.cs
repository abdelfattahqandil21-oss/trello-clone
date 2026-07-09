namespace TrelloClone.Services.IServices;

public interface ICardService
{
    Task<CardResponse> GetByIdAsync(int id);
    Task<CardDetailResponse> GetWithDetailsAsync(int id);
    Task<IEnumerable<CardResponse>> GetByListIdAsync(int listId);
    Task<IEnumerable<CardResponse>> GetByBoardIdAsync(int boardId);
    Task<IEnumerable<CardResponse>> GetByMemberIdAsync(string userId);
    Task<CardResponse> CreateAsync(int listId, int boardId, string userId, CreateCardRequest request);
    Task<CardResponse> UpdateAsync(int id, UpdateCardRequest request);
    Task<CardResponse> MoveAsync(int id, MoveCardRequest request);
    Task ArchiveAsync(int id);
    Task DeleteAsync(int id);
    Task AssignMemberAsync(int cardId, string userId, string assignedByUserId);
    Task RemoveMemberAsync(int cardId, string userId);
}
