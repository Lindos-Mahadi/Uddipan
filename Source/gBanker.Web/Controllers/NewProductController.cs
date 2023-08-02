using AutoMapper;
using gBanker.Service;
using gBanker.Service.ReportServies;
using gBanker.Web.Helpers;
using gBanker.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using System.Web.UI;
using gBanker.Data.CodeFirstMigration.Db;
using System.Security.Cryptography.Xml;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using gBanker.Data.CodeFirstMigration;
using System.Web.Http.Results;
using gBanker.Data.DBDetailModels;
using System.ComponentModel.DataAnnotations;
using Elmah;

namespace gBanker.Web.Controllers
{
    public class NewProductController : BaseController
    {
        private readonly IProductService productService;
        private readonly IInvestorService investorService;
        private readonly IMemberCategoryService memberCategoryService;
        private readonly IProductReportService productReportService;
        private readonly IDurationService durationService;
        private readonly IProductIdentificationService productIdentificationService;
        public NewProductController(IProductService productService, IInvestorService investorService,
            IMemberCategoryService memberCategoryService, IProductReportService productReportService,
            IDurationService durationService, IProductIdentificationService productIdentificationService)
        {
            this.productService = productService;
            this.investorService = investorService;
            this.memberCategoryService = memberCategoryService;
            this.productReportService = productReportService;
            this.durationService = durationService;
            this.productIdentificationService = productIdentificationService;
        }
        public ActionResult ExportData()
        {
            GridView gv = new GridView();
            var allRepaymentSchedule = productService.GetAll().Where(p => p.OrgID == LoggedInOrganizationID);
            var detail = allRepaymentSchedule.ToList();
            gv.DataSource = detail;
            gv.DataBind();
            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=Product.xls");
            Response.ContentType = "application/ms-excel";
            Response.Charset = "";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);
            gv.RenderControl(htw);
            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();

            return RedirectToAction("ProductDetails");
        }
        public ActionResult Index()

