using gBanker.Data.CodeFirstMigration;
using gBanker.Data.CodeFirstMigration.InfrastructureBase;
using gBanker.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace gBanker.Service
{
    public interface IPortalLoanSummaryService : IServiceBase<PortalLoanSummary>
    {
    }
    public class PortalLoanSummaryService : IPortalLoanSummaryService
    {
        private readonly IPortalLoanSummaryRepository repository;
        private readonly IUnitOfWorkCodeFirst unitOfWork;

        public PortalLoanSummaryService(IPortalLoanSummaryRepository repository, IUnitOfWorkCodeFirst unitOfWork)
        {
            this.repository = repository;
            this.unitOfWork = unitOfWork;
        }

        public PortalLoanSummary Create(PortalLoanSummary objectToCreate)
        {
            repository.Add(objectToCreate);
            Save();
            return objectToCreate;
        }

        public void Delete(int id)
        {
            var entity = repository.GetById(id);
            repository.Delete(entity);
            Save();
        }

        public IEnumerable<PortalLoanSummary> GetAll()
        {
            var entities = repository.GetAll();
            return entities;
        }

        public PortalLoanSummary GetById(int id)
        {
            var entity = repository.GetById(id);
            return entity;
        }

        public PortalLoanSummary GetByIdLong(long id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<PortalLoanSummary> GetMany(Expression<Func<PortalLoanSummary, bool>> where)
        {
            var PortalLoanSummarys = repository.GetMany(where);
            return PortalLoanSummarys;
        }

        public bool Inactivate(long id, DateTime? inactiveDate)
        {
            throw new NotImplementedException();
        }

        public bool IsContinued(long id)
        {
            throw new NotImplementedException();
        }

        public void Save()
        {
            unitOfWork.Commit();
        }

        public void Update(PortalLoanSummary objectToUpdate)
        {
            repository.Update(objectToUpdate);
            Save();
        }
    }
}
