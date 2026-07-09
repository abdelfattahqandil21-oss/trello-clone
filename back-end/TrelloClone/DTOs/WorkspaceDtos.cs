namespace TrelloClone.DTOs;

public record CreateWorkspaceRequest(string Name, string? Description, Visibility Visibility = Visibility.Private);
public record UpdateWorkspaceRequest(string? Name, string? Description, Visibility? Visibility);
public record WorkspaceResponse(
    int Id, string Name, string? Description, Visibility Visibility,
    string OwnerId, string OwnerName, DateTime CreatedAt, int MemberCount
);
