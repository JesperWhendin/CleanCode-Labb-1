using WebShop.Common.Interfaces;

namespace WebShop.SQL.Entities;

public abstract class EntityBase : IEntity<int>
{
    public int Id { get; set; } 
}