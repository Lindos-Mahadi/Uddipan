using gBanker.Data.CodeFirstMigration.InfrastructureBase;
using gBanker.Data.CodeFirstMigration;
using gBanker.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace gBanker.Service
{
    public interface INomineeXPortalSavingSummaryService : IServiceBase<NomineeXPortalSavingSummary>
    {
        IEnumerable<NomineeXPortalSavingSummary> GetActiveRecords();
        IEnumerable<NomineeXPortalSavingSummary> GetSavingSummaryNominee(long id);
    }
    public class NomineeXPortalSavingSummaryService : INomineeXPortalSavingSummaryService
    {
        private readonly INomineeXPortalSavingSummaryRepository repository;
        private readonly IUnitOfWorkCodeFirst unitOfWork;

        public NomineeXPortalSavingSummaryService(INomineeXPortalSavingSummaryRepository repository, IUnitOfWorkCodeFirst unitOfWork)
        {
            this.repository = repository;
            this.unitOfWork = unitOfWork;
        }
        public IEnumerable<NomineeXPortalSavingSummary> GetAll()
        {
            var entities = repository.GetAll().OrderBy(c => c.PortalMemberNomineeId);
            return entities;
        }

        public NomineeXPortalSavingSummary GetById(int id)
        {
            var entity = repository.GetById(id);
            return entity;
        }

        public NomineeXPortalSavingSummary Create(NomineeXPortalSavingSummary objectToCreate)
        {
            repository.Add(objectToCreate);
            Save();
            return objectToCreate;
        }

        public void Update(NomineeXPortalSavingSummary objectToUpdate)
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

        public NomineeXPortalSavingSummary GetByIdLong(long id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<NomineeXPortalSavingSummary> GetActiveRecords()
        {
            return repository.GetAll();
        }
        public IEnumerable<NomineeXPortalSavingSummary> GetSavingSummaryNominee(long id)
        {
            return repository.GetMany(t => t.PortalSavingSummaryID == id);
        }

        public IEnumerable<NomineeXPortalSavingSummary> GetMany(Expression<Func<NomineeXPortalSavingSummary, bool>> where)
        {
            throw new NotImplementedException();
        }
    }
}
