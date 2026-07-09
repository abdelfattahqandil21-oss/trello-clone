namespace TrelloClone.Services.IServices;

public interface ICommentService
{
    Task<CommentResponse> GetByIdAsync(int id);
    Task<IEnumerable<CommentResponse>> GetByCardIdAsync(int cardId);
    Task<CommentResponse> CreateAsync(int cardId, string authorId, CreateCommentRequest request);
    Task<CommentResponse> UpdateAsync(int id, string userId, UpdateCommentRequest request);
    Task SoftDeleteAsync(int id, string userId);
}
