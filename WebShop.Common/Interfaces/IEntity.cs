namespace WebShop.Common.Interfaces;

public interface IEntity<T>
{
    T Id { get; set; }
}