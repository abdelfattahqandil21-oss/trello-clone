namespace TrelloClone.Models;

public class Attachment
{
    public int Id { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string FileType { get; set; } = string.Empty;
    public long FileSizeBytes { get; set; }
    public string StorageUrl { get; set; } = string.Empty;
    public string? ThumbnailUrl { get; set; }
    public bool IsCover { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Card Card { get; set; } = null!;
    public int CardId { get; set; }

    public AppUser UploadedByUser { get; set; } = null!;
    public string UploadedByUserId { get; set; } = string.Empty;
}
