using gBanker.Service.ReportServies;
using gBanker.Web.Helpers;
using gBanker.Web.Reports;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace gBanker.Web.Controllers
{
    public class POMIS5AController : BaseController
    {
        #region Variables
        private readonly IUltimateReportService ultimateReportService;
        private readonly IPOMISReportService pomisReportService;
        private readonly IGroupwiseReportService groupwiseReportService;
        public POMIS5AController(IUltimateReportService ultimateReportService, IPOMISReportService pomisReportService, IGroupwiseReportService groupwiseReportService)
        {
            this.ultimateReportService = ultimateReportService;
            this.pomisReportService = pomisReportService;
            this.groupwiseReportService = groupwiseReportService;
        }
        #endregion

        #region Methods
        public ActionResult GeneratePomisReport(string from_date, string qType)
        {
            try
            {

                var param = new { Org = LoggedInOrganizationID, OfficeFrom = LoginUserOfficeID, OfficeTo = LoginUserOfficeID, Date = from_date, qType = qType };
                var mracdb = ultimateReportService.GetDataPOMISFiveAReport(param);

                var reportParam = new Dictionary<string, object>();
                reportParam.Add("param_orgName", ApplicationSettings.OrganiztionName);
                
                if(qType == "1")
                    ReportHelper.PrintReport("SubRptPOMIS5aNote1.rpt", mracdb.Tables[0], reportParam);
                else if (qType == "2")
                    ReportHelper.PrintReport("SubRptPOMIS5aNote2.rpt", mracdb.Tables[0], reportParam);
                else if (qType == "3")
                    ReportHelper.PrintReport("SubRptPOMIS5aNote3.rpt", mracdb.Tables[0], reportParam);
                else if (qType == "4")
                    ReportHelper.PrintReport("SubRptPOMIS5aNote4.rpt", mracdb.Tables[0], reportParam);
                else if (qType == "5")
                    ReportHelper.PrintReport("SubRptPOMIS5aNote5.rpt", mracdb.Tables[0], reportParam);
                    return Content(string.Empty);
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        public ActionResult GeneratePomisReportConsolidationNote(string from_date, string qType)
        {
            try
            {

                var param = new { Org = LoggedInOrganizationID, OfficeFrom = 00, OfficeTo = 9999999, Date = from_date, qType = qType };
                var mracdb = groupwiseReportService.GetDataUltimateReleaseReport(param, "Rpt_POMIS5A_ConsolidationNew");
                var reportParam = new Dictionary<string, object>();
                reportParam.Add("param_orgName", ApplicationSettings.OrganiztionName);
                if (qType == "1")
                    ReportHelper.PrintReport("SubRptPOMIS5aNote1Conso.rpt", mracdb.Tables[0], reportParam);
                else if (qType == "2")
                    ReportHelper.PrintReport("SubRptPOMIS5aNote2Conso.rpt", mracdb.Tables[0], reportParam);
                else if (qType == "3")
                    ReportHelper.PrintReport("SubRptPOMIS5aNote3Conso.rpt", mracdb.Tables[0], reportParam);
                else if (qType == "4")
                    ReportHelper.PrintReport("SubRptPOMIS5aNote4Conso.rpt", mracdb.Tables[0], reportParam);
                else if (qType == "5")
                    ReportHelper.PrintReport("SubRptPOMIS5aNote5Conso.rpt", mracdb.Tables[0], reportParam);
                return Content(string.Empty);

            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }
        public ActionResult GeneratePOMISReport5AFinal(string Date)
        {
            try
            {
                var param = new { Org = LoggedInOrganizationID, OfficeFrom = SessionHelper.LoginUserOfficeID, OfficeTo = SessionHelper.LoginUserOfficeID, Date = Date};
                var POMIS5Finals = pomisReportService.GetDataPOMIS5AReport(param);
                var POMIS5Bs = pomisReportService.GetDataPOMIS5BReport(param);
                var subReportDB = new Dictionary<string, DataTable>();
                subReportDB.Add("SubRptPOMIS5aNote2New.rpt", POMIS5Bs.Tables[0]);
                ReportHelper.PrintWithSubReport("SubRptPOMIS5aNoteFinal.rpt", POMIS5Finals.Tables[0], new Dictionary<string, object>(), subReportDB, new SubRptPOMIS5aNoteFinal());
                return Content(string.Empty);
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        #endregion

        #region Events
        public ActionResult pomis()
        {
            DateTime VDate;
            VDate = System.DateTime.Now;
            ViewData["Trxdate"] = VDate.ToString("dd-MMM-yyyy");
            return View();
        }
        public ActionResult pomisNoteConsolidation()
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
            //ViewData["Trxdate"] = TransactionDate.ToString("dd-MMM-yyyy");
            return View();
        }
        public ActionResult POMISReport5AFinal()
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
            //ViewData["Trxdate"] = TransactionDate.ToString("dd-MMM-yyyy");
            return View();
        }


        public ActionResult Details(int id)
        {
            return View();
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        public ActionResult Edit(int id)
        {
            return View();
        }

        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        public ActionResult Delete(int id)
        {
            return View();
        }

        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
        #endregion
    }
}
