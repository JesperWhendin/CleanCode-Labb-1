using Microsoft.EntityFrameworkCore;
using WebShop.SQL.Interfaces;
using Moq;
using WebShop.SQL;
using WebShop.SQL.Entities;
using WebShop.SQL.Repositories;

namespace WebShop.Tests;

public class ProductRepositoryTests
{
    private readonly Mock<DbSet<Product>> _mockProductDbSet;
    private Mock<SqlWebShopDbContext> _mockDbContext;
    private readonly IProductRepository _productRepository;
    public DbContextOptions<SqlWebShopDbContext> _context = new DbContextOptionsBuilder<SqlWebShopDbContext>().Options;

    public ProductRepositoryTests()
    {
        _mockProductDbSet = new Mock<DbSet<Product>>();
        _mockDbContext = new Mock<SqlWebShopDbContext>(_context);
        _mockDbContext.Setup(c => c.Set<Product>()).Returns(_mockProductDbSet.Object);
        _productRepository = new ProductRepository(_mockDbContext.Object);
    }

    [Fact]
    public void UpdateProduct_WithValidInput_ExpectSuccess()
    {
        // Arrange
        var product = new Product { Id = 1, Name = "Anka", Description = "En gul badanka.", Price = 50, IsAvailable = true };

        // Act
        _productRepository.UpdateProduct(product);

        // Assert
        _mockProductDbSet.Verify(p => p.Update(product), Times.Once);
    }

    [Fact]
    public async Task GetProducts_AvailableOnly_ReturnsProducts()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<SqlWebShopDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        await using var dbContext = new SqlWebShopDbContext(options);
        var productRepository = new ProductRepository(dbContext);
        var products = new List<Product>
        {
            new () { Id = 1, Name = "Anka", Description = "En gul badanka.", Price = 50, IsAvailable = true },
            new () { Id = 2, Name = "Katt", Description = "En svart katt.", Price = 100, IsAvailable = false },
            new () { Id = 3, Name = "Hund", Description = "En brun hund.", Price = 150, IsAvailable = true }
        };
        dbContext.Products.AddRange(products);
        dbContext.SaveChanges();

        // Act
        var result = await productRepository.GetAvailableProducts();

        // Assert
        var availableProducts = dbContext.Products.Where(p => p.IsAvailable).ToList();
        Assert.Equal(availableProducts.Count, result.Count());
        Assert.All(result, p => Assert.True(p.IsAvailable));
    }
}
