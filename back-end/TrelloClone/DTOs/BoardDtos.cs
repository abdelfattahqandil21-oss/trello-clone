namespace TrelloClone.DTOs;

public record CreateBoardRequest(string Name, string? Description, string? BackgroundColor);
public record UpdateBoardRequest(string? Name, string? Description, string? BackgroundColor, string? BackgroundImageUrl, Visibility? Visibility);
public record BoardResponse(
    int Id, int WorkspaceId, string Name, string? Description,
    string? BackgroundColor, string? BackgroundImageUrl, Visibility Visibility,
    bool IsArchived, DateTime CreatedAt
);
public record BoardFullResponse(
    int Id, int WorkspaceId, string Name, string? Description,
    string? BackgroundColor, string? BackgroundImageUrl, Visibility Visibility,
    bool IsArchived, DateTime CreatedAt,
    List<ListResponse> Lists, List<BoardMemberResponse> Members, List<LabelResponse> Labels
);
public record BoardMemberResponse(string UserId, string UserName, string UserEmail, BoardRole Role, DateTime JoinedAt);
