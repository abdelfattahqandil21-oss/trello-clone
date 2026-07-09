namespace TrelloClone.Repository.IRepository;

public interface IRepository<T> where T : class
{
    Task<T?> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<T> AddAsync(T entity);
    T Update(T entity);
    void Delete(T entity);
    Task<bool> ExistsAsync(int id);
    Task<int> SaveChangesAsync();
}
