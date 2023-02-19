using gBanker.Data.CodeFirstMigration;
using gBanker.Data.CodeFirstMigration.InfrastructureBase;
using gBanker.Data.DBDetailModels;
using System.Collections.Generic;
using System.Linq;

namespace gBanker.Data.Repository
{
    public interface ISavingsAccCloseRepository : IRepository<SavingsAccClose>
    {
       
    }
    public class SavingsAccCloseRepository : RepositoryBaseCodeFirst<SavingsAccClose>, ISavingsAccCloseRepository
    {
        public SavingsAccCloseRepository(IDatabaseFactoryCodeFirst databaseFactory)
            : base(databaseFactory)
        {

        }
        
    }
}
