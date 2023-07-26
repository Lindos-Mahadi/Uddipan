using gBanker.Data.CodeFirstMigration.InfrastructureBase;
using gBanker.Data.CodeFirstMigration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using gBanker.Data.DBDetailModels;

namespace gBanker.Data.Repository
{
    public interface IDurationRepository : IRepository<DurationTable>
    {
        IEnumerable<DurationTable> getDurationList();
        IEnumerable<DurationTableModel> getDurationItemList();

    }
    public class DurationRepository : RepositoryBaseCodeFirst<DurationTable>, IDurationRepository
    {
        public DurationRepository(IDatabaseFactoryCodeFirst databaseFactory) : base(databaseFactory)
        {
        }

        public IEnumerable<DurationTableModel> getDurationItemList()
        {
            try
            {
                var sqlCommand = "select Frequency, Duration from DurationTable ";
                var results = DataContext.Database.SqlQuery<DurationTableModel>(sqlCommand).ToList();

                return results;
            }
            catch (Exception ex)
            {
                return new List<DurationTableModel>();
            }
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
