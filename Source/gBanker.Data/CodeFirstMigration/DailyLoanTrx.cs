namespace gBanker.Data.CodeFirstMigration.Db
{
    using gBankerCodeFirstMigration.Db;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("DailyLoanTrx")]
    public partial class DailyLoanTrx
    {
        public long DailyLoanTrxID { get; set; }

        [Column(TypeName = "date")]
        public DateTime TrxDate { get; set; }

        public long LoanSummaryID { get; set; }

        [StringLength(100)]
        public string LoanNo { get; set; }

        public int OfficeID { get; set; }

        public long MemberID { get; set; }

        [StringLength(20)]
        public string MemberCode { get; set; }

        [StringLength(110)]
        public string MemberName { get; set; }

        public short ProductID { get; set; }

        [StringLength(10)]
        public string ProductCode { get; set; }

        [StringLength(50)]
        public string ProductName { get; set; }

        [StringLength(1)]
        public string InterestCalculationMethod { get; set; }

        public int CenterID { get; set; }

        public byte MemberCategoryID { get; set; }

        public int LoanTerm { get; set; }

        public short PurposeID { get; set; }

        [Column(TypeName = "date")]
        public DateTime InstallmentDate { get; set; }

        [Column(TypeName = "numeric")]
        public decimal PrincipalLoan { get; set; }

        [Column(TypeName = "numeric")]
        public decimal LoanRepaid { get; set; }

        [Column(TypeName = "numeric")]
        public decimal LoanDue { get; set; }

        [Column(TypeName = "numeric")]
        public decimal LoanPaid { get; set; }

        [Column(TypeName = "numeric")]
        public decimal CumIntCharge { get; set; }

        [Column(TypeName = "numeric")]
        public decimal IntCharge { get; set; }

        [Column(TypeName = "numeric")]
        public decimal IntDue { get; set; }

        [Column(TypeName = "numeric")]
        public decimal IntPaid { get; set; }

        [Column(TypeName = "numeric")]
        public decimal Advance { get; set; }

        [Column(TypeName = "numeric")]
        public decimal DueRecovery { get; set; }

        public byte TrxType { get; set; }

        public short InstallmentNo { get; set; }
        public byte InvestorID { get; set; }
        public long EmployeeID { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? TotalPaid { get; set; }

        [StringLength(5)]
        public string CollectionStatus { get; set; }

        public bool? IsActive { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime? InActiveDate { get; set; }
        public string CollectionType { get; set; }
        [Required]
        [StringLength(35)]
        public string CreateUser { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime CreateDate { get; set; }
        public string PhoneNo { get; set; }
        public string LoanAccountNo { get; set; }


        public string BankName { get; set; }
        public string ChequeNo { get; set; }

        /// <summary>
        /// New Filed Add For Duration Over

        /// </summary>

        public int Duration { get; set; }

        [Column(TypeName = "numeric")]
        public decimal DurationOverLoanDue { get; set; }

        [Column(TypeName = "numeric")]
        public decimal DurationOverIntDue { get; set; }
        public int OrgID { get; set; }
        public decimal CumLoanDue { get; set; }
        public decimal CumIntDue { get; set; }

        public string MainProductCode { get; set; }
        public int MainProductID { get; set; }
        public virtual Organization Organization { get; set; }
        public virtual Center Center { get; set; }

        public virtual LoanSummary LoanSummary { get; set; }

        public virtual Member Member { get; set; }

        public virtual Office Office { get; set; }

        public virtual Product Product { get; set; }
    }
}
