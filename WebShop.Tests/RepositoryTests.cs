using Microsoft.EntityFrameworkCore;
using WebShop.SQL;
using WebShop.SQL.Entities;
using WebShop.SQL.Repositories;

namespace WebShop.Tests;

public class RepositoryTests
{
    [Fact]
    public async Task GetAllAsync_WithData_ReturnsAll()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<SqlWebShopDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        await using var dbContext = new SqlWebShopDbContext(options);
        var productRepository = new Repository<Product, int, SqlWebShopDbContext>(dbContext);
        var products = new List<Product>
        {
            new () { Id = 1, Name = "Anka", Description = "En gul badanka.", Price = 50, IsAvailable = true },
            new () { Id = 2, Name = "Katt", Description = "En svart katt.", Price = 100, IsAvailable = false },
            new () { Id = 3, Name = "Hund", Description = "En brun hund.", Price = 150, IsAvailable = true }
        };
        dbContext.Products.AddRange(products);
        dbContext.SaveChanges();

        // Act
        var result = productRepository.GetAllAsync();

        // Assert
        Assert.Equal(products.Count, result.Result.Count());
    }

    [Fact]
    public async Task GetAllAsync_WithNoData_ReturnsEmpty()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<SqlWebShopDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        await using var dbContext = new SqlWebShopDbContext(options);
        var productRepository = new Repository<Product, int, SqlWebShopDbContext>(dbContext);
        // Act
        var result = productRepository.GetAllAsync();
        // Assert
        Assert.Empty(result.Result);
    }

    [Fact]
    public async Task GetByIdAsync_WithValidInput_ReturnsEntity()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<SqlWebShopDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        await using var dbContext = new SqlWebShopDbContext(options);
        var productRepository = new Repository<Product, int, SqlWebShopDbContext>(dbContext);
        var product = new Product { Id = 1, Name = "Anka", Description = "En gul badanka.", Price = 50, IsAvailable = true };
        dbContext.Products.Add(product);
        dbContext.SaveChanges();

        // Act
        var result = await productRepository.GetByIdAsync(1);

        // Assert
        Assert.Equal(product, result);
    }

    [Fact]
    public async Task GetByIdAsync_WithInvalidId_ReturnsNull()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<SqlWebShopDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        await using var dbContext = new SqlWebShopDbContext(options);
        var productRepository = new Repository<Product, int, SqlWebShopDbContext>(dbContext);

        // Act
        var result = await productRepository.GetByIdAsync(12345);

        // Assert
        Assert.Null(result);
    }
    
    [Fact]
    public async Task GetManyAsync_WithValidRange_ReturnsCorrectEntities()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<SqlWebShopDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        await using var dbContext = new SqlWebShopDbContext(options);
        var productRepository = new Repository<Product, int, SqlWebShopDbContext>(dbContext);

        var products = new List<Product>
        {
            new () { Id = 1, Name = "Anka", Description = "En gul badanka.", Price = 50, IsAvailable = true },
            new () { Id = 2, Name = "Katt", Description = "En svart katt.", Price = 100, IsAvailable = false },
            new () { Id = 3, Name = "Hund", Description = "En brun hund.", Price = 150, IsAvailable = true },
            new () { Id = 4, Name = "Häst", Description = "En brun häst.", Price = 200, IsAvailable = true },
            new () { Id = 5, Name = "Ko", Description = "En svart och vit ko.", Price = 250, IsAvailable = true }
        };
        dbContext.Products.AddRange(products);
        dbContext.SaveChanges();

        // Act
        var result = await productRepository.GetManyAsync(1, 3);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Count());
        Assert.Equal(new[] { 2, 3, 4 }, result.Select(p => p.Id));
    }

    [Fact]
    public async Task GetManyAsync_WithInvalidRange_ReturnsEmptyList()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<SqlWebShopDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        await using var dbContext = new SqlWebShopDbContext(options);
        var productRepository = new Repository<Product, int, SqlWebShopDbContext>(dbContext);
        var products = new List<Product>
        {
            new () { Id = 1, Name = "Anka", Description = "En gul badanka.", Price = 50, IsAvailable = true },
            new () { Id = 2, Name = "Katt", Description = "En svart katt.", Price = 100, IsAvailable = false },
            new () { Id = 3, Name = "Hund", Description = "En brun hund.", Price = 150, IsAvailable = true }
        };
        dbContext.Products.AddRange(products);
        dbContext.SaveChanges();

        // Act
        var result = await productRepository.GetManyAsync(5, 3);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task AddAsync_WithValidEntity_AddsEntityToDatabase()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<SqlWebShopDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        await using var dbContext = new SqlWebShopDbContext(options);
        var productRepository = new Repository<Product, int, SqlWebShopDbContext>(dbContext);

        var newProduct = new Product { Id = 1, Name = "Papegoja", Description = "Den kan prata.", Price = 100, IsAvailable = true };

        // Act
        await productRepository.AddAsync(newProduct);
        await dbContext.SaveChangesAsync();

        // Assert
        var productInDb = dbContext.Products.FirstOrDefault(p => p.Id == 1);
        Assert.NotNull(productInDb);
        Assert.Equal(newProduct.Name, productInDb.Name);
        Assert.Equal(newProduct.Description, productInDb.Description);
        Assert.Equal(newProduct.Price, productInDb.Price);
        Assert.Equal(newProduct.IsAvailable, productInDb.IsAvailable);
    }

    [Fact]
    public async Task AddAsync_WithDuplicateId_ThrowsException()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<SqlWebShopDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        await using var dbContext = new SqlWebShopDbContext(options);
        var productRepository = new Repository<Product, int, SqlWebShopDbContext>(dbContext);

        var product1 = new Product { Id = 1, Name = "Anka", Description = "En gul badanka.", Price = 50, IsAvailable = true };

        var product2 = new Product { Id = 1, Name = "Katt", Description = "En svart katt.", Price = 100, IsAvailable = true };

        dbContext.Products.Add(product1);
        dbContext.SaveChanges();

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
        {
            await productRepository.AddAsync(product2);
            await dbContext.SaveChangesAsync();
        });
    }

    [Fact]
    public async Task AddAsync_WithIncompleteEntity_ThrowsException()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<SqlWebShopDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        await using var dbContext = new SqlWebShopDbContext(options);
        var productRepository = new Repository<Product, int, SqlWebShopDbContext>(dbContext);

        var incompleteProduct = new Product { Id = 1, Price = 50, IsAvailable = true };

        // Act & Assert
        await Assert.ThrowsAsync<DbUpdateException>(async () =>
        {
            await productRepository.AddAsync(incompleteProduct);
            await dbContext.SaveChangesAsync();
        });
    }

    [Fact]
    public async Task DeleteAsync_WithValidId_RemovesEntityFromDatabase()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<SqlWebShopDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        await using var dbContext = new SqlWebShopDbContext(options);
        var productRepository = new Repository<Product, int, SqlWebShopDbContext>(dbContext);

        var product = new Product { Id = 1, Name = "Anka", Description = "En gul badanka.", Price = 50, IsAvailable = true };
        dbContext.Products.Add(product);
        dbContext.SaveChanges();

        // Act
        await productRepository.DeleteAsync(1);
        await dbContext.SaveChangesAsync();

        // Assert
        var productInDb = dbContext.Products.FirstOrDefault(p => p.Id == 1);
        Assert.Null(productInDb);
    }

    [Fact]
    public async Task DeleteAsync_WithNonExistentId_DoesNothing()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<SqlWebShopDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        await using var dbContext = new SqlWebShopDbContext(options);
        var productRepository = new Repository<Product, int, SqlWebShopDbContext>(dbContext);

        var product = new Product { Id = 1, Name = "Anka", Description = "En gul badanka.", Price = 50, IsAvailable = true };

        dbContext.Products.Add(product);
        dbContext.SaveChanges();

        // Act
        await productRepository.DeleteAsync(2); 
        await dbContext.SaveChangesAsync();

        // Assert
        var productInDb = dbContext.Products.FirstOrDefault(p => p.Id == 1);
        Assert.NotNull(productInDb);
    }
}
