using Microsoft.EntityFrameworkCore;
using WebShop.Common.Interfaces;

namespace WebShop.SQL.Repositories;

public class Repository<IEntity, TId, TContext>(TContext context)
    : IRepository<IEntity, TId>
    where IEntity : class, IEntity<TId>
    where TContext : DbContext
{
    private readonly DbSet<IEntity> _dbSet = context.Set<IEntity>();

    public async Task<IEnumerable<IEntity>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    public async Task<IEntity> GetByIdAsync(TId id)
    {
        return await _dbSet.FindAsync(id);
    }

    public async Task<IEnumerable<IEntity>> GetManyAsync(int start, int count)
    {
        return await _dbSet.Skip(start).Take(count).ToListAsync();
    }

    public async Task AddAsync(IEntity entity)
    {
        await _dbSet.AddAsync(entity);
    }

    public async Task DeleteAsync(TId id)
    {
        var entity = await _dbSet.FindAsync(id);
        if (entity is not null)
            _dbSet.Remove(entity);
    }
}
