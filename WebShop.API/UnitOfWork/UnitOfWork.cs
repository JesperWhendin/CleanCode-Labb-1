using Microsoft.EntityFrameworkCore;
using WebShop.Notifications;
using WebShop.SQL;
using WebShop.SQL.Entities;
using WebShop.SQL.Interfaces;
using WebShop.SQL.Repositories;

namespace WebShop.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly SqlWebShopDbContext _context;
        private readonly ProductSubject _productSubject;
        public IProductRepository Products { get; set; }

        public UnitOfWork(SqlWebShopDbContext context, ProductSubject productSubject)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            Products = new ProductRepository(_context);

            _productSubject = productSubject ?? throw new ArgumentNullException(nameof(productSubject));
        }

        public void Save()
        {
            _context.SaveChanges();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
