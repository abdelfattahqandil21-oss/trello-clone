namespace TrelloClone.Services;

public class AttachmentService : IAttachmentService
{
    private readonly IAttachmentRepository _attachmentRepo;

    public AttachmentService(IAttachmentRepository attachmentRepo)
    {
        _attachmentRepo = attachmentRepo;
    }

    public async Task<AttachmentResponse> GetByIdAsync(int id)
    {
        var attachment = await _attachmentRepo.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Attachment {id} not found");
        return MapToResponse(attachment);
    }

    public async Task<IEnumerable<AttachmentResponse>> GetByCardIdAsync(int cardId)
    {
        var attachments = await _attachmentRepo.GetByCardIdAsync(cardId);
        return attachments.Select(MapToResponse);
    }

    public async Task<AttachmentResponse> CreateAsync(int cardId, string userId, CreateAttachmentRequest request)
    {
        var attachment = new Attachment
        {
            CardId = cardId,
            UploadedByUserId = userId,
            FileName = request.FileName,
            StorageUrl = request.StorageUrl,
            FileSizeBytes = request.FileSizeBytes,
            FileType = request.FileType,
            CreatedAt = DateTime.UtcNow
        };

        attachment = await _attachmentRepo.AddAsync(attachment);
        await _attachmentRepo.SaveChangesAsync();

        return MapToResponse(attachment);
    }

    public async Task DeleteAsync(int id, string userId)
    {
        var attachment = await _attachmentRepo.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Attachment {id} not found");

        if (attachment.UploadedByUserId != userId)
            throw new UnauthorizedAccessException("You can only delete your own attachments");

        _attachmentRepo.Delete(attachment);
        await _attachmentRepo.SaveChangesAsync();
    }

    private static AttachmentResponse MapToResponse(Attachment a) => new(
        a.Id, a.CardId, a.FileName, a.StorageUrl, a.FileSizeBytes, a.FileType,
        a.UploadedByUserId, a.UploadedByUser?.DisplayName ?? a.UploadedByUserId,
        a.CreatedAt
    );
}
