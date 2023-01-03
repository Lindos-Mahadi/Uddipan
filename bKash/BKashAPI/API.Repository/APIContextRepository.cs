using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using API.Repository;
using System.Data.Entity;
using API.DataModel;

namespace API.ContextRepository
{
    public class APIContextRepository<T> : IRepository<T> where T : class
    {
        private readonly gBankerBUROAPIEntities _context;
        private readonly DbSet<T> _dbset;
        public APIContextRepository(APIContextUow uow)
        {
            _context = uow.Context;
            _dbset = _context.Set<T>();
        }

        public virtual void Add(T entity)
        {
            _dbset.Add(entity);
        }

        public virtual void Delete(T entity)
        {
            _dbset.Remove(entity);
        }

        public virtual void Update(T entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
        }
        public virtual T GetById(long id)
        {
            return _dbset.Find(id);
        }

        public IQueryable<T> All()
        {
            return _dbset;
        }
        public IQueryable<T> AllReadOnly()
        {
            return _dbset.AsNoTracking();
        }
        public void Dispose()
        {
            _context.Dispose();
        }

        public IQueryable<T> GetMany(Expression<Func<T, bool>> where)
        {
            return where == null ? _dbset : _dbset.Where(where);
        }
    }
}
