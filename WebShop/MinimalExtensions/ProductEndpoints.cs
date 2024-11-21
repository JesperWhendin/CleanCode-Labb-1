using WebShop.UnitOfWork;

namespace WebShop.MinimalExtensions;

public static class ProductEndpoints
{
    public static IEndpointRouteBuilder MapProductEndpoints(this IEndpointRouteBuilder app)
    {
        //Map endpoints
        var group = app.MapGroup("products");

        group.MapGet("", GetAllAsync);

        return app;
    }

    // Declare methods for enpoints
    // Fix Unit of Work and use it instead of repositories

    public static async Task<IResult> GetAllAsync(IUnitOfWork uow)
    {
        var products = await uow.Products.GetAllAsync();
        return Results.Ok(products);
    }
}