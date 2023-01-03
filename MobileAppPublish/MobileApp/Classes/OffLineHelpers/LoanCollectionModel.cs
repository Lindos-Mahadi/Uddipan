using System.Collections.Generic;
using Newtonsoft.Json;

namespace PMS.Droid.Classes.OffLineHelpers
{
    public class LoanProposalModel
    {
        public int ProposalID { get; set; }
        public string ProductName { get; set; }
        public string PurposeName { get; set; }
        public string CenterName { get; set; }
        public string OfficeName { get; set; }
        public double Amount { get; set; }
        public string MemberCode { get; set; }

        public int ProductID { get; set; }
        public int PurposeID { get; set; }
        public int CenterID { get; set; }
        public int OfficeID { get; set; }
    }
    public class CollectionAPIModel
    {
        public string CollectionDate { get; set; }
        public string UserId { get; set; }
        public int APIVersion { get; set; }
        public List<LoanCollectionModel> Collections { get; set; }
    }
    public class LoanCollectionModel
    {
        public int CollectionID { get; set; }
        public string ProductName { get; set; }      
        public string CenterName { get; set; }
        public string OfficeName { get; set; }
        public double Amount { get; set; }
        public string MemberCode { get; set; }
        public int ProductID { get; set; }       
        public int CenterID { get; set; }
        public int OfficeID { get; set; }
        public long MemberID { get; set; }
        public double DueAmount { get; set; }
        [JsonProperty("TType")]
        public int TrxType { get; set; }
        [JsonProperty("PType")]
        public int ProductType { get; set; }
        public int SyncFlag { get; set; }
        public string Token { get; set; }
        [JsonProperty("Sid")]
        public long SummaryID { get; set; }
        public double IntCharge { get; set; }
        public double LoanInstallment { get; set; }
        public double IntInstallment { get; set; }
        public double fine { get; set; }
        public int CollectionType { get; set; } //KHALID: 1 For Collected, 2 For not collected yet.
        public string Created { get; set; }
    }
    public class LoanCollectionSummaryModel
    {
        public string CenterName { get; set; }
        public string ProductName { get; set; }
        public string Receivable { get; set; }
        public string Collection { get; set; }
        public string Due { get; set; }
        public double DueAmount { get; set; }
        public double CollectionAmount { get; set; }
        public double ReceivableAmount { get; set; }
        public int RecordSequence { get; set; }
        public string RecordType { get; set; }
    }
    public class OfficeModel
    {
        public int OfficeID { get; set; }      
        public string OfficeName { get; set; }
    }

    public class CenterModel
    {
        public int OfficeID { get; set; }
        public string OfficeName { get; set; }
        public int CenterID { get; set; }
        public string CenterName { get; set; }
    }

    public class MemberModel
    {
        public long MemberID { get; set; }
        public string MemberName { get; set; }
        public string MemberCode { get; set; }
        public int OfficeID { get; set; }
        public string OfficeName { get; set; }
        public int CenterID { get; set; }
        public string CenterName { get; set; }
        public int TrxType { get; set; }
    }


    public class ProductModel
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; }       
    }
   
    public class SystemErrorModel
    {
        public int ID { get; set; }
        public string ActionName { get; set; }
        public string ErrorText { get; set; }
        public string InputParameters { get; set; }
        public string UserID { get; set; }
        public string CreateDate { get; set; }
    }
    public class OrganizationModel
    {
        public string Name { get; set; }
        public string OrganizationUrl { get; set; }
    }
    public class MemberProductModel: Java.Lang.Object
    {
        public long MemberID { get; set; }
        public string MemberName { get; set; }
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public int ProductType { get; set; }
        public double LoanRecovery { get; set; }
        public double Recoverable { get; set; }
        public double Balance { get; set; }
        public double PrinBalance { get; set; }
        public double SerBalance { get; set; }
        public int InstallmentNo { get; set; }
        public double TodayCollectionAmount { get; set; }
        public int TrxType { get; set; }
        public long SummaryID { get; set; }
        public string InterestCalculationMethod { get; set; }
        public int Duration { get; set; }
        public double DurationOverLoanDue { get; set; }
        public double DurationOverIntDue { get; set; }
        public double LoanDue { get; set; }               
        public double IntDue { get; set; }
        public double CumIntCharge { get; set; }
        public double CumInterestPaid { get; set; }
               
        public double PrincipalLoan { get; set; }
        public double LoanRepaid { get; set; }
        public double IntCharge { get; set; }
        public double LoanInstallment { get; set; }
        public double IntInstallment { get; set; }
        public double NewDue { get; set; }
        public double PersonalSaving { get; set; }
        public double CumInterest { get; set; }
        public double PersonalWithdraw { get; set; }
        public double SCPaid { get; set; }
        public string MainProductCode { get; set; }

        public double GSCollected { get; set; }
        public double VSCollected { get; set; }
        public double LTSCollected { get; set; }

        public double GSAmount { get; set; }
        public double VSAmount { get; set; }
        public double LTSAmount { get; set; }
        public MemberProductModel GSID { get; set; }
        public MemberProductModel LTSID { get; set; }
        public MemberProductModel VSID { get; set; }
        public int CenterID { get; set; }
        public bool IsCollected { get; set; }
        public int Doc { get; set; }
        public int OrgID { get; set; }

        // Himel added
        public string accountNo { get; set; }
        public double fine { get; set; }
    }
    public class PurposeModel
    {
        public int PurposeID { get; set; }
        public string PurposeName { get; set; }
    }

    public class LoanCollectionListModel
    {
        public long MemberID { get; set; }
        public string MemberName { get; set; }
        public string LoanAmount { get; set; }
        public string DueAmount { get; set; }
        public string SavingsAmount { get; set; }
        public string TotalAmount { get; set; }
        public bool IsSynced { get; set; }
        public string RecordType { get; set; }
        public bool HasDue { get; set; }

        public double LoanAmountDbl { get; set; }
        public double DueAmountDbl { get; set; }
        public double SavingsAmountDbl { get; set; }
        public double TotalAmountDbl { get; set; }
    }
}