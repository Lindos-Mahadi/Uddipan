using gBanker.Data.CodeFirstMigration.InfrastructureBase;
using gBanker.Data.CodeFirstMigration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gBanker.Data.Repository
{
    public interface IDurationRepository : IRepository<DurationTable>
    {
        IEnumerable<DurationTable> getDurationList();

    }
    public class DurationRepository : RepositoryBaseCodeFirst<DurationTable>, IDurationRepository
    {
        public DurationRepository(IDatabaseFactoryCodeFirst databaseFactory) : base(databaseFactory)
        {
        }

        public IEnumerable<DurationTable> getDurationList()
        {
            try
            {
                var sqlCommand = "select Frequency, Duration from DurationTable ";
                var results = DataContext.Database.SqlQuery<DurationTable>(sqlCommand).ToList();

                return results;
            }
            catch (Exception ex)
            {
                return new List<DurationTable>();
            }
        }
    }
}
