using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Web.DataAccess.Data;
using Web.DataAccess.Repository;
using Web.Models;

namespace Web.DataAccess
{
    public class CatagoryRepository : Repository<Catagory>, ICatagoryRepository
    {
        private readonly ApplicationDbContext _db;
        public CatagoryRepository(ApplicationDbContext db) : base(db) 
        {
            _db = db;
        }
        public void Update(Catagory obj)
        {
            _db.Catagories.Update(obj);
        }
    }
}
