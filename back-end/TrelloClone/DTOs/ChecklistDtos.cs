namespace TrelloClone.DTOs;

public record CreateChecklistRequest(string Title);
public record UpdateChecklistRequest(string? Title);
public record ChecklistResponse(int Id, int CardId, string Title, DateTime CreatedAt, int ItemCount, int CompletedCount);
public record ChecklistDetailResponse(int Id, int CardId, string Title, DateTime CreatedAt, List<ChecklistItemResponse> Items);
