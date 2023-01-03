using gBanker.Core.Utility;
using gBanker.Data.DBDetailModels;
using gBanker.Service;
using gBanker.Service.ReportServies;
using gBanker.Service.StoredProcedure;
using gBanker.Web.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace gBanker.Web.Controllers
{
    public class OverdueMemberListController : BaseController
    {
        #region Variables

        private readonly IDailyReportService dailyReportService;
        private readonly IGroupwiseReportService groupwiseReportService;
        private readonly IUltimateReportService unlimitedReportService;
        private readonly IProductService productService;
        private readonly IEmployeeSPService employeespService;

        public OverdueMemberListController(IDailyReportService dailyReportService, IProductService productService, IGroupwiseReportService groupwiseReportService,
            IEmployeeSPService employeespService,
            IUltimateReportService unlimitedReportService)
        {
            this.dailyReportService = dailyReportService;
            this.groupwiseReportService = groupwiseReportService;
            this.unlimitedReportService = unlimitedReportService;
            this.productService = productService;
            this.employeespService = employeespService;

        }

        #endregion


        public JsonResult GetProductList()
        {
            var getProduct = productService.GetAll().Where(s => s.IsActive == true && s.OrgID == LoggedInOrganizationID).OrderBy(e => e.ProductCode);
            var viewProduct = getProduct.Select(x => x).ToList().Select(x => new SelectListItem
            {
                Value = x.ProductID.ToString(),
                Text = x.ProductCode + ' ' + x.ProductName.ToString()
            });
            var prod_items = new List<SelectListItem>();
            if (viewProduct.ToList().Count > 0)
            {
                prod_items.Add(new SelectListItem() { Text = "Select All", Value = "0", Selected = true });
            }
            prod_items.AddRange(viewProduct);
            return Json(prod_items, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GenerateOverdueMemberListNewtReportMainProduct(string DateFrom, string DateTo)
        {
            try
            {
                var param = new { Office = SessionHelper.LoginUserOfficeID, DateFrom = DateFrom, DateTo = DateTo, Org = SessionHelper.LoginUserOrganizationID };
                var OverdueMls = dailyReportService.GetDataNewOverdueMemberListReport(param);
                //var alldata = groupwiseReportService.GetDataUltimateReleaseReportWithReportServer(param, "Proc_Get_DueLoan");

                var reportParam = new Dictionary<string, object>();
                reportParam.Add("param_orgName", ApplicationSettings.OrganiztionName);
                reportParam.Add("DateFrom", DateFrom);
                reportParam.Add("DateTo", DateTo);


                //ReportHelper.PrintReport("rptNewOverDueMemberList.rpt", OverdueMls.Tables[0], new Dictionary<string, object>());                    
                ReportHelper.PrintReport("rptNewOverDueMemberListMPW.rpt", OverdueMls.Tables[0], reportParam);

                return Content(string.Empty);
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }
        public ActionResult GenerateOverdueMemberListNewtReportExportMainProduct(string DateFrom, string DateTo)
        {
            var param = new { Office = SessionHelper.LoginUserOfficeID, DateFrom = DateFrom, DateTo = DateTo, Org = SessionHelper.LoginUserOrganizationID };
            groupwiseReportService.GetDataUltimateReleaseReport(param, "Proc_Get_DueLoan");

            GridView gv = new GridView();
            var allRepaymentSchedule = groupwiseReportService.GetDataUltimateReleaseReport(param, "Proc_Get_DueLoan");
            var detail = allRepaymentSchedule.Tables[0];
            gv.DataSource = detail;
            gv.DataBind();
            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=rptNewOverDueMemberListMainProduct.xls");
            Response.ContentType = "application/ms-excel";
            Response.Charset = "";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);
            gv.RenderControl(htw);
            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();

            return RedirectToAction("OverdueMemberListNewReportMainProduct");
        }
        public ActionResult GenerateOverdueMemberListNewtReport(string DateFrom, string DateTo)
        {
            try
            {
                var param = new { Office = SessionHelper.LoginUserOfficeID, DateFrom = DateFrom, DateTo = DateTo, Org = SessionHelper.LoginUserOrganizationID };
                var OverdueMls = dailyReportService.GetDataNewOverdueMemberListReport(param);
                var reportParam = new Dictionary<string, object>();
                reportParam.Add("param_orgName", ApplicationSettings.OrganiztionName);
                reportParam.Add("DateFrom", DateFrom);
                reportParam.Add("DateTo", DateTo);
                ReportHelper.PrintReport("rptNewOverDueMemberList.rpt", OverdueMls.Tables[0], reportParam);
                return Content(string.Empty);
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }
        public ActionResult GenerateOverdueNewMemberListNewtReport(string DateFrom, string DateTo)
        {
            try
            {
                var param = new { Office = SessionHelper.LoginUserOfficeID, DateFrom = DateFrom, DateTo = DateTo, Org = SessionHelper.LoginUserOrganizationID };
                var OverdueMls = dailyReportService.GetDataNewOverdueNewMemberListReport(param);
                var reportParam = new Dictionary<string, object>();
                reportParam.Add("param_orgName", ApplicationSettings.OrganiztionName);
                reportParam.Add("DateFrom", DateFrom);
                reportParam.Add("DateTo", DateTo);
                ReportHelper.PrintReport("rptNewOverDueMemberListThisMonth.rpt", OverdueMls.Tables[0], reportParam);
                return Content(string.Empty);
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }
        public ActionResult GenerateOverdueMemberListNewtReportExport(string DateFrom, string DateTo)
        {
            var param = new { Office = SessionHelper.LoginUserOfficeID, DateFrom = DateFrom, DateTo = DateTo, Org = SessionHelper.LoginUserOrganizationID };
            groupwiseReportService.GetDataUltimateReleaseReport(param, "Proc_Get_DueLoan");

            GridView gv = new GridView();
            var allRepaymentSchedule = groupwiseReportService.GetDataUltimateReleaseReport(param, "Proc_Get_DueLoan");
            var detail = allRepaymentSchedule.Tables[0];
            gv.DataSource = detail;
            gv.DataBind();
            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=rptNewOverDueMemberList.xls");
            Response.ContentType = "application/ms-excel";
            Response.Charset = "";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);
            gv.RenderControl(htw);
            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();

            return RedirectToAction("Index");
        }

        public ActionResult GenerateOverdueMemberListAllReportMainProduct(string DateTo, string productMainCode = "", string dueType = "")
        {
            try
            {
                //get data new overdue member list all report
                var overdueMls = GetDataNewOverdueMemberListAllReport(DateTo, productMainCode, dueType);

                //var alldata = groupwiseReportService.GetDataUltimateReleaseReportWithReportServer(param, "Rpt_OverDueLoaneeList_All");
                var reportParam = new Dictionary<string, object>();
                reportParam.Add("param_orgName", ApplicationSettings.OrganiztionName);
                //reportParam.Add("DateFrom", DateFrom);
                reportParam.Add("DateTo", DateTo);
                ReportHelper.PrintReport("rptOverDueLoaneeListAllMPW.rpt", overdueMls.Tables[0], reportParam);

                return Content(string.Empty);
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }
        public ActionResult GenerateOverdueMemberListAllReportExportMainProduct(string DateTo, string productMainCode = "", string dueType = "")
        {
            //get data new overdue member list all report for export
            var allRepaymentSchedule = GetDataNewOverdueMemberListAllReportForExport(DateTo, productMainCode, dueType);

            GridView gv = new GridView();

            var detail = allRepaymentSchedule.Tables[0];
            gv.DataSource = detail;
            gv.DataBind();
            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=rptOverDueLoaneeListAllMainProduct.xls");
            Response.ContentType = "application/ms-excel";
            Response.Charset = "";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);
            gv.RenderControl(htw);
            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();
            return RedirectToAction("OverdueMemberListAllReportMainProduct");

        }
        public ActionResult GenerateOverdueMemberListAllReport(string DateTo, string qType, string prod)
        {
            try
            {
                if (qType == "2")
                {
                    var param = new { Org = SessionHelper.LoginUserOrganizationID, Office = SessionHelper.LoginUserOfficeID, DateTo = DateTo };
                    var OverdueMls = dailyReportService.GetDataNewOverdueMemberListAllReport(param);

                    var reportParam = new Dictionary<string, object>();
                    reportParam.Add("param_orgName", ApplicationSettings.OrganiztionName);
                    //reportParam.Add("DateFrom", DateFrom);
                    reportParam.Add("DateTo", DateTo);


                    //ReportHelper.PrintReport("rptNewOverDueMemberList.rpt", OverdueMls.Tables[0], new Dictionary<string, object>());                    
                    ReportHelper.PrintReport("rptOverDueLoaneeListAll.rpt", OverdueMls.Tables[0], reportParam);
                }
                else if (qType == "1")
                {

                    var param = new { Org = SessionHelper.LoginUserOrganizationID, Office = SessionHelper.LoginUserOfficeID, DateTo = DateTo, prod = prod };
                    var OverdueMls = dailyReportService.GetDataNewOverdueMemberListAllReportProductWise(param);

                    var reportParam = new Dictionary<string, object>();
                    reportParam.Add("param_orgName", ApplicationSettings.OrganiztionName);
                    //reportParam.Add("DateFrom", DateFrom);
                    reportParam.Add("DateTo", DateTo);


                    //ReportHelper.PrintReport("rptNewOverDueMemberList.rpt", OverdueMls.Tables[0], new Dictionary<string, object>());                    
                    ReportHelper.PrintReport("rptOverDueLoaneeListProductWise.rpt", OverdueMls.Tables[0], reportParam);
                }
                return Content(string.Empty);
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }
        public ActionResult GenerateOverdueMemberListAllReportDisburseDateWise(string DateFrom, string DateTo)
        {
            try
            {

                var param = new { Org = SessionHelper.LoginUserOrganizationID, Office = SessionHelper.LoginUserOfficeID, DateFrom = DateFrom, DateTo = DateTo };
                var OverdueMls = dailyReportService.GetDataNewOverdueMemberListAllReportDisburseDateWise(param);

                var reportParam = new Dictionary<string, object>();
                reportParam.Add("param_orgName", ApplicationSettings.OrganiztionName);
                //reportParam.Add("DateFrom", DateFrom);
                reportParam.Add("DateTo", DateTo);


                //ReportHelper.PrintReport("rptNewOverDueMemberList.rpt", OverdueMls.Tables[0], new Dictionary<string, object>());                    
                ReportHelper.PrintReport("rptOverDueLoaneeListAll.rpt", OverdueMls.Tables[0], reportParam);


                return Content(string.Empty);
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }
        public ActionResult GenerateOverdueMemberListAllReportExport(string DateTo)
        {
            //try
            //{
            var param = new { Org = SessionHelper.LoginUserOrganizationID, Office = SessionHelper.LoginUserOfficeID, DateTo = DateTo };
            groupwiseReportService.GetDataUltimateReleaseReport(param, "Rpt_OverDueLoaneeList_All");

            GridView gv = new GridView();
            var allRepaymentSchedule = groupwiseReportService.GetDataUltimateReleaseReport(param, "Rpt_OverDueLoaneeList_All");
            var detail = allRepaymentSchedule.Tables[0];
            gv.DataSource = detail;
            gv.DataBind();
            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=rptOverDueLoaneeListAll.xls");
            Response.ContentType = "application/ms-excel";
            Response.Charset = "";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);
            gv.RenderControl(htw);
            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();

            return RedirectToAction("OverdueMemberListAllReport");
        }

        public ActionResult OverdueMemberListAllReport()
        {
            DateTime VDate;
            VDate = System.DateTime.Now;
            if (IsDayInitiated)
            {
                ViewData["Trxdate"] = TransactionDate.ToString("dd-MMM-yyyy");
            }
            else
            {
                ViewData["Trxdate"] = VDate.ToString("dd-MMM-yyyy");
            }
            IEnumerable<SelectListItem> items = new SelectList(" ");

            ViewData["ProductList"] = items;

            return View();
        }
        // GET: /OverdueMemberList/
        public ActionResult OverdueMemberListNewReportMainProduct()
        {
            DateTime VDate;
            VDate = System.DateTime.Now;
            if (IsDayInitiated)
            {
                ViewData["Trxdate"] = TransactionDate.ToString("dd-MMM-yyyy");
            }
            else
            {
                ViewData["Trxdate"] = VDate.ToString("dd-MMM-yyyy");
            }
            return View();
        }
        public ActionResult OverdueMemberListAllReportMainProduct()
        {
            DateTime VDate;
            VDate = System.DateTime.Now;
            if (IsDayInitiated)
            {
                ViewData["Trxdate"] = TransactionDate.ToString("dd-MMM-yyyy");
            }
            else
            {
                ViewData["Trxdate"] = VDate.ToString("dd-MMM-yyyy");
            }
            IEnumerable<SelectListItem> items = new SelectList(" ");
            ViewData["ProductList"] = items;
            ViewData["DueType"] = items;

            return View();
        }

        //public ActionResult OverdueMemberListAllReport()
        //{
        //    DateTime VDate;
        //    VDate = System.DateTime.Now;
        //    if (IsDayInitiated)
        //    {
        //        ViewData["Trxdate"] = TransactionDate.ToString("dd-MMM-yyyy");
        //    }
        //    else
        //    {
        //        ViewData["Trxdate"] = VDate.ToString("dd-MMM-yyyy");
        //    }
        //    IEnumerable<SelectListItem> items = new SelectList(" ");

        //    ViewData["ProductList"] = items;

        //    return View();
        //}

        public ActionResult OverdueMemberListAllReport_DisburseDateWise()
        {
            DateTime VDate;
            VDate = System.DateTime.Now;
            if (IsDayInitiated)
            {
                ViewData["Trxdate"] = TransactionDate.ToString("dd-MMM-yyyy");
            }
            else
            {
                ViewData["Trxdate"] = VDate.ToString("dd-MMM-yyyy");
            }
            IEnumerable<SelectListItem> items = new SelectList(" ");

            ViewData["ProductList"] = items;

            return View();
        }
        public ActionResult Index()
        {
            DateTime VDate;
            VDate = System.DateTime.Now;
            if (IsDayInitiated)
            {
                ViewData["Trxdate"] = TransactionDate.ToString("dd-MMM-yyyy");
            }
            else
            {
                ViewData["Trxdate"] = VDate.ToString("dd-MMM-yyyy");
            }
            return View();
        }

        //
        // GET: /OverdueMemberList/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /OverdueMemberList/Create
        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /OverdueMemberList/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /OverdueMemberList/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /OverdueMemberList/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /OverdueMemberList/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /OverdueMemberList/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }


        #region Ajax Calls

        public JsonResult GetProductListForOverdue()
        {

            var getProduct = productService.GetMany(s => s.IsActive == true && s.OrgID == LoggedInOrganizationID && s.MainProductCode !=null && s.MainProductCode !="" ).GroupBy(g=>g.MainProductCode).Select(f=>f.FirstOrDefault()).OrderBy(e => e.ProductCode);
            var viewProduct = getProduct.Select(x => x).ToList().Select(x => new SelectListItem
            {
                Value = x.MainProductCode.ToString(),
                Text = $@"{x.MainProductCode.ToString()} - {x.ProductName.ToString()}"
            });
            var prod_items = new List<SelectListItem>();
            if (viewProduct.ToList().Count > 0)
            {
                prod_items.Add(new SelectListItem() { Text = "Select All", Value = "", Selected = true });
            }
            prod_items.AddRange(viewProduct);
            return Json(prod_items, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Private Methods

        private DataSet GetDataNewOverdueMemberListAllReport(string dateTo,string productMainCode="", string dueType="")
        {
            if (SessionHelper.LoginUserOrganizationID == MFIConstants.Society_For_Social_Service_SSS)
            {
                var param = new { Org = SessionHelper.LoginUserOrganizationID, Office = SessionHelper.LoginUserOfficeID, DateTo = dateTo, productMainCode = productMainCode, dueType=dueType };
                var OverdueMls = dailyReportService.GetDataNewOverdueMemberListAllReportByFilter(param);

                return OverdueMls;
            }
            else
            {
                var param = new { Org = SessionHelper.LoginUserOrganizationID, Office = SessionHelper.LoginUserOfficeID, DateTo = dateTo };
                var OverdueMls = dailyReportService.GetDataNewOverdueMemberListAllReport(param);
                return OverdueMls;
            }
        }

        private DataSet GetDataNewOverdueMemberListAllReportForExport(string dateTo, string productMainCode = "", string dueType = "")
        {
            if (SessionHelper.LoginUserOrganizationID == MFIConstants.Society_For_Social_Service_SSS)
            {                
                var param = new { Org = SessionHelper.LoginUserOrganizationID, Office = SessionHelper.LoginUserOfficeID, DateTo = dateTo, productMainCode = productMainCode, dueType = dueType };                
                var allRepaymentSchedule = groupwiseReportService.ExportExcellData(param, "Rpt_OverDueLoaneeList_All_By_Filter");

                return allRepaymentSchedule;
            }
            else
            {
                var param = new { Org = SessionHelper.LoginUserOrganizationID, Office = SessionHelper.LoginUserOfficeID, DateTo = dateTo };
                var allRepaymentSchedule = groupwiseReportService.ExportExcellData(param, "Rpt_OverDueLoaneeList_All");
                return allRepaymentSchedule;
            }
        }

        #endregion
    }
}
