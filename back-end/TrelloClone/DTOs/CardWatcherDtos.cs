namespace TrelloClone.DTOs;

public record WatcherResponse(string UserId, string UserName, string UserEmail, DateTime WatchedAt);
