using WebShop.Repositories;
using WebShop.SQL.Interfaces;

namespace WebShop.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        IProductRepository Products { get; }
        void Save();
    }
}

