using gBanker.Data.CodeFirstMigration;
using gBanker.Data.CodeFirstMigration.InfrastructureBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gBanker.Data.Repository
{
    public interface INotificationTableRepository : IRepository<NotificationTable>
    { 

    }
    public class NotificationTableRepository : RepositoryBaseCodeFirst<NotificationTable>, INotificationTableRepository
    {
        public NotificationTableRepository(IDatabaseFactoryCodeFirst databaseFactory) : base(databaseFactory)
        {

        }
    }
}
