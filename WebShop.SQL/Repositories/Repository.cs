using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using WebShop.Common.Interfaces;

namespace WebShop.SQL.Repositories;

public class Repository<IEntity> : IRepository<IEntity, Guid> where IEntity : class, IEntity<Guid>
{
    private readonly DbContext _context;
    private readonly DbSet<IEntity> _dbSet;

    public Repository(DbContext context)
    {
        _context = context;
        _dbSet = context.Set<IEntity>();
    }
    public async Task AddAsync(IEntity entity)
    {
        await _dbSet.AddAsync(entity);
    }

    public async Task DeleteAsync(Guid id)
    {
        var entity = await _dbSet.FindAsync(id);
        if (entity is not null)
            _dbSet.Remove(entity);
    }

    public async Task<IEnumerable<IEntity>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    public async Task<IEntity> GetByIdAsync(Guid id)
    {
        return await _dbSet.FindAsync(id);
    }

    public async Task<IEnumerable<IEntity>> GetManyAsync(int start, int count)
    {
        return await _dbSet.Skip(start).Take(count).ToListAsync();
    }
}
