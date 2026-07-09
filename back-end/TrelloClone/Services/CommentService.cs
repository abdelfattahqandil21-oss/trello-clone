namespace TrelloClone.Services;

public class CommentService : ICommentService
{
    private readonly ICommentRepository _commentRepo;

    public CommentService(ICommentRepository commentRepo)
    {
        _commentRepo = commentRepo;
    }

    public async Task<CommentResponse> GetByIdAsync(int id)
    {
        var comment = await _commentRepo.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Comment {id} not found");
        return MapToResponse(comment);
    }

    public async Task<IEnumerable<CommentResponse>> GetByCardIdAsync(int cardId)
    {
        var comments = await _commentRepo.GetByCardIdAsync(cardId);
        return comments.Select(MapToResponse);
    }

    public async Task<CommentResponse> CreateAsync(int cardId, string authorId, CreateCommentRequest request)
    {
        var comment = new Comment
        {
            CardId = cardId,
            AuthorId = authorId,
            Content = request.Content,
            CreatedAt = DateTime.UtcNow
        };

        comment = await _commentRepo.AddAsync(comment);
        await _commentRepo.SaveChangesAsync();

        return MapToResponse(comment);
    }

    public async Task<CommentResponse> UpdateAsync(int id, string userId, UpdateCommentRequest request)
    {
        var comment = await _commentRepo.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Comment {id} not found");

        if (comment.AuthorId != userId)
            throw new UnauthorizedAccessException("You can only edit your own comments");

        comment.Content = request.Content;
        comment.EditedAt = DateTime.UtcNow;
        comment.IsEdited = true;

        _commentRepo.Update(comment);
        await _commentRepo.SaveChangesAsync();

        return MapToResponse(comment);
    }

    public async Task SoftDeleteAsync(int id, string userId)
    {
        var comment = await _commentRepo.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Comment {id} not found");

        if (comment.AuthorId != userId)
            throw new UnauthorizedAccessException("You can only delete your own comments");

        comment.IsDeleted = true;
        await _commentRepo.SaveChangesAsync();
    }

    private static CommentResponse MapToResponse(Comment c) => new(
        c.Id, c.CardId, c.Content, c.AuthorId,
        c.Author?.DisplayName ?? c.AuthorId,
        c.CreatedAt, c.EditedAt ?? c.CreatedAt, c.IsEdited
    );
}
