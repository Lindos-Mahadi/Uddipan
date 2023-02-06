using gBanker.Data.CodeFirstMigration;
using gBanker.Data.CodeFirstMigration.InfrastructureBase;
using gBanker.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace gBanker.Service
{
    public interface IFileService: IServiceBase<FileUploadTable>
    {
    }

    public class FileService : IFileService
    {
        public FileUpload Create(FileUpload objectToCreate)
        {
            throw new NotImplementedException();
        }
        private readonly IFileRepository repository;
        private readonly IUnitOfWorkCodeFirst unitOfWork;

        public FileService(IFileRepository repository, IUnitOfWorkCodeFirst unitOfWork)
        {
            this.repository = repository;
            this.unitOfWork = unitOfWork;
        }

        public void Delete(int id)
        {
            var file = repository.GetById(id);
            repository.Delete(file);
            Save();
        }

        public IEnumerable<FileUploadTable> GetAll()
        {
            var files = repository.GetAll();
            return files;
        }

        public FileUploadTable GetById(int id)
        {
            var file = repository.GetById(id);
            return file;
        }

        public FileUploadTable GetByIdLong(long id)
        {
            var file = repository.GetById(id);
            return file;
        }

        public IEnumerable<FileUploadTable> GetMany(Expression<Func<FileUploadTable, bool>> where)
        {
            var files = repository.GetMany(where);
            return files;
        }

        public bool Inactivate(long id, DateTime? inactiveDate)
        {
            throw new NotImplementedException();
        }

        public bool IsContinued(long id)
        {
            throw new NotImplementedException();
        }

        public void Save()
        {
            unitOfWork.Commit();
        }

        public void Update(FileUploadTable objectToUpdate)
        {
            repository.Update(objectToUpdate);
            Save();
        }

        public FileUploadTable Create(FileUploadTable objectToCreate)
        {
            repository.Add(objectToCreate);
            Save();
            return objectToCreate;
        }
    }
}
