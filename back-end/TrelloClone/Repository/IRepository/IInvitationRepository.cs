namespace TrelloClone.Repository.IRepository;

public interface IInvitationRepository : IRepository<Invitation>
{
    Task<Invitation?> GetByTokenAsync(string token);
    Task<IEnumerable<Invitation>> GetByEmailAsync(string email);
    Task<IEnumerable<Invitation>> GetPendingByTargetAsync(int targetId, InvitationTargetType targetType);
}
