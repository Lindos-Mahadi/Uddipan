using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace gBanker.Web.ViewModels
{
    public class NomineeXPortalSavingSummaryViewModel
    {
        [Key]
        public long PortalMemberNomineeId { get; set; }

        public string NomineeName { get; set; }
        public string NFatherName { get; set; }
        public string NRelationName { get; set; }
        public string NAddressName { get; set; }
        public int? NAlocation { get; set; }
        public long ImageId { get; set; }
        public long NIDId { get; set; }
        public long PortalSavingSummaryID { get; set; }
    }
}