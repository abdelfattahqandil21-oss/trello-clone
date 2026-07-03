namespace TrelloClone.DataAccess;

public class AppDpContext : IdentityDbContext<AppUser>
{
    public AppDpContext(DbContextOptions<AppDpContext> options) : base(options)
    {
    }

    public DbSet<Board> Boards => Set<Board>();
}
