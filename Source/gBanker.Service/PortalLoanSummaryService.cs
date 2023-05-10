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
        private readonly INotificationTableService notificationTable;

        public PortalLoanSummaryService(IPortalLoanSummaryRepository repository, IUnitOfWorkCodeFirst unitOfWork, INotificationTableService notificationTable)
        {
            this.repository = repository;
            this.unitOfWork = unitOfWork;
            this.notificationTable = notificationTable;
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
            var entity = repository.GetByIdLong(id);
            return entity;
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
            if (objectToUpdate.ApprovalStatus == true && objectToUpdate.LoanStatus == 2)
            {
                NotificationTable notification = new NotificationTable
                {
                    Message = "Your Loan is Approved",
                    SenderType = "LoanApproval",
                    SenderID = (long)objectToUpdate.PortalLoanSummaryID,
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
