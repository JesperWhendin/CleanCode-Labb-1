using WebShop.Common.Interfaces;

namespace WebShop.SQL.Entities;

public class Product : IEntity<int>
{
    public int Id { get; set; } = new ();
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public bool IsAvailable { get; set; }

}