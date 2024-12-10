using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System.Text.Json;
using WebShop.SQL.Entities;
using WebShop.UnitOfWork;
using FakeItEasy;
using WebShop.MinimalExtensions;

namespace WebShop.Tests;

public class ProductEndpointTests
{
    [Fact]
    public async Task GetAllAsync_ReturnsAll()
    {
        // Arrange
        var uowFake = A.Fake<IUnitOfWork>();
        var productsFake = A.CollectionOfDummy<Product>(3);
        A.CallTo(() => uowFake.Products.GetAllAsync()).Returns(Task.FromResult(productsFake.AsEnumerable()));

        // Act
        var result = await ProductEndpoints.GetAllAsync(uowFake);

        // Assert
        var valid = Assert.IsType<Ok<IEnumerable<Product>>>(result);
        var value = Assert.IsAssignableFrom<IEnumerable<Product>>(valid.Value);
        Assert.Equal(3, value.Count());
    }

    [Fact]
    public async Task GetAllAsync_NoProducts_ReturnsNoContent()
    {
        // Arrange
        var uowFake = A.Fake<IUnitOfWork>();
        var productsFake = new List<Product>();
        A.CallTo(() => uowFake.Products.GetAllAsync()).Returns(Task.FromResult(productsFake.AsEnumerable()));

        // Act
        var result = await ProductEndpoints.GetAllAsync(uowFake);

        // Assert
        Assert.IsType<NoContent>(result);
    }

    [Fact]
    public async Task GetByIdAsync_ValidId_ReturnsProduct()
    {
        // Arrange
        var uowFake = A.Fake<IUnitOfWork>();
        var productId = 1;
        var productFake = new Product { Id = productId, Name = "Test Product", Description = "Test Description", Price = 100, IsAvailable = true };
        A.CallTo(() => uowFake.Products.GetByIdAsync(productId)).Returns(Task.FromResult(productFake));

        // Act
        var result = await ProductEndpoints.GetByIdAsync(uowFake, productId);

        // Assert
        var valid = Assert.IsType<Ok<Product>>(result);
        var value = Assert.IsAssignableFrom<Product>(valid.Value);
        Assert.Equal(productId, value.Id);
        Assert.Equal("Test Product", value.Name);
        Assert.Equal(100, value.Price);
    }

    [Fact]
    public async Task GetByIdAsync_InvalidId_ReturnsNotFound()
    {
        // Arrange
        var uowFake = A.Fake<IUnitOfWork>();
        var invalidProductId = 999;
        A.CallTo(() => uowFake.Products.GetByIdAsync(invalidProductId)).Returns(Task.FromResult<Product>(null));

        // Act
        var result = await ProductEndpoints.GetByIdAsync(uowFake, invalidProductId);

        // Assert
        Assert.IsType<NotFound>(result);
    }

    [Fact]
    public async Task GetManyAsync_ValidRange_ReturnsProducts()
    {
        // Arrange
        var uowFake = A.Fake<IUnitOfWork>();
        var start = 0;
        var count = 3;
        var productsFake = A.CollectionOfDummy<Product>(count);
        A.CallTo(() => uowFake.Products.GetManyAsync(start, count)).Returns(Task.FromResult(productsFake.AsEnumerable()));

        // Act
        var result = await ProductEndpoints.GetManyAsync(uowFake, start, count);

        // Assert
        var valid = Assert.IsType<Ok<IEnumerable<Product>>>(result);
        var value = Assert.IsAssignableFrom<IEnumerable<Product>>(valid.Value);
        Assert.Equal(count, value.Count());
    }

    [Fact]
    public async Task GetManyAsync_NoProductsInRange_ReturnsNoContent()
    {
        // Arrange
        var uowFake = A.Fake<IUnitOfWork>();
        var start = 0;
        var count = 5;
        var productsFake = new List<Product>();
        A.CallTo(() => uowFake.Products.GetManyAsync(start, count))
            .Returns(Task.FromResult(productsFake.AsEnumerable()));

        // Act
        var result = await ProductEndpoints.GetManyAsync(uowFake, start, count);

        // Assert
        Assert.IsType<NoContent>(result);
    }

    [Fact]
    public async Task AddAsync_ValidProduct_ReturnsOk()
    {
        // Arrange
        var uowFake = A.Fake<IUnitOfWork>();
        var productFake = new Product { Id = 1, Name = "Anka", Description = "En gul badanka.", Price = 50, IsAvailable = true };

        A.CallTo(() => uowFake.Products.AddAsync(productFake)).Returns(Task.CompletedTask);
        A.CallTo(() => uowFake.Save()).DoesNothing();

        // Act
        var result = await ProductEndpoints.AddAsync(uowFake, productFake);

        // Assert
        var okResult = Assert.IsType<Ok>(result);
        A.CallTo(() => uowFake.Products.AddAsync(productFake)).MustHaveHappened();
        A.CallTo(() => uowFake.Save()).MustHaveHappened();
    }

