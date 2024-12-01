using Microsoft.EntityFrameworkCore;
using WebShop.SQL.Entities;
using WebShop.SQL.Interfaces;

namespace WebShop.SQL.Repositories
{
    public class ProductRepository(SqlWebShopDbContext context)
        : Repository<Product, int, SqlWebShopDbContext>(context), IProductRepository
    {
        private readonly DbSet<Product> _dbSet = context.Set<Product>();

        public async Task UpdateProduct(Product product)
        {
            _dbSet.Update(product);
        }

        public async Task<IEnumerable<Product>> GetAvailableProducts()
        {
            return await _dbSet.Where(p => p.IsAvailable).ToListAsync();
        }
    }
}
