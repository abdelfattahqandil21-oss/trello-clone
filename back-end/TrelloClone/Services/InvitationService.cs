namespace TrelloClone.Services;

public class InvitationService : IInvitationService
{
    private readonly IInvitationRepository _invitationRepo;

    public InvitationService(IInvitationRepository invitationRepo)
    {
        _invitationRepo = invitationRepo;
    }

    public async Task<InvitationResponse> CreateAsync(string inviterUserId, CreateInvitationRequest request)
    {
        var token = Guid.NewGuid().ToString("N");

        var invitation = new Invitation
        {
            InviterUserId = inviterUserId,
            InviteeEmail = request.InviteeEmail,
            TargetType = request.TargetType,
            TargetId = request.TargetId,
            Role = request.Role,
            Token = token,
            Status = InvitationStatus.Pending,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(7)
        };

        invitation = await _invitationRepo.AddAsync(invitation);
        await _invitationRepo.SaveChangesAsync();

        return MapToResponse(invitation);
    }

    public async Task<InvitationResponse> GetByTokenAsync(string token)
    {
        var invitation = await _invitationRepo.GetByTokenAsync(token)
            ?? throw new KeyNotFoundException("Invitation not found");
        return MapToResponse(invitation);
    }

    public async Task<IEnumerable<InvitationResponse>> GetByEmailAsync(string email)
    {
        var invitations = await _invitationRepo.GetByEmailAsync(email);
        return invitations.Select(MapToResponse);
    }

    public async Task AcceptAsync(string token, string userId)
    {
        var invitation = await _invitationRepo.GetByTokenAsync(token)
            ?? throw new KeyNotFoundException("Invitation not found");

        if (invitation.Status != InvitationStatus.Pending)
            throw new InvalidOperationException("Invitation is no longer pending");

        if (invitation.ExpiresAt < DateTime.UtcNow)
        {
            invitation.Status = InvitationStatus.Expired;
            await _invitationRepo.SaveChangesAsync();
            throw new InvalidOperationException("Invitation has expired");
        }

        invitation.Status = InvitationStatus.Accepted;
        invitation.RespondedAt = DateTime.UtcNow;
        await _invitationRepo.SaveChangesAsync();
    }

    public async Task RejectAsync(string token)
    {
        var invitation = await _invitationRepo.GetByTokenAsync(token)
            ?? throw new KeyNotFoundException("Invitation not found");

        invitation.Status = InvitationStatus.Rejected;
        invitation.RespondedAt = DateTime.UtcNow;
        await _invitationRepo.SaveChangesAsync();
    }

    public async Task CancelAsync(int invitationId, string userId)
    {
        var invitation = await _invitationRepo.GetByIdAsync(invitationId)
            ?? throw new KeyNotFoundException("Invitation not found");

        if (invitation.InviterUserId != userId)
            throw new UnauthorizedAccessException("You can only cancel your own invitations");

        invitation.Status = InvitationStatus.Cancelled;
        await _invitationRepo.SaveChangesAsync();
    }

    private static InvitationResponse MapToResponse(Invitation i) => new(
        i.Id, i.InviterUserId, i.InviterUser?.DisplayName ?? i.InviterUserId,
        i.InviteeEmail, i.TargetType.ToString(), i.TargetId,
        i.Role.ToString(), i.Status.ToString(),
        i.CreatedAt, i.ExpiresAt
    );
}
