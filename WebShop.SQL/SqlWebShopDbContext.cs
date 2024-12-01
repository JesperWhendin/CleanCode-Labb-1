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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>()
            .Property(p => p.Price)
            .HasPrecision(18, 2);



        #region DataSeeding
        modelBuilder.Entity<Product>().HasData(
            new Product
            {
                Id = 1,
                Name = "Product 1",
                Description = "Description 1",
                Price = 10,
                IsAvailable = true
            },
            new Product
            {
                Id = 2,
                Name = "Product 2",
                Description = "Description 2",
                Price = 20,
                IsAvailable = true
            },
            new Product
            {
                Id = 3,
                Name = "Product 3",
                Description = "Description 3",
                Price = 30,
                IsAvailable = false
            });
        #endregion

        base.OnModelCreating(modelBuilder);
    }
}
