using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gBanker.Data.CodeFirstMigration
{
    [Table("Saving")]
    public class SavingsAccClose
    {
        public long Id { get; set; }
        public string Status { get; set; }
        public long MemberID { get; set; }
        public long OfficeID { get; set; }
        public long SavingAccountID { get; set; }
        public string CreateUser { get; set; } = string.Empty;
        public DateTime? CreateDate { get; set; } = DateTime.Now;
        public string UpdateUser { get; set; } = string.Empty;
        public DateTime? UpdateDate { get; set; }
    }
}
