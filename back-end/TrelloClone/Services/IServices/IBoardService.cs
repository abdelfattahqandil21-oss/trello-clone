namespace TrelloClone.Services.IServices;

public interface IBoardService
{
    Task<BoardResponse> GetByIdAsync(int id);
    Task<BoardFullResponse> GetFullAsync(int id);
    Task<IEnumerable<BoardResponse>> GetByWorkspaceIdAsync(int workspaceId);
    Task<IEnumerable<BoardResponse>> GetByMemberIdAsync(string userId);
    Task<BoardResponse> CreateAsync(int workspaceId, CreateBoardRequest request, string userId);
    Task<BoardResponse> UpdateAsync(int id, UpdateBoardRequest request);
    Task ArchiveAsync(int id);
    Task UnarchiveAsync(int id);
    Task DeleteAsync(int id);
    Task<IEnumerable<BoardMemberResponse>> GetMembersAsync(int boardId);
    Task AddMemberAsync(int boardId, string userId, BoardRole role);
    Task RemoveMemberAsync(int boardId, string userId);
    Task UpdateMemberRoleAsync(int boardId, string userId, BoardRole role);
}
