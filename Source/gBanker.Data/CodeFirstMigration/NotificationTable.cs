using System;
using System.Collections.Generic;
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
        public string SenderType { get; set; }
        public long? SenderID { get; set; }
        public string ReceiverType { get; set; }
        public long? ReceiverID { get; set; }
        public bool Sms { get; set; }
        public bool Email { get; set; }
        public bool Push { get; set; }
    }
}
