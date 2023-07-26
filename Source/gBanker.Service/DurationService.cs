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
    public interface IDurationService : IServiceBase<DurationTable>
    {
        IEnumerable<DurationTableModel> getDurationItemList();
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
        public IEnumerable<DurationTable> GetAll()
        {
            var entities = repository.GetAll();
            return entities;
        }
        public DurationTable GetById(int id)
        {
            var entity = repository.GetById(id);
            return entity;
        }
        public DurationTable Create(DurationTable objectToCreate)
        {
            repository.Add(objectToCreate);
            Save();
            return objectToCreate;
        }
        public void Update(DurationTable objectToUpdate)
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

        public DurationTable GetByIdLong(long id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<DurationTable> GetMany(Expression<Func<DurationTable, bool>> where)
        {
            throw new NotImplementedException();
        }

        public bool IsContinued(long id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<DurationTableModel> getDurationItemList()
        {
            try
            {
                return repository.getDurationItemList();
            }
            catch (Exception ex)
            {
                return new List<DurationTableModel>();
            }
        }
    }
}
