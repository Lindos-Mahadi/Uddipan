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
    public interface IPortalSavingSummaryRepository : IRepository<PortalSavingSummary>
    {

    }
    public class PortalSavingSummaryRepository : RepositoryBaseCodeFirst<PortalSavingSummary>, IPortalSavingSummaryRepository
    {
        public PortalSavingSummaryRepository(IDatabaseFactoryCodeFirst databaseFactory) : base(databaseFactory)
        {
        }
    }
}
