using gBanker.Data.CodeFirstMigration.InfrastructureBase;
using gBanker.Data.CodeFirstMigration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gBanker.Data.Repository
{
    public interface IProductIdentificationRepository : IRepository<ProductIdentification>
    {
        IEnumerable<ProductIdentification> getProductIdentificationList();
    }
    public class ProductIdentificationRepository : RepositoryBaseCodeFirst<ProductIdentification>, IProductIdentificationRepository
    {
        public ProductIdentificationRepository(IDatabaseFactoryCodeFirst databaseFactory) : base(databaseFactory)
        {
        }

        public IEnumerable<ProductIdentification> getProductIdentificationList()
        {
            try
            {
                var sqlCommand = "select Termdeposit, VoluntarySavings, GeneralSavings, SpecialSavings from ProductIdentification ";
                var results = DataContext.Database.SqlQuery<ProductIdentification>(sqlCommand).ToList();
                return results;
            }
            catch (Exception ex)
            {
                return new List<ProductIdentification>();
            }
        }
    }
}

