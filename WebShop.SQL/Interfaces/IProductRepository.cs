using WebShop.Common.Interfaces;
using WebShop.SQL.Entities;

namespace WebShop.SQL.Interfaces;

public interface IProductRepository : IRepository<Product, int>
{
    Task UpdateProduct(Product product);
    Task<IEnumerable<Product>> GetAvailableProducts();
}