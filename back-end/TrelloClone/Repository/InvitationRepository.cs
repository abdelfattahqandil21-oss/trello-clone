namespace TrelloClone.Repository;

public class InvitationRepository : Repository<Invitation>, IInvitationRepository
{
    public InvitationRepository(AppDpContext context) : base(context) { }

    public async Task<Invitation?> GetByTokenAsync(string token)
        => await _context.Invitations
            .FirstOrDefaultAsync(i => i.Token == token);

    public async Task<IEnumerable<Invitation>> GetByEmailAsync(string email)
        => await _context.Invitations
            .Where(i => i.InviteeEmail == email)
            .OrderByDescending(i => i.CreatedAt)
            .ToListAsync();

    public async Task<IEnumerable<Invitation>> GetPendingByTargetAsync(int targetId, InvitationTargetType targetType)
        => await _context.Invitations
            .Where(i => i.TargetId == targetId && i.TargetType == targetType && i.Status == InvitationStatus.Pending)
            .ToListAsync();
}
