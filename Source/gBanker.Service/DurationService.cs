using gBanker.Data.CodeFirstMigration.InfrastructureBase;
using gBanker.Data.CodeFirstMigration;
using gBanker.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using gBanker.Data.DBDetailModels;

namespace gBanker.Service
{
    public interface IDurationService : IServiceBase<Duration>
    {
        IEnumerable<DurationModel> getDurationItemList();
    }
    public class DurationService : IDurationService
    {
        private readonly IDurationRepository repository;
        private readonly IUnitOfWorkCodeFirst unitOfWork;

        public DurationService(IDurationRepository repository, IUnitOfWorkCodeFirst unitOfWork)
        {
            this.repository = repository;
            this.unitOfWork = unitOfWork;
        }
        public IEnumerable<Duration> GetAll()
        {
            var entities = repository.GetAll();
            return entities;
        }
        public Duration GetById(int id)
        {
            var entity = repository.GetById(id);
            return entity;
        }
        public Duration Create(Duration objectToCreate)
        {
            repository.Add(objectToCreate);
            Save();
            return objectToCreate;
        }
        public void Update(Duration objectToUpdate)
        {
            repository.Update(objectToUpdate);
            Save();
        }
        public void Delete(int id)
        {
            var entity = repository.GetById(id);
            repository.Delete(entity);
            Save();
        }

        public void Save()
        {
            unitOfWork.Commit();
        }
        public bool Inactivate(long id, DateTime? inactiveDate)
        {
            throw new NotImplementedException();
        }

        public Duration GetByIdLong(long id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Duration> GetMany(Expression<Func<Duration, bool>> where)
        {
            throw new NotImplementedException();
        }

        public bool IsContinued(long id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<DurationModel> getDurationItemList()
        {
            try
            {
                return repository.getDurationItemList();
            }
            catch (Exception ex)
            {
                return new List<DurationModel>();
            }
        }
    }
}
