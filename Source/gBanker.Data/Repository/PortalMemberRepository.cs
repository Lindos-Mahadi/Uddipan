using gBanker.Data.CodeFirstMigration.InfrastructureBase;
using gBanker.Data.CodeFirstMigration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gBanker.Data.Repository
{
    public interface IPortalMemberRepository : IRepository<PortalMember>
    {

    }
    public class PortalMemberRepository : RepositoryBaseCodeFirst<PortalMember>, IPortalMemberRepository
    {
        public PortalMemberRepository(IDatabaseFactoryCodeFirst databaseFactory) 
            : base(databaseFactory)
        {
        }
    }
}
