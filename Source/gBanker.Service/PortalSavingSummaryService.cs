using gBanker.Data.CodeFirstMigration;
using gBanker.Data.CodeFirstMigration.Db;
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
    public interface IPortalSavingSummaryService : IServiceBase<PortalSavingSummary>
    {

    }
    public class PortalSavingSummaryService : IPortalSavingSummaryService
    {
        private readonly IPortalSavingSummaryRepository repository;
        private readonly IUnitOfWorkCodeFirst unitOfWork;
        private readonly INotificationTableService notificationTable;

        public PortalSavingSummaryService(IPortalSavingSummaryRepository repository, IUnitOfWorkCodeFirst unitOfWork, INotificationTableService notificationTable)
        {
            this.repository = repository;
            this.unitOfWork = unitOfWork;
            this.notificationTable = notificationTable;
        }

        public PortalSavingSummary Create(PortalSavingSummary objectToCreate)
        {
            //repository.Add(objectToCreate);
            //Save();
            //return objectToCreate;
            throw new NotImplementedException();
        }

        public void Delete(int id)
        {
            //var entity = repository.GetById(id);
            //repository.Delete(entity);
            //Save();
            throw new NotImplementedException();
        }

        public IEnumerable<PortalSavingSummary> GetAll()
        {
            var entities = repository.GetAll().OrderBy(t => t.PortalSavingSummaryID);
            return entities;
        }

        public PortalSavingSummary GetById(int id)
        {
            var entity = repository.GetById(id);
            return entity;
        }

        public PortalSavingSummary GetByIdLong(long id)
        {
            var entity = repository.Get(w => w.PortalSavingSummaryID == id);
            return entity;
        }

        public IEnumerable<PortalSavingSummary> GetMany(Expression<Func<PortalSavingSummary, bool>> where)
        {
            var porSaving= repository.GetMany(where);
            return porSaving;
        }

        public bool Inactivate(long id, DateTime? inactiveDate)
        {
            //var obj = repository.GetById(id);
            //if (obj != null)
            //{
            //    obj.OfficeID = inactiveDate.HasValue ? inactiveDate : DateTime.Now;
            //    obj.IsActive = false;
            //    repository.Update(obj);
            //    Save();
            //    return true;
            //}
            //return false;
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

        public void Update(PortalSavingSummary objectToUpdate)
        {
           
            if (objectToUpdate.ApprovalStatus == true && objectToUpdate.SavingStatus == 2)
            {
                NotificationTable notification = new NotificationTable
                {
                    Message = "Your Savings account is Approved",
                    SenderType = "SavingAccountOpening",
                    SenderID = (long)objectToUpdate.PortalSavingSummaryID,
                    ReceiverType = "Approved",
                    ReceiverID = (long)objectToUpdate.MemberID,
                    Email = true,
                    SMS = true,
                    Push = true,
                    Status = "P",
                    CreateDate = DateTime.UtcNow,
                    UpdateDate = DateTime.UtcNow,
                    CreateUser = "Admin",
                    UpdateUser = "Admin"
                };
                notificationTable.Create(notification);
            }
            //else
            //{
            //    notification.Message = "";
            //    notificationTable.Create(notification);
            //}
            repository.Update(objectToUpdate);
            Save();
        }
    }
}
