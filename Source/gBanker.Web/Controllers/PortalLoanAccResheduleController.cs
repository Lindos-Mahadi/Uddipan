﻿using AutoMapper;
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
    public class PortalLoanAccResheduleController : Controller
    {
        private readonly ILoanAccRescheduleService loanAccRescheduleService;

        public PortalLoanAccResheduleController(ILoanAccRescheduleService loanAccRescheduleService)
        {
            this.loanAccRescheduleService = loanAccRescheduleService;
        }

        // GET: PortalLoanAccReshedule
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult PortalLoanAccResheduleInfo(int jtStartIndex, int jtPageSize, string jtSorting)
        {
            try
            {
                //List<LoanAccReschedule> loAccReshedule = new List<LoanAccReschedule>();
                var loanAccReshedule = loanAccRescheduleService.GetAll();

                if (loanAccReshedule != null)
                {
                    var mapLoanAccReshedule = Mapper.Map<IEnumerable<LoanAccReschedule>, List<LoanAccRescheduleViewModel>>(loanAccReshedule);
                    
                }
                var loanAccResheduleDetail = loanAccReshedule.Skip(jtStartIndex).Take(jtPageSize).ToList();
                var currentPageRecords = loanAccResheduleDetail.ToList();
                return Json(new { Result = "Ok", Records = currentPageRecords, TotalRecordCount = loanAccReshedule.Count()});

            }
            catch (Exception ex)
            {

                return Json(new { Result = "Error", Message = ex.Message});
            }
            
        }
    }
}