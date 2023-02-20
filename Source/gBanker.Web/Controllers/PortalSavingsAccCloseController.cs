using AutoMapper;
using gBanker.Data.CodeFirstMigration;
using gBanker.Service;
using gBanker.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace gBanker.Web.Controllers
{
    public class PortalSavingsAccCloseController : Controller
    {
        private readonly ISavingsAccCloseService savingsAccCloseService;

        public PortalSavingsAccCloseController(ISavingsAccCloseService savingsAccCloseService)
        {
            this.savingsAccCloseService = savingsAccCloseService;
        }

        // GET: PortalSavingsAccClose
        public ActionResult Index()
        {
            return View();
        }
        public JsonResult PortalSavingsAccCloseInfo(int jtStartIndex, int jtPageSize, string jtSorting)
        {
            try
            {
                var savingsAccClose = savingsAccCloseService.GetAll();
                if (savingsAccClose != null)
                {
                    var savingsAccCloseMap = Mapper.Map<IEnumerable<SavingsAccClose>, List<SavingsAccCloseViewModel>>(savingsAccClose);
                }

                var savingsAccCloseDetail = savingsAccClose.Skip(jtStartIndex).Take(jtPageSize).ToList();
                var currentPageRecords = savingsAccCloseDetail.ToList();
                return Json(new { Result = "OK", Records = currentPageRecords, TotalCountRecord = savingsAccClose.Count() });
            }
            catch (Exception ex)
            {

                return Json(new { Result = "Error", Message = ex.Message });
            }
        }
        //[HttpPost]
        //public JsonResult PortalSavingsAccCloseInfo(int jtStartIndex, int jtPageSize, string jtSorting)
        //{
        //    try
        //    {
        //        //List<LoanAccReschedule> loAccReshedule = new List<LoanAccReschedule>();
        //        var loanAccReshedule = savingsAccCloseService.GetAll();

        //        if (loanAccReshedule != null)
        //        {
        //            var mapLoanAccReshedule = Mapper.Map<IEnumerable<SavingsAccClose>, List<SavingsAccCloseViewModel>>(loanAccReshedule);

        //        }
        //        var loanAccResheduleDetail = loanAccReshedule.Skip(jtStartIndex).Take(jtPageSize).ToList();
        //        var currentPageRecords = loanAccResheduleDetail.ToList();
        //        return Json(new { Result = "OK", Records = currentPageRecords, TotalRecordCount = loanAccReshedule.Count() });

        //    }
        //    catch (Exception ex)
        //    {

        //        return Json(new { Result = "Error", Message = ex.Message });
        //    }
        //}
    }
}