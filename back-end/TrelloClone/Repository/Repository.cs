namespace TrelloClone.Repository;

public class Repository<T> : IRepository<T> where T : class
{
    protected readonly AppDpContext _context;
    protected readonly DbSet<T> _dbSet;

    public Repository(AppDpContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public virtual async Task<T?> GetByIdAsync(int id) => await _dbSet.FindAsync(id);

    public virtual async Task<IEnumerable<T>> GetAllAsync() => await _dbSet.ToListAsync();

    public virtual async Task<T> AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
        return entity;
    }

    public virtual T Update(T entity)
    {
        _dbSet.Update(entity);
        return entity;
    }

    public virtual void Delete(T entity) => _dbSet.Remove(entity);

    public virtual async Task<bool> ExistsAsync(int id) => await _dbSet.FindAsync(id) is not null;

    public virtual async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();
}
