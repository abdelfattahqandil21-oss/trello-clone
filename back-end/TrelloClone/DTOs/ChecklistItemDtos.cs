namespace TrelloClone.DTOs;

public record CreateChecklistItemRequest(string Title);
public record UpdateChecklistItemRequest(string? Title, bool? IsChecked, string? AssignedUserId);
public record ChecklistItemResponse(int Id, int ChecklistId, string Title, bool IsChecked, int Position, string? AssignedUserId, string? AssignedUserName);
