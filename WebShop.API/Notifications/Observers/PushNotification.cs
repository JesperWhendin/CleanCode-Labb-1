using WebShop.SQL.Entities;

namespace WebShop.Notifications.Observers;

public class PushNotification : INotificationObserver
{
    public void Update(Product product)
    {
        Console.WriteLine($"New product added: {product.Name}.");
    }
}