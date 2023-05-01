using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gBanker.Data.CodeFirstMigration
{
    [Table("NotificationTable")]
    public partial class NotificationTable
    {
        public long Id { get; set; }
        public string Message { get; set; }
        [StringLength(50)]
        public string SenderType { get; set; }
        public long? SenderID { get; set; }
        [StringLength(50)]
        public string ReceiverType { get; set; }
        public long? ReceiverID { get; set; }
        public bool Email { get; set; }
        public bool SMS { get; set; }
        public bool Push { get; set; }
        public string Status { get; set; }
        public DateTime? CreateDate { get; set; }
        //[StringLength(15)]
        public string CreateUser { get; set; }
        public DateTime? UpdateDate { get; set; }
        //[StringLength(15)]
        public string UpdateUser { get; set; }

    }
}
