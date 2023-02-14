using gBanker.Data.CodeFirstMigration;
using gBanker.Data.CodeFirstMigration.InfrastructureBase;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace gBanker.Data.Repository
{
    public interface IFileRepository: IRepository<FileUploadTable>
    {
        IEnumerable<FileUploadTable> GetByListOfIds(List<long> Ids);
        IEnumerable<FileUploadTable> CreateMany(IEnumerable<FileUploadTable> files);
    }

    public class FileRepository: RepositoryBaseCodeFirst<FileUploadTable>, IFileRepository
    {
        private readonly IDatabaseFactoryCodeFirst databaseFactory;
        public FileRepository(IDatabaseFactoryCodeFirst databaseFactory)
            : base(databaseFactory)
        {
            this.databaseFactory = databaseFactory;
        }

        public IEnumerable<FileUploadTable> GetByListOfIds(List<long> Ids)
        {
            var files = DataContext.FileUploadTable.Where(f => Ids.Contains(f.FileUploadId));
            return files;
        }

        public IEnumerable<FileUploadTable> CreateMany(IEnumerable<FileUploadTable> files)
        {
            var uploadedFiles = DataContext.FileUploadTable.AddRange(files);
            return uploadedFiles;
        }

    }
}
