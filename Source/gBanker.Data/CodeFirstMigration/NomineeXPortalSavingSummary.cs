using gBanker.Data.CodeFirstMigration.Db;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gBanker.Data.CodeFirstMigration
{
    [Table("NomineeXPortalSavingSummary")]

    public class NomineeXPortalSavingSummary
    {
        [Key]
        public long PortalMemberNomineeId { get; set; }

        public string NomineeName { get; set; }
        public string NFatherName { get; set; }
        public string NRelationName { get; set; }
        public string NAddressName { get; set; }
        public int? NAlocation { get; set; }

        public long PortalSavingSummaryID { get; set; }
        //public PortalSavingSummary PortalSavingSummary { get; set; }
    }
}
