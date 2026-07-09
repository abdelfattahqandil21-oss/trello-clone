namespace TrelloClone.DTOs;

public record NotificationResponse(int Id, string Type, string ReferenceEntityType, int ReferenceEntityId, string Message, bool IsRead, DateTime CreatedAt);