        {
            return View();
        }
        [HttpPost]
        public ActionResult GetProducts(int jtStartIndex, int jtPageSize, string jtSorting, string filterColumn, string filterValue)
        {
            try
            {

                long totalCount;
                var allproduct = productService.GetProductDetailPaged(filterColumn, filterValue, jtStartIndex, jtSorting, jtPageSize, out totalCount, LoggedInOrganizationID);
                var currentPageRecords = Mapper.Map<IEnumerable<Product>, IEnumerable<ProductViewModel>>(allproduct);

                return Json(new { Result = "OK", Records = currentPageRecords, TotalRecordCount = totalCount });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }


        }
        public ActionResult GetProductMainCodeList(string productCode)
        {
            try
            {
                /*var allproduct = productService.GetProductMainCodeList().ToList();*/

                List<Product> allproductList = productService.GetAll().ToList();


                List<Product> getMainProduct = allproductList.Where(p => p.MainProductCode == productCode).ToList();

                float maxProdCode = getMainProduct.Max(x => float.Parse(x.ProductCode));
                float maxProdCodeInFloat = maxProdCode + 0.01f;
                string maxProdCodeStr = maxProdCodeInFloat.ToString("00.00");



                //var result = new { Result = "OK", Records = getMainProduct, TotalRecordCount = getMainProduct.Count() };
                var json = new JsonResult() { Data = maxProdCodeStr, ContentType = "application/json", JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                return json;
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }


        }
        public ActionResult GenerateReport(string fromDate, string toDate)
        {
            var param = new { Status = 1 };
            var allproducts = productReportService.GetDataProductInfo(param);

            var reportParam = new Dictionary<string, object>();
            reportParam.Add("Header1", ApplicationSettings.OrganiztionName);
            ReportHelper.PrintReport("rptProductInfo.rpt", allproducts.Tables[0], reportParam);
            return Content(string.Empty); ;

            //ReportHelper.PrintReport("rptProductInfo.rpt", allproducts.Tables[0], new Dictionary<string, object>());
            //return Content(string.Empty); ;
        }
        // GET: Product/Create
        public ActionResult Create()
        {

            // ViewBag.investorList = viewInvestor;

            var model = new ProductViewModel();
            MapDropDownListCreate(model);

            return View(model);
            //////////////////===================================================/////////////////////////////////////////////////
            //Below line of code will set the default value in the view....
            //var defaultCategory = new InvestorViewModel() { in = "", ParentMemberCategoryID = 1, CategoryList = viewCategory.Select(s => new SelectListItem() { Text = s.CategoryName, Value = s.MemberCategoryID.ToString() }) };
            //return View(defaultCategory);


        }
        private void MapDropDownListCreate(ProductViewModel model)
        {
            var frequency = new List<SelectListItem>();
            frequency.Add(new SelectListItem() { Text = "Select One", Value = "", Selected = true });
            frequency.Add(new SelectListItem() { Text = "Weekly", Value = "W" });
            frequency.Add(new SelectListItem() { Text = "Monthly", Value = "M" });

            //var insuranceList = new List<SelectListItem>();
            //insuranceList.Add(new SelectListItem() { Text = "Yes", Value = "Y", Selected = true });
            //insuranceList.Add(new SelectListItem() { Text = "No", Value = "No" });

            var calcList = new List<SelectListItem>();
            calcList.Add(new SelectListItem() { Text = "Flat", Value = "F" });
            calcList.Add(new SelectListItem() { Text = "Declined", Value = "D", Selected = true });
            calcList.Add(new SelectListItem() { Text = "Amortaization", Value = "A" });
            calcList.Add(new SelectListItem() { Text = "Amortaization-Fixed", Value = "E" });
            calcList.Add(new SelectListItem() { Text = "Abasan", Value = "H" });
            calcList.Add(new SelectListItem() { Text = "Variance", Value = "V" });
            var prodType = new List<SelectListItem>();
            prodType.Add(new SelectListItem() { Text = "Savings", Value = "0" });
            prodType.Add(new SelectListItem() { Text = "Loan", Value = "1", Selected = true });


            var mProductList = productService.GetProductMainCodeList().AsEnumerable().Select(t => new SelectListItem
            {
                Text = t.MainProductCode + " - " + t.MainItemName,
                Value = t.MainProductCode
            }).ToList();
            mProductList.Insert(0, new SelectListItem { Text = "Select Main Product", Value = "", Selected = true });


            var mainItemInsurance = productService.GetProductMainCodeList().AsEnumerable().Select(t => new SelectListItem
            {
                Text = t.MainItemName,
                Value = t.MainItemName
            }).ToList();
            mainItemInsurance.Insert(0, new SelectListItem { Text = "Select Item Name", Value = "", Selected = true });
            model.MainProductInsuranceList = mainItemInsurance;

            var productIdentificationList = productIdentificationService.getProductIdentificationItemList().AsEnumerable().Select(p => new SelectListItem
            {
                Text = p.IdentificationName,
                Value = p.ID.ToString()
            }).ToList();
            productIdentificationList.Insert(0, new SelectListItem { Text = "Select Product Identification", Value = "", Selected = true });
            model.ProductIdentificationItemList = productIdentificationList;

            model.MainProductList = mainItemInsurance;

            var durationList = durationService.getDurationItemList().AsEnumerable().Select(t => new SelectListItem
            {
                Text = t.ProductPaymentFrequency + " - " + t.DurationName.ToString(),
                Value = t.ID.ToString(),
            }).ToList();
            durationList.Insert(0, new SelectListItem { Text = "Select Duration", Value = "", Selected = true });

            model.DurationItemList = durationList;
            // var viewInvestor = allinvestor.Select(m => new SelectListItem() { Text = string.Format("{0} - {1}", m.InvestorCode, m.InvestorName), Value = m.InvestorID.ToString() });

            model.PInvestorListItems = prodType;
            model.PCalcuationMethodListItems = calcList.AsEnumerable();
            model.PFrequencyListItems = frequency.AsEnumerable();

            model.MainProductList = mProductList;
            //model.InsuranceItemList = insuranceList.AsEnumerable();
            model.MemberCategoryList = memberCategoryService.GetAll().Where(m => m.OrgID == LoggedInOrganizationID).Select(s => new SelectionViewModel() { Code = s.MemberCategoryCode, Id = s.MemberCategoryID, DisplayName = string.Format("{0} - {1}", s.MemberCategoryCode, s.CategoryName), IsSelected = false }).ToList();
        }

        // POST: Product/Create
        [HttpPost]
        public ActionResult Create(ProductViewModel model)
        {
            try
            {

                if (string.IsNullOrWhiteSpace(model.MainProductCode))
                {
                    string mainProductCode = model.ProductCode.Split('.')[0];
                    model.MainProductCode = mainProductCode + ".00";
                    model.MainItemName = model.ProductFullNameEng;
                    model.ProductCode = mainProductCode;
                }


                // model.CreateDate = System.DateTime.Now;
                model.IsActive = true;
                //var selectedMemberCategory = model.MemberCategoryList.Where(w => w.IsSelected).ToList();

                var entity = Mapper.Map<ProductViewModel, Product>(model);
                //Add Validlation Logic.
                if (ModelState.IsValid)
                {
                    var errors = productService.IsValidProduct(entity);
                    if (errors.ToList().Count == 0)
                    {
                        //if (entity.GracePeriod > 31)
                        //{
                        //    return GetErrorMessageResult("Grace Period exceeds");
                        //}
                        entity.OrgID = Convert.ToInt16(LoggedInOrganizationID);
                        productService.Create(entity);
                        return GetSuccessMessageResult();
                    }
                    else
                    {
                        ModelState.AddModelError("MainProductCode", "Duplicate Product Code");
                        return GetErrorMessageResult(errors);
                    }
                }
                //return GetErrorMesreturn View(model);sageResult();
                return View(model);

            }
            catch (Exception ex)
            {
                return GetErrorMessageResult(ex);
            }
        }
    }
}