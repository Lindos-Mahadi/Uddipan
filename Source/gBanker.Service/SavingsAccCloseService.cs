using gBanker.Core.Common;
using gBanker.Data.CodeFirstMigration.InfrastructureBase;
using gBanker.Data.CodeFirstMigration.Db;
using gBanker.Data.DBDetailModels;
using gBanker.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using gBanker.Data.CodeFirstMigration;

namespace gBanker.Service
{
    public interface ISavingsAccCloseService : IServiceBase<SavingsAccClose>
    {


    }
    public class SavingsAccCloseService : ISavingsAccCloseService
    {
        private readonly ISavingsAccCloseRepository repository;
        private readonly IUnitOfWorkCodeFirst unitOfWork;
        public SavingsAccCloseService(ISavingsAccCloseRepository repository, IUnitOfWorkCodeFirst unitOfWork)
        {
            this.repository = repository;
            this.unitOfWork = unitOfWork;
        }

        public SavingsAccClose Create(SavingsAccClose objectToCreate)
        {

           repository.Add(objectToCreate);
            Save();
            return objectToCreate;

        }

        public void Delete(int id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<SavingsAccClose> GetAll()
        {
            var pendingList = repository.GetMany(t=> t.Status == "P");
            return pendingList;
        }

        public SavingsAccClose GetById(int id)
        {
            throw new NotImplementedException();
        }

        public SavingsAccClose GetByIdLong(long id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<SavingsAccClose> GetMany(Expression<Func<SavingsAccClose, bool>> where)
        {
            throw new NotImplementedException();
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

        public void Update(SavingsAccClose objectToUpdate)
        {
            throw new NotImplementedException();
        }
    }

}
