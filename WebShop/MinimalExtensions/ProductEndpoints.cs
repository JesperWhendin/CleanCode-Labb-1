using WebShop.SQL.Entities;
using WebShop.UnitOfWork;

namespace WebShop.MinimalExtensions;

public static class ProductEndpoints
{
    public static IEndpointRouteBuilder MapProductEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/product").WithDisplayName("Product Endpoints");

        group.MapGet("{id:int}", GetByIdAsync);
        group.MapGet("", GetAllAsync);
        group.MapGet("{start:int}/{count:int}", GetManyAsync);
        group.MapPost("", AddAsync);
        group.MapDelete("{id:int}", DeleteAsync);
        group.MapPut("{id:int}", UpdateAsync);
        group.MapGet("/available", GetAvailableProductsAsync);

        return app;
    }

    public static async Task<IResult> GetAllAsync(IUnitOfWork uow)
    {
        var products = await uow.Products.GetAllAsync();
        return products is not null ? Results.Ok(products) : Results.NoContent();
    }
    public static async Task<IResult> GetByIdAsync(IUnitOfWork uow, int id)
    {
        var product = await uow.Products.GetByIdAsync(id);
        return product is not null ? Results.Ok(product) : Results.NotFound();
    }

    public static async Task<IResult> GetManyAsync(IUnitOfWork uow, int start, int count)
    {
        var products = await uow.Products.GetManyAsync(start, count);
        return Results.Ok(products);
    }

    public static async Task<IResult> AddAsync(IUnitOfWork uow, Product product)
    {
        await uow.Products.AddAsync(product);
        return Results.Created($"/api/product/{product.Id}", product);
    }

    public static async Task<IResult> DeleteAsync(IUnitOfWork uow, int id)
    {
        await uow.Products.DeleteAsync(id);
        return Results.NoContent();
    }

    public static async Task<IResult> UpdateAsync(IUnitOfWork uow, int id, Product product)
    {
        product.Id = id;
        await uow.Products.UpdateProduct(product);
        return Results.NoContent();
    }

    public static async Task<IResult> GetAvailableProductsAsync(IUnitOfWork uow)
    {
        var products = await uow.Products.GetAvailableProducts();
        return products is not null ? Results.Ok(products) : Results.NoContent();
    }
}
