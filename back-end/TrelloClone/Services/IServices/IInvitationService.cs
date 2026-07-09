namespace TrelloClone.Services.IServices;

public interface IInvitationService
{
    Task<InvitationResponse> CreateAsync(string inviterUserId, CreateInvitationRequest request);
    Task<InvitationResponse> GetByTokenAsync(string token);
    Task<IEnumerable<InvitationResponse>> GetByEmailAsync(string email);
    Task AcceptAsync(string token, string userId);
    Task RejectAsync(string token);
    Task CancelAsync(int invitationId, string userId);
}
