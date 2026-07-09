namespace TrelloClone.DTOs;

public record CreateCommentRequest(string Content);
public record UpdateCommentRequest(string Content);
public record CommentResponse(int Id, int CardId, string Content, string AuthorId, string AuthorName, DateTime CreatedAt, DateTime UpdatedAt, bool IsEdited);
