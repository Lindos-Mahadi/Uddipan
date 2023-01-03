using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace API.Repository
{
    public interface IRepository<T> : IDisposable where T : class
    {
        void Add(T entity);
        void Delete(T entity);
        void Update(T entity);
        T GetById(long id);
        IQueryable<T> All();
        IQueryable<T> AllReadOnly();
        IQueryable<T> GetMany(Expression<Func<T, bool>> where);
    }
}
