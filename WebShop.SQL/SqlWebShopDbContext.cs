using Microsoft.EntityFrameworkCore;
using WebShop.SQL.Entities;

namespace WebShop.SQL;

public class SqlWebShopDbContext : DbContext
{
    // Declare DbSets
    public DbSet<Product> Products { get; set; }


    public SqlWebShopDbContext(DbContextOptions<SqlWebShopDbContext> options) : base(options)
    {
    }
}
