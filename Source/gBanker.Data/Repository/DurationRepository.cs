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
    public interface IDurationRepository : IRepository<Duration>
    {
        IEnumerable<Duration> getDurationList();
        IEnumerable<DurationModel> getDurationItemList();

    }
    public class DurationRepository : RepositoryBaseCodeFirst<Duration>, IDurationRepository
    {
        public DurationRepository(IDatabaseFactoryCodeFirst databaseFactory) : base(databaseFactory)
        {
        }

        public IEnumerable<DurationModel> getDurationItemList()
        {
            try
            {
                var sqlCommand = "select ID, Frequency, DurationName, ProductPaymentFrequency from Duration ";
                var results = DataContext.Database.SqlQuery<DurationModel>(sqlCommand).ToList();

                return results;
            }
            catch (Exception ex)
            {
                return new List<DurationModel>();
            }
        }

        public IEnumerable<Duration> getDurationList()
        {
            try
            {
                var sqlCommand = "select Frequency, DurationName, ProductPaymentFrequency from Duration ";
                var results = DataContext.Database.SqlQuery<Duration>(sqlCommand).ToList();

                return results;
            }
            catch (Exception ex)
            {
                return new List<Duration>();
            }
        }
    }
}
