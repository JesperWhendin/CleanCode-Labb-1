using Microsoft.EntityFrameworkCore;
using WebShop.Notifications;
using WebShop.Repositories;
using WebShop.SQL;
using WebShop.SQL.Entities;
using WebShop.SQL.Interfaces;
using WebShop.SQL.Repositories;

namespace WebShop.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly SqlWebShopDbContext _context;
        private readonly DbSet<Product> _dbSet;
        public IProductRepository Products { get; set; }


        
        // private readonly ProductSubject _productSubject;

        
        public UnitOfWork(SqlWebShopDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            Products = new ProductRepository(_context);



            // Om inget ProductSubject injiceras, skapa ett nytt
            // _productSubject = productSubject ?? new ProductSubject();

            // Registrera standardobservatörer
            // _productSubject.Attach(new EmailNotification());
        }

        //public void NotifyProductAdded(Product product) // Borde inte vara product entity - Skapa DTO typ
        //{
        //    _productSubject.Notify(product);
        //}

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
