using gBanker.Service;
using gBanker.Service.ReportServies;
using gBanker.Web.Helpers;
using gBanker.Web.ViewModels;
using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace gBanker.Web.Controllers
{
    public class ProductMappingController : BaseController
    {
        #region Variables
        private readonly IOfficeService officeService;
        private readonly IUltimateReportService ultimateReportService;
        private readonly IProductService productService;


        public ProductMappingController(IOfficeService officeService,  IProductService productService,  IUltimateReportService ultimateReportService)
        {
            this.officeService = officeService;
            this.productService = productService;
            this.ultimateReportService = ultimateReportService;
        }
#endregion

        // GET: ProductMapping
        public ActionResult Set()
        {
            ProdAccMappingViewModel model = new ProdAccMappingViewModel();

            var offc_id = Convert.ToInt32(SessionHelper.LoginUserOfficeID);
            //var allOffice = officeService.GetAll().Where(m => m.OfficeLevel == 4 && m.OfficeID == offc_id && m.OrgID == LoggedInOrganizationID).ToList();
            var allOffice = officeService.GetAll();
            // var allOffice = officeService.GetById(offc_id);
            var viewOffice = allOffice.Select(x => x).ToList().Select(x => new SelectListItem
            //var viewOffice = allOffice.Select(x => x).ToList().Select(x => new SelectListItem
            {
                Value = x.OfficeID.ToString(),
                Text = string.Format("{0}, {1}", x.OfficeCode.ToString(), x.OfficeName.ToString())
            });
            var ofc_items = new List<SelectListItem>();
            ofc_items.Add(new SelectListItem() { Text = "Please Select", Value = "0", Selected = true });
            ofc_items.AddRange(viewOffice);
            model.OfficeList = ofc_items;


            return View(model);
        }



        #region Methods
        public JsonResult GetAvailableProductList(string OfficeID)
        {
            try
            {
                List<ProductViewModel> List_ProductInfoViewModel = new List<ProductViewModel>();
                if (Convert.ToInt32(OfficeID) > 0 && OfficeID != "")
                {
                    var param = new { OfficeID = Convert.ToInt32(OfficeID) };
                    var officeList = ultimateReportService.GetProductListForMapping(param);

                    List_ProductInfoViewModel = officeList.Tables[0].AsEnumerable()
                    .Select(row => new ProductViewModel
                    {
                        Sl = row.Field<int>("Sl"),
                        MyProductID = row.Field<Int16>("ProductId"),
                        ProductCode = row.Field<string>("ProductCode"),
                        ProductName = row.Field<string>("ProductFullNameEng")
                    }).ToList();
                }
                return Json(List_ProductInfoViewModel.ToList(), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetSelectedProductList(string OfficeID)
        {
            try
            {
                List<ProductViewModel> List_ProductInfoViewModel = new List<ProductViewModel>();
                if (Convert.ToInt32(OfficeID) > 0 && OfficeID != "")
                {
                    var param = new { OfficeID = Convert.ToInt32(OfficeID) };
                    var officeList = ultimateReportService.GetSelectedProductListForMapping(param);

                    List_ProductInfoViewModel = officeList.Tables[0].AsEnumerable()
                    .Select(row => new ProductViewModel
                    {
                        Sl = row.Field<int>("Sl"),
                        MyProductID = row.Field<Int16>("ProductId"),
                        ProductCode = row.Field<string>("ProductCode"),
                        ProductName = row.Field<string>("ProductFullNameEng")
                    }).ToList();
                }
                return Json(List_ProductInfoViewModel.ToList(), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
         
        public JsonResult OfficeWiseProductSave(List<string> allProductIds, string OfficeID)
        {
            int dealOfficeId = 0;
            if (OfficeID != "")
            {
                
                var officeID = Convert.ToInt32(OfficeID);
                dealOfficeId = officeID;
                foreach (var ProdId in allProductIds)
                {
                    var param = new { OfficeID = Convert.ToInt32(OfficeID), ProductId = ProdId };
                    var officeList = ultimateReportService.SaveProductMapping(param);
                }
                return Json(dealOfficeId, JsonRequestBehavior.AllowGet);
            }
            return Json("Error", JsonRequestBehavior.AllowGet);
        }


        
        public JsonResult OfficeWiseProductDelete(List<string> ProductIds, string OfficeID)
        {
            int dealOfficeId = 0;
            if (OfficeID != "")
            {

                var officeID = Convert.ToInt32(OfficeID);
                dealOfficeId = officeID;
                foreach (var ProdId in ProductIds)
                {
                    var param = new { OfficeID = Convert.ToInt32(OfficeID), ProductId = ProdId };
                    var officeList = ultimateReportService.DeleteProductMapping(param);
        
                }
                return Json(dealOfficeId, JsonRequestBehavior.AllowGet);
            }
            return Json("Error", JsonRequestBehavior.AllowGet);
        }



        #endregion Methods



    }// End Class
}// End Namespace