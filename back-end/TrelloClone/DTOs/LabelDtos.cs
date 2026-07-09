namespace TrelloClone.DTOs;

public record CreateLabelRequest(string Name, string Color);
public record UpdateLabelRequest(string? Name, string? Color);
public record LabelResponse(int Id, int BoardId, string Name, string Color);
