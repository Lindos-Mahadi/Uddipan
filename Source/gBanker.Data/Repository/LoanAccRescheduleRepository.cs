using gBanker.Data.CodeFirstMigration.InfrastructureBase;
using gBanker.Data.CodeFirstMigration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using gBanker.Data.CodeFirstMigration.Db;

namespace gBanker.Data.Repository
{
    public interface ILoanAccRescheduleRepository : IRepository<LoanAccReschedule>
    {
        IEnumerable<LoanAccReschedule> GetMemberOfficeNameList();
    }

    //public class LoanAccRescheduleViewModel
    //{
    //    public long Id { get; set; }
    //    public long MemberID { get; set; }
    //    public long OfficeID { get; set; }
    //    public long LoanID { get; set; }
    //    public string CreateUser { get; set; } = string.Empty;
    //    public DateTime? CreateDate { get; set; } = DateTime.Now;
    //    public string UpdateUser { get; set; } = string.Empty;
    //    public DateTime? UpdateDate { get; set; }
    //    public string Status { get; set; }
    //    public string OfficeName { get; set; }
    //    public string MemberCode { get; set; }
    //    public string FirstName { get; set; }
    //    public string LastName { get; set; }
    //}

    public class LoanAccRescheduleRepository : RepositoryBaseCodeFirst<LoanAccReschedule>, ILoanAccRescheduleRepository
    {
        public LoanAccRescheduleRepository(IDatabaseFactoryCodeFirst databaseFactory)
            : base(databaseFactory)
        {

        }

        public IEnumerable<LoanAccReschedule> GetMemberOfficeNameList()
        {
            var memberOfficeName = (from loanAccRes in DataContext.LoanAccReschedule
                                   join memName in DataContext.Members on loanAccRes.MemberID equals memName.MemberID
                                   select new
                                   {
                                       Id= loanAccRes.Id,
                                       MemberID= memName.MemberID,
                                       OfficeID= memName.OfficeID,
                                       LoanID= loanAccRes.LoanID,
                                       Status = "P",
                                       FirstName= memName.FirstName,
                                       LastName = memName.LastName,
                                       OfficeName = memName.Office.OfficeName
                                   });
            return (IEnumerable<LoanAccReschedule>)memberOfficeName.ToList();
        }
    }
}
