using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace gBanker.Web.ViewModels
{
    public class LoanAccRescheduleViewModel
    {
        public long Id { get; set; }
        public long MemberID { get; set; }
        public long OfficeID { get; set; }
        public long LoanID { get; set; }
        public string CreateUser { get; set; } = string.Empty;
        public DateTime? CreateDate { get; set; } = DateTime.Now;
        public string UpdateUser { get; set; } = string.Empty;
        public DateTime? UpdateDate { get; set; }
        public string Status { get; set; }

    }
}