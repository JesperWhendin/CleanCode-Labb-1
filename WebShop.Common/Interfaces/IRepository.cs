namespace WebShop.Common.Interfaces;

public interface IRepository<TEntity, in TId> where TEntity : IEntity<TId>
{
    Task<TEntity> GetByIdAsync(TId id);
    Task<IEnumerable<TEntity>> GetAllAsync();
    Task<IEnumerable<TEntity>> GetManyAsync(int start, int count);
    Task AddAsync(TEntity entity);
    Task DeleteAsync(TId id);
}
