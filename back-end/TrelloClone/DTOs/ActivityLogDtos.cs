namespace TrelloClone.DTOs;

public record ActivityLogResponse(int Id, int? BoardId, int? CardId, string ActorUserId, string ActorName, string ActionType, string EntityType, int? EntityId, string? MetadataJson, DateTime CreatedAt);
