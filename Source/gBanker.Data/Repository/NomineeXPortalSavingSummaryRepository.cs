using gBanker.Data.CodeFirstMigration.InfrastructureBase;
using gBanker.Data.CodeFirstMigration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gBanker.Data.Repository
{
    public interface INomineeXPortalSavingSummaryRepository : IRepository<NomineeXPortalSavingSummary>
    {
    }
    public class NomineeXPortalSavingSummaryRepository : RepositoryBaseCodeFirst<NomineeXPortalSavingSummary>, INomineeXPortalSavingSummaryRepository
    {
        public NomineeXPortalSavingSummaryRepository(IDatabaseFactoryCodeFirst databaseFactory)
            : base(databaseFactory)
        {

        }
    }
}
