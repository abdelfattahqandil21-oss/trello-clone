namespace TrelloClone.DataAccess
{
    public class AppDpContext : DbContext
    {
        public AppDpContext(DbContextOptions<AppDpContext> options) : base(options)
        {
        }
        public DbSet<AppUser> Users { get; set; }
    }
}
