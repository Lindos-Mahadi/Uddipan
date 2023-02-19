using gBanker.Data.CodeFirstMigration.InfrastructureBase;
using gBanker.Data.CodeFirstMigration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gBanker.Data.Repository
{
    public interface ILoanAccRescheduleRepository : IRepository<LoanAccReschedule>
    {
    }
    public class LoanAccRescheduleRepository : RepositoryBaseCodeFirst<LoanAccReschedule>, ILoanAccRescheduleRepository
    {
        public LoanAccRescheduleRepository(IDatabaseFactoryCodeFirst databaseFactory)
            : base(databaseFactory)
        {

        }
    }
}
