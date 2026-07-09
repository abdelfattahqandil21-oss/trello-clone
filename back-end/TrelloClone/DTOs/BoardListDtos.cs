namespace TrelloClone.DTOs;

public record CreateListRequest(string Name);
public record UpdateListRequest(string? Name, decimal? Position);
public record ReorderListRequest(int ListId, decimal NewPosition);
public record ReorderCardsRequest(int CardId, int NewListId, decimal NewPosition);
public record ListResponse(
    int Id, int BoardId, string Name, decimal Position,
    bool IsArchived, DateTime CreatedAt, int CardCount
);
public record ListWithCardsResponse(
    int Id, int BoardId, string Name, decimal Position,
    bool IsArchived, DateTime CreatedAt, List<CardResponse> Cards
);
