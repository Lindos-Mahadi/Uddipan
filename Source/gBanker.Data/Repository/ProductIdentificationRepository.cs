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
    public interface IProductIdentificationRepository : IRepository<ProductIdentification>
    {
        IEnumerable<ProductIdentification> getProductIdentificationList();
        IEnumerable<ProductIdentificationModel> getProductIdentificationItemList();
    }
    public class ProductIdentificationRepository : RepositoryBaseCodeFirst<ProductIdentification>, IProductIdentificationRepository
    {
        public ProductIdentificationRepository(IDatabaseFactoryCodeFirst databaseFactory) : base(databaseFactory)
        {
        }

        public IEnumerable<ProductIdentificationModel> getProductIdentificationItemList()
        {
            try
            {
                var sqlCommand = "select IdentificationName from ProductIdentification";
                var results = DataContext.Database.SqlQuery<ProductIdentificationModel>(sqlCommand).ToList();
                return results;
            }
            catch (Exception ex)
            {
                return new List<ProductIdentificationModel>();
            }
        }

        public IEnumerable<ProductIdentification> getProductIdentificationList()
        {
            try
            {
                var sqlCommand = "select IdentificationName from ProductIdentification";
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

