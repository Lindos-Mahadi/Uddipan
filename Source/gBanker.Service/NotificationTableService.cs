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
    public interface INotificationTableService : IServiceBase<NotificationTable>
    {

    }
    public class NotificationTableService : INotificationTableService
    {
        private readonly INotificationTableRepository repository;
        private readonly IUnitOfWorkCodeFirst unitOfWork;

        public NotificationTable Create(NotificationTable objectToCreate)
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

        public IEnumerable<NotificationTable> GetAll()
        {
            var entities = repository.GetAll().OrderBy(c => c.SenderID);
            return entities;
        }

        public NotificationTable GetById(int id)
        {
            var entity = repository.GetById(id);
            return entity;
        }

        public NotificationTable GetByIdLong(long id)
        {
            var entity = repository.GetById(id);
            return entity;
        }

        public IEnumerable<NotificationTable> GetMany(Expression<Func<NotificationTable, bool>> where)
        {
            var notification = repository.GetMany(where);
            return notification;
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

        public void Update(NotificationTable objectToUpdate)
        {
            repository.Update(objectToUpdate);
            Save();
        }
    }
}
