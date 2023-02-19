using gBanker.Core.Common;
using gBanker.Data.CodeFirstMigration;
using gBanker.Data.CodeFirstMigration.InfrastructureBase;
using gBanker.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace gBanker.Service
{
    public interface ILoanAccRescheduleService : IServiceBase<LoanAccReschedule>
    {
      
    }
    public class LoanAccRescheduleService : ILoanAccRescheduleService
    {
        private readonly ILoanAccRescheduleRepository repository;
        private readonly IUnitOfWorkCodeFirst unitOfWork;

        public LoanAccRescheduleService(ILoanAccRescheduleRepository repository, IUnitOfWorkCodeFirst unitOfWork)
        {
            this.repository = repository;
            this.unitOfWork = unitOfWork;
        }
        public IEnumerable<LoanAccReschedule> GetAll()
        {
            var entities = repository.GetMany(t => t.Status == "P");
            return entities;
        }

        public LoanAccReschedule GetById(int id)
        {
            var entity = repository.GetById(id);
            return entity;
        }
       

        public LoanAccReschedule Create(LoanAccReschedule objectToCreate)
        {
            repository.Add(objectToCreate);
            Save();
            return objectToCreate;
        }

        public void Update(LoanAccReschedule objectToUpdate)
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
            //throw new NotImplementedException();
            unitOfWork.Commit();
        }
        public bool Inactivate(long id, DateTime? inactiveDate)
        {
            throw new NotImplementedException();
        }
        public bool IsContinued(long id)
        {
            throw new NotImplementedException();
        }

        public LoanAccReschedule GetByIdLong(long id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<LoanAccReschedule> GetMany(Expression<Func<LoanAccReschedule, bool>> where)
        {
            var entities = repository.GetMany(where).Where(b => b.Status == "P");
            return entities;
        }
    }
}
