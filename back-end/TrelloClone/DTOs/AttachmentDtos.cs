namespace TrelloClone.DTOs;

public record CreateAttachmentRequest(string FileName, string StorageUrl, long FileSizeBytes, string FileType);
public record AttachmentResponse(int Id, int CardId, string FileName, string StorageUrl, long FileSizeBytes, string FileType, string UploadedByUserId, string UploadedByUserName, DateTime CreatedAt);
