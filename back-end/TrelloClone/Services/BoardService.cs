namespace TrelloClone.Services;

public class BoardService : IBoardService
{
    private readonly IBoardRepository _boardRepo;
    private readonly IWorkspaceRepository _workspaceRepo;
    private readonly IRepository<BoardMember> _memberRepo;

    public BoardService(IBoardRepository boardRepo, IWorkspaceRepository workspaceRepo, IRepository<BoardMember> memberRepo)
    {
        _boardRepo = boardRepo;
        _workspaceRepo = workspaceRepo;
        _memberRepo = memberRepo;
    }

    public async Task<BoardResponse> GetByIdAsync(int id)
    {
        var board = await _boardRepo.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Board {id} not found");
        return MapToResponse(board);
    }

    public async Task<BoardFullResponse> GetFullAsync(int id)
    {
        var board = await _boardRepo.GetFullAsync(id)
            ?? throw new KeyNotFoundException($"Board {id} not found");

        return new BoardFullResponse(
            board.Id, board.WorkspaceId, board.Name, board.Description,
            board.BackgroundColor, board.BackgroundImageUrl, board.Visibility,
            board.IsArchived, board.CreatedAt,
            board.Lists?.Select(l => new ListResponse(
                l.Id, l.BoardId, l.Name, l.Position,
                l.IsArchived, l.CreatedAt, l.Cards?.Count ?? 0
            )).ToList() ?? [],
            board.Members?.Select(m => new BoardMemberResponse(
                m.UserId, m.User?.DisplayName ?? m.UserId,
                m.User?.Email ?? "", m.Role, m.JoinedAt
            )).ToList() ?? [],
            board.Labels?.Select(l => new LabelResponse(
                l.Id, l.BoardId, l.Name ?? "", l.Color
            )).ToList() ?? []
        );
    }

    public async Task<IEnumerable<BoardResponse>> GetByWorkspaceIdAsync(int workspaceId)
    {
        var boards = await _boardRepo.GetByWorkspaceIdAsync(workspaceId);
        return boards.Select(MapToResponse);
    }

    public async Task<IEnumerable<BoardResponse>> GetByMemberIdAsync(string userId)
    {
        var boards = await _boardRepo.GetByMemberIdAsync(userId);
        return boards.Select(MapToResponse);
    }

    public async Task<BoardResponse> CreateAsync(int workspaceId, CreateBoardRequest request, string userId)
    {
        var workspace = await _workspaceRepo.GetByIdAsync(workspaceId)
            ?? throw new KeyNotFoundException($"Workspace {workspaceId} not found");

        var board = new Board
        {
            WorkspaceId = workspaceId,
            Name = request.Name,
            Description = request.Description,
            BackgroundColor = request.BackgroundColor,
            Visibility = Visibility.Private,
            CreatedAt = DateTime.UtcNow
        };

        board = await _boardRepo.AddAsync(board);

        board.Members.Add(new BoardMember
        {
            BoardId = board.Id,
            UserId = userId,
            Role = BoardRole.Admin,
            JoinedAt = DateTime.UtcNow
        });

        await _boardRepo.SaveChangesAsync();
        return MapToResponse(board);
    }

    public async Task<BoardResponse> UpdateAsync(int id, UpdateBoardRequest request)
    {
        var board = await _boardRepo.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Board {id} not found");

        if (request.Name is not null) board.Name = request.Name;
        if (request.Description is not null) board.Description = request.Description;
        if (request.BackgroundColor is not null) board.BackgroundColor = request.BackgroundColor;
        if (request.BackgroundImageUrl is not null) board.BackgroundImageUrl = request.BackgroundImageUrl;
        if (request.Visibility.HasValue) board.Visibility = request.Visibility.Value;

        board.UpdatedAt = DateTime.UtcNow;
        _boardRepo.Update(board);
        await _boardRepo.SaveChangesAsync();

        return MapToResponse(board);
    }

    public async Task ArchiveAsync(int id)
    {
        var board = await _boardRepo.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Board {id} not found");

        board.IsArchived = true;
        board.ArchivedAt = DateTime.UtcNow;
        await _boardRepo.SaveChangesAsync();
    }

    public async Task UnarchiveAsync(int id)
    {
        var board = await _boardRepo.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Board {id} not found");

        board.IsArchived = false;
        board.ArchivedAt = null;
        await _boardRepo.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var board = await _boardRepo.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Board {id} not found");

        _boardRepo.Delete(board);
        await _boardRepo.SaveChangesAsync();
    }

    public async Task<IEnumerable<BoardMemberResponse>> GetMembersAsync(int boardId)
    {
        var board = await _boardRepo.GetWithMembersAsync(boardId)
            ?? throw new KeyNotFoundException($"Board {boardId} not found");

        return board.Members.Select(m => new BoardMemberResponse(
            m.UserId, m.User?.DisplayName ?? m.UserId,
            m.User?.Email ?? "", m.Role, m.JoinedAt
        ));
    }

    public async Task AddMemberAsync(int boardId, string userId, BoardRole role)
    {
        var board = await _boardRepo.GetWithMembersAsync(boardId)
            ?? throw new KeyNotFoundException($"Board {boardId} not found");

        if (board.Members.Any(m => m.UserId == userId))
            throw new InvalidOperationException("User is already a member of this board");

        board.Members.Add(new BoardMember
        {
            BoardId = boardId,
            UserId = userId,
            Role = role,
            JoinedAt = DateTime.UtcNow
        });

        await _boardRepo.SaveChangesAsync();
    }

    public async Task RemoveMemberAsync(int boardId, string userId)
    {
        var board = await _boardRepo.GetWithMembersAsync(boardId)
            ?? throw new KeyNotFoundException($"Board {boardId} not found");

        var member = board.Members.FirstOrDefault(m => m.UserId == userId)
            ?? throw new KeyNotFoundException($"Member {userId} not found in board");

        _memberRepo.Delete(member);
        await _memberRepo.SaveChangesAsync();
    }

    public async Task UpdateMemberRoleAsync(int boardId, string userId, BoardRole role)
    {
        var board = await _boardRepo.GetWithMembersAsync(boardId)
            ?? throw new KeyNotFoundException($"Board {boardId} not found");

        var member = board.Members.FirstOrDefault(m => m.UserId == userId)
            ?? throw new KeyNotFoundException($"Member {userId} not found in board");

        member.Role = role;
        await _boardRepo.SaveChangesAsync();
    }

    private static BoardResponse MapToResponse(Board b) => new(
        b.Id, b.WorkspaceId, b.Name, b.Description,
        b.BackgroundColor, b.BackgroundImageUrl, b.Visibility,
        b.IsArchived, b.CreatedAt
    );
}
