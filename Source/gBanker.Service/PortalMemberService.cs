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
        private readonly INotificationTableService notificationTable;

        public PortalMemberService(IPortalMemberRepository repository, IUnitOfWorkCodeFirst unitOfWork, INotificationTableService notificationTable)
        {
            this.repository = repository;
            this.unitOfWork = unitOfWork;
            this.notificationTable = notificationTable;
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
            //if (objectToUpdate.ApprovalStatus == true && objectToUpdate.SavingStatus == 2)
            //{
            //    NotificationTable notification = new NotificationTable
            //    {
            //        Message = "Your Savings account is Approved",
            //        SenderType = "SavingAccountOpening",
            //        SenderID = (long)objectToUpdate,
            //        ReceiverType = "Good",
            //        ReceiverID = (long)objectToUpdate.MemberID,
            //        Email = true,
            //        SMS = true,
            //        Push = true,
            //        Status = "A",
            //        CreateDate = DateTime.UtcNow,
            //        UpdateDate = DateTime.UtcNow,
            //        CreateUser = "Admin",
            //        UpdateUser = "Admin"
            //    };
            //    notificationTable.Create(notification);
            //}
            ////else
            ////{
            ////    notification.Message = "";
            ////    notificationTable.Create(notification);
            ////}
            repository.Update(objectToUpdate);
            Save();
        }
    }
}
