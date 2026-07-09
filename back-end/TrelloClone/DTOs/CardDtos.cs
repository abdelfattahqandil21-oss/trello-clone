namespace TrelloClone.DTOs;

public record CreateCardRequest(string Title, string? Description, decimal? Position, DateOnly? DueDate, DateTime? DueDateTime);
public record UpdateCardRequest(string? Title, string? Description, decimal? Position, string? CoverImageUrl, string? CoverColor, DateOnly? DueDate, DateTime? DueDateTime, bool? IsDueComplete);
public record MoveCardRequest(int NewListId, decimal NewPosition);
public record CardResponse(
    int Id, int ListId, int BoardId, string Title, string? Description,
    decimal Position, string? CoverImageUrl, string? CoverColor,
    DateOnly? DueDate, bool IsDueComplete, bool IsArchived,
    string CreatedByUserId, DateTime CreatedAt, int MemberCount, int CommentCount
);
public record CardDetailResponse(
    int Id, int ListId, int BoardId, string Title, string? Description,
    decimal Position, string? CoverImageUrl, string? CoverColor,
    DateOnly? DueDate, DateTime? DueDateTime, bool IsDueComplete, bool IsArchived,
    string CreatedByUserId, DateTime CreatedAt, DateTime UpdatedAt,
    List<CardMemberResponse> Members, List<CardLabelResponse> Labels,
    List<ChecklistDetailResponse> Checklists, List<CommentResponse> Comments,
    List<AttachmentResponse> Attachments, List<WatcherResponse> Watchers
);
public record CardMemberResponse(string UserId, string UserName, string UserEmail, string? AssignedByUserId, DateTime AssignedAt);
public record CardLabelResponse(int LabelId, string LabelName, string LabelColor, int CardId);
