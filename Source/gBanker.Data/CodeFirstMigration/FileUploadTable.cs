using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gBanker.Data.CodeFirstMigration
{
    [Table("FileUploadTable")]
    public class FileUploadTable
    {
        [Key]
        public long FileUploadId { get; set; }
        public string EntityName { get; set; }
        public long EntityId { get; set; }
        public string PropertyName { get; set; }
        public byte[] File { get; set; }
        public string FileName { get; set; }
        public string Type { get; set; }
    }
}