    [Fact]
    public async Task AddAsync_InvalidProduct_ReturnsBadRequest()
    {
        // Arrange
        var uowFake = A.Fake<IUnitOfWork>();
        Product productFake = null;

        // Act
        var result = await ProductEndpoints.AddAsync(uowFake, productFake);

        // Assert
        var badRequestResult = Assert.IsType<BadRequest>(result);
        A.CallTo(() => uowFake.Products.AddAsync(A<Product>.Ignored)).MustNotHaveHappened();
        A.CallTo(() => uowFake.Save()).MustNotHaveHappened();
    }

    [Fact]
    public async Task DeleteAsync_ProductExists_ReturnsOk()
    {
        // Arrange
        var uowFake = A.Fake<IUnitOfWork>();
        var productFake = new Product { Id = 1, Name = "Anka", Description = "En gul badanka.", Price = 50, IsAvailable = true };

        A.CallTo(() => uowFake.Products.GetByIdAsync(1)).Returns(Task.FromResult(productFake));
        A.CallTo(() => uowFake.Products.DeleteAsync(1)).Returns(Task.CompletedTask);
        A.CallTo(() => uowFake.Save()).DoesNothing();

        // Act
        var result = await ProductEndpoints.DeleteAsync(uowFake, 1);

        // Assert
        var okResult = Assert.IsType<Ok>(result);
        A.CallTo(() => uowFake.Products.DeleteAsync(1)).MustHaveHappened();
        A.CallTo(() => uowFake.Save()).MustHaveHappened();
    }

    [Fact]
    public async Task DeleteAsync_ProductNotFound_ReturnsNotFound()
    {
        // Arrange
        var uowFake = A.Fake<IUnitOfWork>();
        A.CallTo(() => uowFake.Products.GetByIdAsync(1)).Returns(Task.FromResult<Product>(null));

        // Act
        var result = await ProductEndpoints.DeleteAsync(uowFake, 1);

        // Assert
        var notFoundResult = Assert.IsType<NotFound>(result);
        A.CallTo(() => uowFake.Products.DeleteAsync(1)).MustNotHaveHappened();
        A.CallTo(() => uowFake.Save()).MustNotHaveHappened();
    }

    [Fact]
    public async Task UpdateAsync_ValidProduct_ReturnsOk()
    {
        // Arrange
        var uowFake = A.Fake<IUnitOfWork>();
        var productFake = new Product { Id = 1, Name = "Anka", Description = "En gul badanka.", Price = 50, IsAvailable = true };

        A.CallTo(() => uowFake.Products.UpdateProduct(productFake)).Returns(Task.CompletedTask);
        A.CallTo(() => uowFake.Save()).DoesNothing();

        // Act
        var result = await ProductEndpoints.UpdateAsync(uowFake, productFake);

        // Assert
        var okResult = Assert.IsType<Ok>(result);
        A.CallTo(() => uowFake.Products.UpdateProduct(productFake)).MustHaveHappened();
        A.CallTo(() => uowFake.Save()).MustHaveHappened();
    }

    [Fact]
    public async Task UpdateAsync_InvalidProduct_ReturnsBadRequest()
    {
        // Arrange
        var uowFake = A.Fake<IUnitOfWork>();
        Product productFake = null;

        // Act
        var result = await ProductEndpoints.UpdateAsync(uowFake, productFake);

        // Assert
        var badRequestResult = Assert.IsType<BadRequest>(result);
        A.CallTo(() => uowFake.Products.UpdateProduct(A<Product>.Ignored)).MustNotHaveHappened();
        A.CallTo(() => uowFake.Save()).MustNotHaveHappened();
    }

    [Fact]
    public async Task GetAvailableProductsAsync_ProductsAvailable_ReturnsOk()
    {
        // Arrange
        var uowFake = A.Fake<IUnitOfWork>();
        var availableProducts = new List<Product>
        {
            new () { Id = 1, Name = "Anka", Description = "En gul badanka.", Price = 50, IsAvailable = true },
            new () { Id = 3, Name = "Hund", Description = "En brun hund.", Price = 150, IsAvailable = true },
            new () { Id = 3, Name = "Katt", Description = "En svart katt.", Price = 100, IsAvailable = false }
        };
        A.CallTo(() => uowFake.Products.GetAvailableProducts()).Returns(Task.FromResult(availableProducts.Where(p => p.IsAvailable).AsEnumerable()));

        // Act
        var result = await ProductEndpoints.GetAvailableProductsAsync(uowFake);

        // Assert
        var okResult = Assert.IsType<Ok<IEnumerable<Product>>>(result);
        var value = Assert.IsAssignableFrom<IEnumerable<Product>>(okResult.Value);
        Assert.Equal(2, value.Count());
        Assert.All(value, product => Assert.True(product.IsAvailable));
    }

    [Fact]
    public async Task GetAvailableProductsAsync_NoProductsAvailable_ReturnsNoContent()
    {
        // Arrange
        var uowFake = A.Fake<IUnitOfWork>();
        A.CallTo(() => uowFake.Products.GetAvailableProducts()).Returns(Task.FromResult(Enumerable.Empty<Product>()));

        // Act
        var result = await ProductEndpoints.GetAvailableProductsAsync(uowFake);

        // Assert
        var noContentResult = Assert.IsType<NoContent>(result);
    }
}
