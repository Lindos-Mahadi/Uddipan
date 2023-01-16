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
    public interface IPortalMemberService : IServiceBase<PortalMember>
    {
    }
    public class PortalMemberService : IPortalMemberService
    {
        private readonly IPortalMemberRepository repository;
        private readonly IUnitOfWorkCodeFirst unitOfWork;

        public PortalMemberService(IPortalMemberRepository repository, IUnitOfWorkCodeFirst unitOfWork)
        {
            this.repository = repository;
            this.unitOfWork = unitOfWork;
        }

        public PortalMember Create(PortalMember objectToCreate)
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

        public IEnumerable<PortalMember> GetAll()
        {
            var entities = repository.GetAll().OrderBy(c => c.FirstName);
            return entities;
        }

        public PortalMember GetById(int id)
        {
            var entity = repository.GetById(id);
            return entity;
        }

        public PortalMember GetByIdLong(long id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<PortalMember> GetMany(Expression<Func<PortalMember, bool>> where)
        {
            var portalMembers = repository.GetMany(where);
            return portalMembers;
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

        public void Update(PortalMember objectToUpdate)
        {
            repository.Update(objectToUpdate);
            Save();
        }
    }
}
