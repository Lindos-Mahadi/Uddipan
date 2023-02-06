using gBanker.Data.CodeFirstMigration;
using gBanker.Data.CodeFirstMigration.InfrastructureBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gBanker.Data.Repository
{
    public interface IFileRepository: IRepository<FileUploadTable>
    {
    }

    public class FileRepository: RepositoryBaseCodeFirst<FileUploadTable>, IFileRepository
    {
        public FileRepository(IDatabaseFactoryCodeFirst databaseFactory)
            : base(databaseFactory)
        {

        }
    }
}
