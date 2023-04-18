using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace gBanker.Web.ViewModels
{
    [Table("LoanAccRescheduleViewModel")]
    public class LoanAccRescheduleViewModel
    {
        [Key]
        public long Id { get; set; }
        public long MemberID { get; set; }
        public long OfficeID { get; set; }
        public long LoanID { get; set; }
        public string CreateUser { get; set; } = string.Empty;
        public DateTime? CreateDate { get; set; } = DateTime.Now;
        public string UpdateUser { get; set; } = string.Empty;
        public DateTime? UpdateDate { get; set; }
        public string Status { get; set; }
        //public string OfficeCode { get; set; }
        public string OfficeName { get; set; }
        public string MemberCode { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}