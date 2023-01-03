using gBanker.Data.CodeFirstMigration;
using gBanker.Service;
using gBanker.Service.ReportExecutionService;
using gBanker.Service.ReportServies;
using gBanker.Web.Helpers;
using gBanker.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace gBanker.Web.Controllers
{
    public class FundManagementController : BaseController
    {
        #region Private Members
        public readonly IUltimateReportService ultimateReportService;
        public readonly IPKSFFundLoanService iPKSFFundLoanService;
        #endregion

        #region Ctor
        public FundManagementController(IUltimateReportService ultimateReportService, IPKSFFundLoanService iPKSFFundLoanService)
        {
            this.ultimateReportService = ultimateReportService;
            this.iPKSFFundLoanService = iPKSFFundLoanService;
        }
        #endregion

        #region Index
        public ActionResult Index()
        {
            var model = new FundLoanViewModel();
            var getFundName = ultimateReportService.GetDataWithoutParameter("GetFundLoanName");
            var getFundNameList = getFundName.Tables[0].AsEnumerable().Select(p => new SelectListItem
            {
                Text = p.Field<string>("FundLoanName"),
                Value = p.Field<int>("AccId").ToString()
            }).ToList();
            var fundLoanList = new List<SelectListItem>();
            fundLoanList.Add(new SelectListItem { Text = "Please Select", Value = "" });
            fundLoanList.AddRange(getFundNameList);
            model.FundLoanCodeList = fundLoanList;
            return View(model);
        }

        #endregion

        #region Update

        [HttpPost]
        public ActionResult Update(FundLoanViewModel model)
        {
            try
            {
                foreach (var item in model.LoanRepaymentDetailList)
                {
                    var entity = iPKSFFundLoanService.GetById(item.FundLoanID);
                    if (entity != null)
                    {
                        entity.ServiceCharge = item.ServiceCharge;
                        entity.LoanInstallmentAmount = item.LoanInstallmentAmount;
                        entity.InstallmentDate = Convert.ToDateTime(item.InstallmentDate);
                        iPKSFFundLoanService.Update(entity);
                    }
                    else
                    {
                        return GetErrorMessageResult();
                    }                    
                }                
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Json(new { Result = "OK", Message= "Updated successfully", JsonRequestBehavior.AllowGet });
        }

        #endregion

        #region Ajax Calls
        [HttpPost]
        public JsonResult GetFundManagementDetails(int jtStartIndex, int jtPageSize, string jtSorting, FundLoanViewModel fundLoanObject)
        {
            try
            {
                long TotCount;
                var param = new { @FundLoanCode = fundLoanObject.FundLoanCode, @PrincipalAmount = fundLoanObject.PrincipalAmount,
                    @LoanSanctionNo = fundLoanObject.LoanSanctionNo, @LoanSanctionTerm = fundLoanObject.LoanSanctionTerm,
                    @LoanSanctionApproveDate = fundLoanObject.LoanSanctionApproveDate,
                    @LoanDisbursementDate = fundLoanObject.LoanDisbursementDate,
                    @DisbursedAmount = fundLoanObject.DisbursedAmount,
                    @LoanDuration = fundLoanObject.LoanDuration, @InterestRate = fundLoanObject.InterestRate,
                    @GracePeriod = fundLoanObject.GracePeriod, @TotalInstallmentNo = fundLoanObject.TotalInstallmentNo,
                    @Qtype=1
                };
                var getFundData = ultimateReportService.GetDataWithParameter(param, "GeneratePKSFFundLOan");
                var getSelectedData = getFundData.Tables[0].AsEnumerable().Select(p => new FundLoanViewModel
                {
                    FundLoanID = p.Field<int>("FundLoanID"),
                    NoOfInstallment = p.Field<int>("NoOfInstallment"),
                    LoanInstallmentAmount = p.Field<decimal?>("LoanInstallmentAmount"),
                    ServiceCharge = p.Field<decimal?>("ServiceCharge"),
                    TotalPayableAmount = p.Field<decimal?>("LoanInstallmentAmount") + p.Field<decimal?>("ServiceCharge"),
                    InstallmentDate = p.Field<DateTime>("InstallmentDate").ToString("dd-MMM-yyyy")
                }).ToList();
                TotCount = getSelectedData.Count();
                var currentPageRecords = getSelectedData.ToList();
                return Json(new { Result = "OK", Records = currentPageRecords, TotalRecordCount = TotCount });
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                throw;
            }
        }

        #endregion

        #region Private Methods

        #endregion

        #region PreviewReport
        //public ActionResult Preview(string FundLoanCode, decimal PrincipalAmount, int LoanSanctionNo, int LoanSanctionTerm, 
        //    string LoanSanctionApproveDate, string LoanDisbursementDate, decimal DisbursedAmount,int InterestRate,
        //    int LoanDuration,int GracePeriod, int TotalInstallmentNo)

public ActionResult Preview(string FundLoanCode, int LoanSanctionNo, int LoanSanctionTerm, string LoanDisbursementDate)
        {            
            try
            {
                int Qtype = 0;
                decimal vPrincipalAmount = 0;
                DateTime vLoanSanctionApproveDate = System.DateTime.Now;
                decimal vDisbursedAmount = 0;
                int vLoanDuration = 0;
                int vGracePeriod = 0;
                int vTotalInstallmentNo = 0;
                var paramValues = new List<ParameterValue>();
                paramValues.Add(new ParameterValue() { Name = "FundLoanCode", Value = FundLoanCode });
                paramValues.Add(new ParameterValue() { Name = "PrincipalAmount", Value = vPrincipalAmount.ToString() });
                paramValues.Add(new ParameterValue() { Name = "LoanSanctionNo", Value = LoanSanctionNo.ToString() });
                paramValues.Add(new ParameterValue() { Name = "LoanSanctionTerm", Value = LoanSanctionTerm.ToString() });
                paramValues.Add(new ParameterValue() { Name = "LoanSanctionApproveDate", Value = vLoanSanctionApproveDate.ToString() });
                paramValues.Add(new ParameterValue() { Name = "LoanDisbursementDate", Value = LoanDisbursementDate.ToString() });
                paramValues.Add(new ParameterValue() { Name = "DisbursedAmount", Value = vDisbursedAmount.ToString() });
                paramValues.Add(new ParameterValue() { Name = "InterestRate", Value = vDisbursedAmount.ToString() });
                paramValues.Add(new ParameterValue() { Name = "LoanDuration", Value = vLoanDuration.ToString() });
                paramValues.Add(new ParameterValue() { Name = "GracePeriod", Value = vGracePeriod.ToString() });
                paramValues.Add(new ParameterValue() { Name = "TotalInstallmentNo", Value = vTotalInstallmentNo.ToString() });
                paramValues.Add(new ParameterValue() { Name = "CompanyName", Value = SessionHelper.OrganizationName});
                paramValues.Add(new ParameterValue() { Name = "CompanyAddress", Value = SessionHelper.OrganizationAddress});
                paramValues.Add(new ParameterValue() { Name = "Qtype", Value = Qtype.ToString() });
                PrintSSRSReport("/gBanker_Reports/FundManagement", paramValues.ToArray(), "gBankerDbContext");
                return Content(string.Empty);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion
    }
}