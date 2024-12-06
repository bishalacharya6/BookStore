using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Web.DataAccess.Repository
{
    public interface IRepository<T> where T : class
    {
        // T is category (sometimes list, and sometime single objects)
        IEnumerable<T> GetAll(Expression<Func<T, bool>>? filter = null, string? includeProperties = null);
        T Get(Expression<Func<T, bool>> filter, string? includeProperties = null, bool tracked = false);
        void Add(T item);
        void Remove(T item);    
        void RemoveRange(IEnumerable<T> items);
    }

}
