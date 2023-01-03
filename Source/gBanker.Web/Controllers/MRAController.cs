using gBanker.Service.ReportServies;
using gBanker.Web.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
namespace gBanker.Web.Controllers
{
    public class MRAController : BaseController
    {
        #region Variables
        private readonly IMRAReportService mraReportService;
        public MRAController(IMRAReportService mraReportService)
        {
            this.mraReportService = mraReportService;

        }
        #endregion
        public ActionResult GenerateMraReport(string Date, string Qtype)
        {
            try
            {

                var param = new { Office = SessionHelper.LoginUserOfficeID, Date = Date, Qtype = Qtype, Org = SessionHelper.LoginUserOrganizationID };
                var Mras = mraReportService.GetDataMraReport(param);

                var reportParam = new Dictionary<string, object>();
                reportParam.Add("param_orgName", ApplicationSettings.OrganiztionName);
                reportParam.Add("UptoDate", Date);
                // ReportHelper.PrintReport("rptTodaysSummary.rpt", TodaysSummarys.Tables[0], reportParam);

                if (Qtype == "1")
                {
                    //ReportHelper.PrintReport("MRA_01.rpt", Mras.Tables[0], new Dictionary<string, object>());                    
                    ReportHelper.PrintReport("MRA_01.rpt", Mras.Tables[0], reportParam);
                }
                else if (Qtype == "2")
                {

                    //ReportHelper.PrintReport("MRA_02.rpt", Mras.Tables[0], new Dictionary<string, object>());                    
                    ReportHelper.PrintReport("MRA_02.rpt", Mras.Tables[0], reportParam);
                }
                else if (Qtype == "3")
                {

                    //ReportHelper.PrintReport("MRA_03.rpt", Mras.Tables[0], new Dictionary<string, object>());                    
                    ReportHelper.PrintReport("MRA_03.rpt", Mras.Tables[0], reportParam);
                }
                else if (Qtype == "4")
                {
                    //ReportHelper.PrintReport("MRA_04.rpt", Mras.Tables[0], new Dictionary<string, object>());
                    ReportHelper.PrintReport("MRA_04.rpt", Mras.Tables[0], reportParam);
                }
                else if (Qtype == "5")
                {

                    //ReportHelper.PrintReport("MRA_05.rpt", Mras.Tables[0], new Dictionary<string, object>());
                    ReportHelper.PrintReport("MRA_05.rpt", Mras.Tables[0], reportParam);
                }
                else if (Qtype == "6")
                {

                    //ReportHelper.PrintReport("rpt_MRA_ProvisionCalculation.rpt", Mras.Tables[0], new Dictionary<string, object>());
                    ReportHelper.PrintReport("rpt_MRA_ProvisionCalculation.rpt", Mras.Tables[0], reportParam);
                }

                return Content(string.Empty);


            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }

        }
        //
        // GET: /MRA/
        public ActionResult MRA()
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

        //
        // GET: /MRA/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /MRA/Create
        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /MRA/Create
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
        // GET: /MRA/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /MRA/Edit/5
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
        // GET: /MRA/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /MRA/Delete/5
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
    }
}
