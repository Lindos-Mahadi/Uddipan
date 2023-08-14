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
    public interface IProductIdentificationService : IServiceBase<ProductIdentification>
    {
        IEnumerable<ProductIdentification> getProductIdentificationList();
        IEnumerable<ProductIdentificationModel> getProductIdentificationItemList();
    }
    public class ProductIdentificationService : IProductIdentificationService
    {
        private readonly IProductIdentificationRepository repository;
        private readonly IUnitOfWorkCodeFirst unitOfWork;

        public ProductIdentificationService(IProductIdentificationRepository repository, IUnitOfWorkCodeFirst unitOfWork)
        {
            this.repository = repository;
            this.unitOfWork = unitOfWork;
        }


        public IEnumerable<ProductIdentification> GetAll()
        {
            var entities = repository.GetAll();
            return entities;
        }

        public ProductIdentification GetById(int id)
        {
            var entity = repository.GetById(id);
            return entity;
        }

        public ProductIdentification Create(ProductIdentification objectToCreate)
        {
            repository.Add(objectToCreate);
            Save();
            return objectToCreate;
        }
        public void Update(ProductIdentification objectToUpdate)
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

        public ProductIdentification GetByIdLong(long id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ProductIdentification> GetMany(Expression<Func<ProductIdentification, bool>> where)
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

        public IEnumerable<ProductIdentificationModel> getProductIdentificationItemList()
        {
            try
            {
                return repository.getProductIdentificationItemList();
            }
            catch (Exception ex)
            {
                return new List<ProductIdentificationModel>();
            }
        }

        public IEnumerable<ProductIdentification> getProductIdentificationList()
        {
            throw new NotImplementedException();
        }

    }
}
