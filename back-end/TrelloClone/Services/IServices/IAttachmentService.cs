namespace TrelloClone.Services.IServices;

public interface IAttachmentService
{
    Task<AttachmentResponse> GetByIdAsync(int id);
    Task<IEnumerable<AttachmentResponse>> GetByCardIdAsync(int cardId);
    Task<AttachmentResponse> CreateAsync(int cardId, string userId, CreateAttachmentRequest request);
    Task DeleteAsync(int id, string userId);
}
