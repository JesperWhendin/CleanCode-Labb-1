using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using WebShop.MinimalExtensions;
using WebShop.Notifications;
using WebShop.Notifications.Observers;
using WebShop.SQL;
using WebShop.UnitOfWork;

var builder = WebApplication.CreateBuilder(args);

//TODO: Behövs controllers fortfarande?
//builder.Services.AddControllers();

builder.Services.AddAuthorization();

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddTransient<INotificationObserver, EmailNotification>();
builder.Services.AddTransient<INotificationObserver, PushNotification>();
builder.Services.AddSingleton<ProductSubject>();

// DefaultConnection defineras i appsettings.json

var defaultConnection = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<SqlWebShopDbContext>(
    options => options.UseSqlServer(defaultConnection))
    .Configure<SqlServerDbContextOptionsBuilder>(sqlOptions =>
        sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 3,
            maxRetryDelay: TimeSpan.FromSeconds(10),
            errorNumbersToAdd: null));

builder.Services.ConfigureHttpJsonOptions(
    o => o.SerializerOptions.ReferenceHandler = ReferenceHandler.Preserve);

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(sGenOptions =>
{
    sGenOptions.TagActionsBy(aD => new List<string>
    {
        aD.ActionDescriptor.DisplayName!
    });
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseAuthorization();

//TODO: Controllers
//app.MapControllers();

app.MapProductEndpoints();

app.Run();