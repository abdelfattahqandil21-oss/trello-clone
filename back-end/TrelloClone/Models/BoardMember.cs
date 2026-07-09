namespace TrelloClone.Models;

public class BoardMember
{
    public int Id { get; set; }
    public BoardRole Role { get; set; }
    public DateTime JoinedAt { get; set; }

    public int BoardId { get; set; }
    public Board Board { get; set; } = null!;

    public AppUser User { get; set; } = null!;
    public string UserId { get; set; } = string.Empty;
}
