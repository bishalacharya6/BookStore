using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Web.DataAccess.Data;
using Web.DataAccess.Repository;

namespace Web.DataAccess
{
    public class UnitOfWork : IUnitOfWork
    {
        private ApplicationDbContext _db;
        public ICatagoryRepository Catagory { get; private set; }
        public IProductRepository Product { get; private set; }
        public ICompanyRepository Company { get; private set; }
        public IApplicationUserRepository ApplicationUser { get; private set; }
        public IShoppingCartRepository ShoppingCart { get; private set; }
        public IOrderDetailRepository OrderDetail { get; private set; }
        public IOrderHeaderRepository OrderHeader { get; private set; }

        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;
            ApplicationUser = new ApplicationUserRepository(_db);
            ShoppingCart = new ShoppingCartRepository(_db);
            Catagory = new CatagoryRepository(_db);
            Product = new ProductRepository(_db);   
            Company = new CompanyRepository(_db);   
            OrderHeader = new OrderHeaderRepository(_db);   
            OrderDetail = new OrderDetailRepository(_db);    
        }

        public void Save()
        {
            _db.SaveChanges();
        }

    }
}
