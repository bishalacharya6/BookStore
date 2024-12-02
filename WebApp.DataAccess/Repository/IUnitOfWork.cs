using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Web.DataAccess.Repository
{
    public interface IUnitOfWork    
    {
        ICatagoryRepository Catagory { get; }
        IProductRepository Product { get; }

        void Save();
    }
}
