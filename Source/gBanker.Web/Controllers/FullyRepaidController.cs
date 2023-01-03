using gBanker.Service;
using gBanker.Service.ReportServies;
using gBanker.Web.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace gBanker.Web.Controllers
{
    public class FullyRepaidController : BaseController
    {
             
        #region Variables
        private readonly IDailyReportService dailyReportService;
        private readonly IProductService productService;


        public FullyRepaidController(IDailyReportService dailyReportService, IProductService productService)
        {
            this.dailyReportService = dailyReportService;
            this.productService = productService;
    
        }
        #endregion

        #region Methods
        public JsonResult GetProductList()
        {
            var getProduct = productService.GetAll().Where(s => s.IsActive == true && s.OrgID == LoggedInOrganizationID).OrderBy(e => e.ProductCode);
            var viewProduct = getProduct.Select(x => x).ToList().Select(x => new SelectListItem
            {
                Value = x.ProductID.ToString(),
                Text = x.ProductCode.ToString() + ", " + x.ProductName.ToString()
            });
            var prod_items = new List<SelectListItem>();
            if (viewProduct.ToList().Count > 0)
            {
                prod_items.Add(new SelectListItem() { Text = "Select All", Value = "0", Selected = true });
            }
            prod_items.AddRange(viewProduct);
            return Json(prod_items, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GenerateFullyrepaidReport()
        {
            try
            {

                var param = new { Office = SessionHelper.LoginUserOfficeID, Org = SessionHelper.LoginUserOrganizationID, EmpID =LoggedInEmployeeID};
                var FullyReapids = dailyReportService.GetDataFullyRepaidReport(param);

                var reportParam = new Dictionary<string, object>();
                reportParam.Add("param_orgName", ApplicationSettings.OrganiztionName);

                //ReportHelper.PrintReport("rptFullyRep.rpt", FullyReapids.Tables[0], new Dictionary<string, object>());                    
                ReportHelper.PrintReport("rptFullyRep.rpt", FullyReapids.Tables[0], reportParam);

                return Content(string.Empty);
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }
        public ActionResult GenerateFullyrepaid_DateRangeReport(string Qtype, string Date1, string Date2, string Product)
        {
            try
            {

                var param = new { Qtype = Qtype, Office = SessionHelper.LoginUserOfficeID, Date1 = Date1, Date2 = Date2, Org = SessionHelper.LoginUserOrganizationID, Product = Product, EmpID = LoggedInEmployeeID };
                var FullyReapids = dailyReportService.GetDataFullyRepaid_DateRangeReport(param);

                var reportParam = new Dictionary<string, object>();
                reportParam.Add("param_orgName", ApplicationSettings.OrganiztionName);
                reportParam.Add("Date1", Date1);
                reportParam.Add("Date2", Date2);
                //reportParam.Add("Product", Product);

                //ReportHelper.PrintReport("rptFullyRep.rpt", FullyReapids.Tables[0], new Dictionary<string, object>());                    
                ReportHelper.PrintReport("rptFullyRep_DateRang.rpt", FullyReapids.Tables[0], reportParam);

                return Content(string.Empty);
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }
        #endregion

        #region Events
        //
        // GET: /FullyRepaid/
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult FRDateRange()
        {
            IEnumerable<SelectListItem> items = new SelectList(" ");
            ViewData["ProductList"] = items;
            
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
        // GET: /FullyRepaid/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /FullyRepaid/Create
        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /FullyRepaid/Create
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
        // GET: /FullyRepaid/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /FullyRepaid/Edit/5
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
        // GET: /FullyRepaid/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /FullyRepaid/Delete/5
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
        #endregion