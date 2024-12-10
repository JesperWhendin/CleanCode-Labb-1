using WebShop.SQL.Entities;
using WebShop.UnitOfWork;

namespace WebShop.MinimalExtensions;

public static class ProductEndpoints
{
    public static IEndpointRouteBuilder MapProductEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/products").WithDisplayName("Product Endpoints");

        group.MapGet("{id:int}", GetByIdAsync);
        group.MapGet("", GetAllAsync);
        group.MapGet("{start:int}/{count:int}", GetManyAsync);
        group.MapPost("", AddAsync);
        group.MapDelete("{id:int}", DeleteAsync);
        group.MapPut("", UpdateAsync);
        group.MapGet("/available", GetAvailableProductsAsync);

        return app;
    }

    public static async Task<IResult> GetAllAsync(IUnitOfWork uow)
    {
        var products = await uow.Products.GetAllAsync();
        if (products == null || !products.Any())
        {
            return Results.NoContent();
        }
        return Results.Ok(products);
    }

    public static async Task<IResult> GetByIdAsync(IUnitOfWork uow, int id)
    {
        var product = await uow.Products.GetByIdAsync(id);
        return product is not null ? Results.Ok(product) : Results.NotFound();
    }

    public static async Task<IResult> GetManyAsync(IUnitOfWork uow, int start, int count)
    {
        var products = await uow.Products.GetManyAsync(start, count);
        if (products == null || !products.Any())
        {
            return Results.NoContent();
        }
        return Results.Ok(products);
    }

    public static async Task<IResult> AddAsync(IUnitOfWork uow, Product product)
    {
        if (product is null) return Results.BadRequest();
        await uow.Products.AddAsync(product);
        uow.Save();
        return Results.Ok();
    }

    public static async Task<IResult> DeleteAsync(IUnitOfWork uow, int id)
    {
        var product = await uow.Products.GetByIdAsync(id);
        if (product is null)
            return Results.NotFound();

        await uow.Products.DeleteAsync(id);
        uow.Save();
        return Results.Ok();
    }

    public static async Task<IResult> UpdateAsync(IUnitOfWork uow, Product product)
    {
        if (product is null)
            return Results.BadRequest();

        await uow.Products.UpdateProduct(product);
        uow.Save();
        return Results.Ok();
    }

    public static async Task<IResult> GetAvailableProductsAsync(IUnitOfWork uow)
    {
        var products = await uow.Products.GetAvailableProducts();
        return products.Any() ? Results.Ok(products) : Results.NoContent();
    }
}
