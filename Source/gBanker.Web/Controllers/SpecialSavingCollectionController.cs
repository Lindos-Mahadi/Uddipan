﻿using AutoMapper;
using gBanker.Data.CodeFirstMigration;
using gBanker.Data.CodeFirstMigration.Db;
using gBanker.Service;
using gBanker.Web.Helpers;
using gBanker.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Data;
//using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using gBanker.Service.ReportServies;
using Antlr.Runtime;

namespace gBanker.Web.Controllers
{

    public class SpecialSavingCollectionController : BaseController
    {
        private readonly ICenterService centerService;
        private readonly ISpecialSavingCollectionService specialSavingCollectionService;
        private readonly IBranchService branchService;
        private readonly IOfficeService officeService;
        private readonly IProductService productService;
        private readonly IMemberCategoryService membercategoryService;
        private readonly IPurposeService purposeService;
        private readonly IMemberService memberService;
        private readonly ISavingSummaryService savingSummaryService;
        private readonly IAccTrxMasterService accMasterService;
        private readonly IAccTrxDetailService accDetailService;
        private readonly ISavingCollectionService savingCollectionService;
        private readonly IUltimateReportService ultimateReportService;
        private readonly IAccChartService accChartService;
        private readonly IApplicationSettingsService applicationSettingsService;
        private readonly IDailySavingTrxService dailySavingTrxService;
        private readonly IGroupwiseReportService groupwiseReportService;
        private readonly ISavingsAccCloseService savingsAccCloseService;
        private readonly IPortalSavingSummaryService portalSavingSummaryService;
        public SpecialSavingCollectionController(ISpecialSavingCollectionService specialSavingCollectionService, 
            IUltimateReportService ultimateReportService, ISavingSummaryService savingSummaryService, ICenterService centerService, 
            IProductService productService, IMemberCategoryService membercategoryService, IOfficeService officeService, 
            IPurposeService purposeService, IMemberService memberService, IAccTrxMasterService accMasterService, 
            IAccTrxDetailService accDetailService, ISavingCollectionService savingCollectionService, 
            IAccChartService accChartService, IApplicationSettingsService applicationSettingsService, 
            IDailySavingTrxService dailySavingTrxService, IGroupwiseReportService groupwiseReportService, 
            ISavingsAccCloseService savingsAccCloseService, IPortalSavingSummaryService portalSavingSummaryService)
        {
            this.specialSavingCollectionService = specialSavingCollectionService;
            this.centerService = centerService;
            this.productService = productService;
            this.membercategoryService = membercategoryService;
            this.officeService = officeService;
            this.purposeService = purposeService;
            this.memberService = memberService;
            this.savingSummaryService = savingSummaryService;
            this.accMasterService = accMasterService;
            this.accDetailService = accDetailService;
            this.savingCollectionService = savingCollectionService;
            this.ultimateReportService = ultimateReportService;
            this.accChartService = accChartService;
            this.applicationSettingsService = applicationSettingsService;
            this.dailySavingTrxService = dailySavingTrxService;
            this.groupwiseReportService = groupwiseReportService;
            this.savingsAccCloseService = savingsAccCloseService;
            this.portalSavingSummaryService= portalSavingSummaryService;
        }
        [HttpGet]
        public JsonResult GetMemberImageData(string MemberId)
        {
            try
            {

                var memberPhoto = memberService.GetByIdLong(Convert.ToInt64(MemberId));

                MemberViewModel myMember = new MemberViewModel();
                //myMember.MemberID = 1234;
                if (memberPhoto.MemberImg != null)
                {
                    myMember.MemberImage64String = Convert.ToBase64String(memberPhoto.MemberImg);
                    if (myMember.MemberImage64String == "")
                    {
                        myMember.MemberImage64String = @"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";
                    }
                }
                else
                {
                    myMember.MemberImage64String = @"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";
                }//End Of member Image

                if (memberPhoto.ThumbImg != null)
                {
                    myMember.MemberSignature64String = Convert.ToBase64String(memberPhoto.ThumbImg);
                    if (myMember.MemberSignature64String == "")
                    {
                        myMember.MemberSignature64String = @"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";
                    }
                }
                else
                {
                    myMember.MemberSignature64String = @"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";
                }// END of Signature

                List<MemberViewModel> List_ViewModel = new List<MemberViewModel>();

                List_ViewModel.Add(myMember);
                return Json(List_ViewModel.ToList(), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            { return Json(ex.ToString(), JsonRequestBehavior.AllowGet); }

        }
        [HttpPost]
        public ActionResult GetSpecialCollection(int jtStartIndex, int jtPageSize, string jtSorting, string filterColumn, string filterValue)
        {
            try
            {

                string vday = string.Empty;
                if (IsDayInitiated)
                    vday = TransactionDay;
                var specialLoandetail = specialSavingCollectionService.GetSpecialsavingCollectionDetailEmpWise(LoggedInOrganizationID, SessionHelper.LoginUserOfficeID.Value, vday, filterColumn, filterValue, Convert.ToInt16(LoggedInEmployeeID));
                var detail = specialLoandetail.ToList();

                var totalCount = detail.Count();
                var entities = detail.Skip(jtStartIndex).Take(jtPageSize);
                var currentPageRecords = Mapper.Map<IEnumerable<proc_get_SpecialSavingCollection_Result>, IEnumerable<SpecialSavingCollectionViewModel>>(entities);

                return Json(new { Result = "OK", Records = currentPageRecords, TotalRecordCount = totalCount });

                //var viewploansummary = Mapper.Map<IEnumerable<proc_get_SpecialSavingCollection_Result>, IEnumerable<SpecialSavingCollectionViewModel>>(detail);
                //return Json(new { Result = "OK", Records = viewploansummary });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }

        }
        public JsonResult GetProductList(string Member_id, string center_id)
        {
            List<ProductViewModel> List_ProductViewModel = new List<ProductViewModel>();
            var param = new { OfficeId = LoginUserOfficeID, MemberId = Member_id, CenterId = center_id };
            var div_items = ultimateReportService.GetProductSavingList(param);

            List_ProductViewModel = div_items.Tables[0].AsEnumerable()
            .Select(row => new ProductViewModel
            {
                ProductID = row.Field<Int16>("ProductID"),
                ProductCode = row.Field<string>("ProductCode"),
                ProductName = row.Field<string>("ProductName")
            }).ToList();

            var viewProduct = List_ProductViewModel.Select(x => x).ToList().Select(x => new SelectListItem
            {
                Value = x.ProductID.ToString(),
                Text = x.ProductCode.ToString() + " " + x.ProductName.ToString()
            });
            var d_items = new List<SelectListItem>();
            d_items.Add(new SelectListItem() { Text = "Please Select", Value = "0" });
            d_items.AddRange(viewProduct);
            return Json(d_items, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetInstallment(decimal SavingInstallment, decimal WithDrawal, decimal Penalty, decimal balance, string officeId, string centerId, string MemId, string ProdId, string noAccount)
        {
            decimal vLoanInstallment = 0;
            decimal savInsall;
            decimal with = 0;
            decimal penal = 0;
            decimal vWithDrawal;
            decimal vBal = 0;
            var model = new SpecialSavingCollectionViewModel();



            model.OfficeID = Convert.ToInt16(SessionHelper.LoginUserOfficeID.Value);
            model.CenterID = Convert.ToInt32(centerId);
            model.MemberID = Convert.ToInt64(MemId);
            model.ProductID = Convert.ToInt16(ProdId);
            model.NoOfAccount = Convert.ToInt16(noAccount);
            var entity = Mapper.Map<SpecialSavingCollectionViewModel, DailySavingTrx>(model);
            savInsall = SavingInstallment;
            with = WithDrawal;
            penal = Penalty;


            var getBal = specialSavingCollectionService.Proc_getSavingLastBalance(Convert.ToInt16(LoginUserOfficeID), Convert.ToInt32(entity.CenterID), Convert.ToInt64(entity.MemberID), Convert.ToInt16(entity.ProductID), Convert.ToInt16(noAccount), Convert.ToDecimal(entity.SavingInstallment), Convert.ToDecimal(entity.Withdrawal), TransactionDate, Convert.ToInt16(entity.TransType));
            var getdetail = getBal.ToList();
            var GetBalance = getdetail.Where(g => g.officeid == Convert.ToInt16(LoginUserOfficeID)).FirstOrDefault();


            //savInsall = sm.SavingInstallment;
            savInsall = SavingInstallment;
            with = WithDrawal;
            penal = Penalty;
            //Loantrm = sm.NoOfAccount;

            vLoanInstallment = (Convert.ToDecimal(GetBalance.Balance) + savInsall + Penalty - with);

            // var result = new { loan = vLoanInstallment.ToString(), savInstall = savInsall.ToString(), withdrawal = vWithDrawal.ToString(), penalty = penal.ToString() };
            var result = new { loan = vLoanInstallment.ToString(), savInstall = savInsall.ToString(), withdrawal = with.ToString(), penalty = penal.ToString() };
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetNoOfAccount(string officeId, string centerId, string MemId, string ProdId)
        {
            int vLoanTerm;
            int Loantrm = 0;
            decimal vBalance = 0;

            decimal savInsall = 0;
            decimal with = 0;
            decimal penal = 0;
            decimal vDueSavingInstallment = 0;
            var model = new SpecialSavingCollectionViewModel();
            model.OfficeID = Convert.ToInt16(SessionHelper.LoginUserOfficeID.Value);
            model.CenterID = Convert.ToInt32(centerId);
            model.MemberID = Convert.ToInt64(MemId);
            model.ProductID = Convert.ToInt16(ProdId);


                var entity = Mapper.Map<SpecialSavingCollectionViewModel, DailySavingTrx>(model);

            List<Proc_get_SavingLastBalance> getBal = new List<Proc_get_SavingLastBalance>();
            var param = new
            {
                @OfficeID = LoginUserOfficeID,
                @CenterID = Convert.ToInt32(entity.CenterID),
                @MemberID = Convert.ToInt64(entity.MemberID),
                @ProductID = Convert.ToInt16(entity.ProductID),
                @NoOfAccount = Convert.ToInt16(model.NoOfAccount),

                @DailySavinInstallment = Convert.ToDecimal(entity.SavingInstallment),
                @WithDrawal = Convert.ToDecimal(entity.Withdrawal),
                //@lcl_BusinessDate = TransactionDate,
                @lcl_BusinessDate = DateTime.UtcNow,
                @TransType = Convert.ToInt16(entity.TransType)

            };
            var div_items = ultimateReportService.GetSpecialSavingLastBalance(param);
            getBal = div_items.Tables[0].AsEnumerable()
           .Select(row => new Proc_get_SavingLastBalance
           {
               officeid = row.Field<Int32>("officeid"),
               Centerid = row.Field<Int32>("Centerid"),
               MemberID = row.Field<long>("MemberID"),
               ProductID = row.Field<short>("ProductID"),
               NoOfAccount = row.Field<Int32>("NoOfAccount"),
               SavingSummaryID = row.Field<long>("SavingSummaryID"),
               Balance = row.Field<decimal>("Balance"),
               SavingInstallment = row.Field<decimal>("SavingInstallment"),
               DueSavingInstallment= row.Field<decimal>("DueSavingInstallment")
           }).ToList();
            if (getBal.Count() > 0)
            {
                var getdetail = getBal.ToList();
                var GetBalance = getdetail.Where(g => g.officeid == Convert.ToInt16(LoginUserOfficeID)).FirstOrDefault();
                savInsall = GetBalance.SavingInstallment;
                with = 0;
                penal = GetBalance.Penalty;
                //Loantrm = sm.NoOfAccount;
                Loantrm = GetBalance.NoOfAccount;
                vBalance = (Convert.ToDecimal(GetBalance.Balance) + savInsall + penal) - with;
                vDueSavingInstallment = GetBalance.DueSavingInstallment;
                vLoanTerm = Loantrm;
               
            }
            else
            {
                savInsall = Convert.ToDecimal(0.00);
                with = Convert.ToDecimal(0.00);
                penal = Convert.ToDecimal(0.00);
                vDueSavingInstallment = 0;
                vLoanTerm = 0;
                vBalance = Convert.ToDecimal(0.00);
                vLoanTerm = 0;

            }
                var result = new { LoanTerm = vLoanTerm.ToString(), Balance = vBalance, savInstall = savInsall.ToString(), withdrawal = with.ToString(), penalty = penal.ToString(), DueSavingInstallment = vDueSavingInstallment.ToString() };
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetTranstype(string officeId, string centerId, string MemId, string ProdId, string TransTypeId, decimal SavingInstallment, decimal WithDrawal, decimal Penalty)
        {
            int vLoanTerm;
            int Loantrm = 0;
            decimal vBalance = 0;

            decimal savInsall = 0;
            decimal with = 0;
            decimal penal = 0;

            var model = new SpecialSavingCollectionViewModel();
            model.OfficeID = Convert.ToInt16(SessionHelper.LoginUserOfficeID.Value);
            model.CenterID = Convert.ToInt32(centerId);
            model.MemberID = Convert.ToInt64(MemId);
            model.ProductID = Convert.ToInt16(ProdId);


            var entity = Mapper.Map<SpecialSavingCollectionViewModel, DailySavingTrx>(model);



            var getBal = specialSavingCollectionService.Proc_getSavingLastBalance(Convert.ToInt16(LoginUserOfficeID), Convert.ToInt16(entity.CenterID), Convert.ToInt64(entity.MemberID), Convert.ToInt16(entity.ProductID), Convert.ToInt16(1), Convert.ToDecimal(entity.SavingInstallment), Convert.ToDecimal(entity.Withdrawal), TransactionDate, Convert.ToInt16(TransTypeId));
            var getdetail = getBal.ToList();
            var GetBalance = getdetail.Where(g => g.officeid == Convert.ToInt16(LoginUserOfficeID)).FirstOrDefault();


            //savInsall = sm.SavingInstallment;
            savInsall = GetBalance.SavingInstallment;
            Loantrm = GetBalance.NoOfAccount;
            vBalance = (Convert.ToDecimal(GetBalance.Balance) + savInsall + penal) - with;

            vLoanTerm = Loantrm;
          

            var result = new { LoanTerm = vLoanTerm.ToString(), Balance = vBalance, savInstall = savInsall.ToString(), withdrawal = with.ToString(), penalty = penal.ToString() };
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetLastInstallment(decimal SavingInstallment, decimal WithDrawal, decimal Penalty,decimal balance, string officeId, string centerId, string MemId, string ProdId, string noAccount, string TransTypeId )
        {
            int vLoanTerm;
            int Loantrm = 0;
            decimal vBalance = 0;

            decimal savInsall = SavingInstallment;
            decimal with = WithDrawal;
            decimal penal = Penalty;

            var model = new SpecialSavingCollectionViewModel();
            model.OfficeID = Convert.ToInt16(SessionHelper.LoginUserOfficeID.Value);
            model.CenterID = Convert.ToInt32(centerId);
            model.MemberID = Convert.ToInt64(MemId);
            model.ProductID = Convert.ToInt16(ProdId);
            model.NoOfAccount = Convert.ToInt16(noAccount);
            model.SavingInstallment = SavingInstallment;
            model.Withdrawal = WithDrawal;
            var entity = Mapper.Map<SpecialSavingCollectionViewModel, DailySavingTrx>(model);

            List<Proc_get_SavingLastBalance> getBal = new List<Proc_get_SavingLastBalance>();
            var param = new
            {
                @OfficeID = LoginUserOfficeID,
                @CenterID = Convert.ToInt32(entity.CenterID),
                @MemberID = Convert.ToInt64(entity.MemberID),
                @ProductID = Convert.ToInt16(entity.ProductID),
                @NoOfAccount = Convert.ToInt16(model.NoOfAccount),

                @DailySavinInstallment = Convert.ToDecimal(entity.SavingInstallment),
                @WithDrawal = Convert.ToDecimal(entity.Withdrawal),
                @lcl_BusinessDate = DateTime.UtcNow,
                @TransType = Convert.ToInt16(entity.TransType)

            };
            var div_items = ultimateReportService.Proc_getSavingLastBalance(param);
            getBal = div_items.Tables[0].AsEnumerable()
           .Select(row => new Proc_get_SavingLastBalance
           {
               officeid = row.Field<Int32>("officeid"),
               Centerid = row.Field<Int32>("Centerid"),
               MemberID = row.Field<long>("MemberID"),
               ProductID = row.Field<short>("ProductID"),
               NoOfAccount = row.Field<Int32>("NoOfAccount"),
               SavingSummaryID = row.Field<long>("SavingSummaryID"),
               Balance = row.Field<decimal>("Balance"),
               SavingInstallment = row.Field<decimal>("SavingInstallment")
           }).ToList();

            if (getBal.Count() > 0)
            {
                var getdetail = getBal.ToList();
                var GetBalance = getdetail.Where(g => g.officeid == Convert.ToInt16(LoginUserOfficeID)).FirstOrDefault();

                Loantrm = GetBalance.NoOfAccount;

                //vBalance = (Convert.ToDecimal(GetBalance.Balance) + savInsall + penal) - with;
                vBalance = (Convert.ToDecimal(GetBalance.Balance) + 0 + penal);

                vLoanTerm = Loantrm;

            }
            else
            {
                savInsall = Convert.ToDecimal(0.00);
                with = Convert.ToDecimal(0.00);
                penal = Convert.ToDecimal(0.00);
                //Loantrm = sm.NoOfAccount;
                //Loantrm = 0;
                vLoanTerm = 0;
                vBalance = Convert.ToDecimal(0.00);
                vLoanTerm = 0;

            }
            var result = new { LoanTerm = vLoanTerm.ToString(), Balance = vBalance, savInstall = savInsall.ToString(), withdrawal = with.ToString(), penalty = penal.ToString() };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        //public JsonResult GetLastInstallment(string officeId, string centerId, string MemId, string ProdId,string noAccount, string TransTypeId, decimal SavingInstallment, decimal WithDrawal, decimal Penalty)
        //{
        //    int vLoanTerm;
        //    int Loantrm = 0;
        //    decimal vBalance = 0;

        //    decimal savInsall = SavingInstallment;
        //    decimal with = WithDrawal;
        //    decimal penal = Penalty;

        //    var model = new SpecialSavingCollectionViewModel();
        //    model.OfficeID = Convert.ToInt16(SessionHelper.LoginUserOfficeID.Value);
        //    model.CenterID = Convert.ToInt32(centerId);
        //    model.MemberID = Convert.ToInt64(MemId);
        //    model.ProductID = Convert.ToInt16(ProdId);
        //    model.NoOfAccount = Convert.ToInt16(noAccount);

        //    var entity = Mapper.Map<SpecialSavingCollectionViewModel, DailySavingTrx>(model);

        //    List<Proc_get_SavingLastBalance> getBal = new List<Proc_get_SavingLastBalance>();
        //    var param = new
        //    {
        //        @OfficeID = LoginUserOfficeID,
        //        @CenterID = Convert.ToInt32(entity.CenterID),
        //        @MemberID = Convert.ToInt64(entity.MemberID),
        //        @ProductID = Convert.ToInt16(entity.ProductID),
        //        @NoOfAccount = Convert.ToInt16(model.NoOfAccount),

        //        @DailySavinInstallment = Convert.ToDecimal(entity.SavingInstallment),
        //        @WithDrawal = Convert.ToDecimal(entity.Withdrawal),
        //        @lcl_BusinessDate = TransactionDate,
        //        @TransType = Convert.ToInt16(entity.TransType)

        //    };
        //    var div_items = ultimateReportService.Proc_getSavingLastBalance(param);
        //    getBal = div_items.Tables[0].AsEnumerable()
        //   .Select(row => new Proc_get_SavingLastBalance
        //   {
        //       officeid = row.Field<Int32>("officeid"),
        //       Centerid = row.Field<Int32>("Centerid"),
        //       MemberID = row.Field<long>("MemberID"),
        //       ProductID = row.Field<short>("ProductID"),
        //       NoOfAccount = row.Field<Int32>("NoOfAccount"),
        //       SavingSummaryID = row.Field<long>("SavingSummaryID"),
        //       Balance = row.Field<decimal>("Balance"),
        //       SavingInstallment = row.Field<decimal>("SavingInstallment")
        //   }).ToList();

        //    if (getBal.Count() > 0)
        //    {
        //        var getdetail = getBal.ToList();
        //        var GetBalance = getdetail.Where(g => g.officeid == Convert.ToInt16(LoginUserOfficeID)).FirstOrDefault();

        //        Loantrm = GetBalance.NoOfAccount;

        //    vBalance = (Convert.ToDecimal(GetBalance.Balance) + savInsall + penal) - with;

        //    vLoanTerm = Loantrm;

        //    }
        //    else
        //    {
        //        savInsall = Convert.ToDecimal(0.00);
        //        with = Convert.ToDecimal(0.00);
        //        penal = Convert.ToDecimal(0.00);
        //        //Loantrm = sm.NoOfAccount;
        //        //Loantrm = 0;
        //        vLoanTerm = 0;
        //        vBalance = Convert.ToDecimal(0.00);
        //        vLoanTerm = 0;

        //    }
        //    var result = new { LoanTerm = vLoanTerm.ToString(), Balance = vBalance, savInstall = savInsall.ToString(), withdrawal = with.ToString(), penalty = penal.ToString() };
        //    return Json(result, JsonRequestBehavior.AllowGet);
        //}
        public JsonResult GetSpecialSavingLastBalance(string officeId, string centerId, string MemId, string ProdId, string NoOfAccount = "0")
        {
            int vLoanTerm;
            int Loantrm = 0;
            decimal vBalance = 0;

            decimal savInsall = 0;
            decimal with = 0;
            decimal penal = 0;
            decimal vDueSavingInstallment = 0;
            var model = new SpecialSavingCollectionViewModel();
            model.OfficeID = Convert.ToInt16(SessionHelper.LoginUserOfficeID.Value);
            model.CenterID = Convert.ToInt32(centerId);
            model.MemberID = Convert.ToInt64(MemId);
            model.ProductID = Convert.ToInt16(ProdId);
            model.NoOfAccount = Convert.ToInt32(NoOfAccount);


            var entity = Mapper.Map<SpecialSavingCollectionViewModel, DailySavingTrx>(model);



            List<Proc_get_SavingLastBalance> getBal = new List<Proc_get_SavingLastBalance>();
            var param = new
            {
                @OfficeID = LoginUserOfficeID,
                @CenterID = Convert.ToInt32(entity.CenterID),
                @MemberID = Convert.ToInt64(entity.MemberID),
                @ProductID = Convert.ToInt16(entity.ProductID),
                @NoOfAccount = Convert.ToInt16(model.NoOfAccount),

                @DailySavinInstallment = Convert.ToDecimal(entity.SavingInstallment),
                @WithDrawal = Convert.ToDecimal(entity.Withdrawal),
                //@lcl_BusinessDate = TransactionDate,
                @lcl_BusinessDate = DateTime.UtcNow,
                @TransType = Convert.ToInt16(entity.TransType)

            };
            var div_items = ultimateReportService.GetSpecialSavingLastBalance(param);

            getBal = div_items.Tables[0].AsEnumerable()
            .Select(row => new Proc_get_SavingLastBalance
            {
                officeid = row.Field<Int32>("officeid"),
                Centerid = row.Field<Int32>("Centerid"),
                MemberID = row.Field<long>("MemberID"),
                ProductID = row.Field<short>("ProductID"),
                NoOfAccount = row.Field<Int32>("NoOfAccount"),
                SavingSummaryID = row.Field<long>("SavingSummaryID"),
                Balance = row.Field<decimal>("Balance"),
                SavingInstallment = row.Field<decimal>("SavingInstallment"),
                DueSavingInstallment= row.Field<decimal>("DueSavingInstallment")
            }).ToList();

            //END

            if (getBal.Count() > 0)
            {
                var getdetail = getBal.ToList();
                var GetBalance = getdetail.Where(g => g.officeid == Convert.ToInt16(LoginUserOfficeID)).FirstOrDefault();

                savInsall = GetBalance.SavingInstallment;
                with = 0;
                penal = GetBalance.Penalty;
                //Loantrm = sm.NoOfAccount;
                Loantrm = GetBalance.NoOfAccount;
                vBalance = (Convert.ToDecimal(GetBalance.Balance) + savInsall + penal) - with;
                vDueSavingInstallment = GetBalance.DueSavingInstallment;
                vLoanTerm = Loantrm;
             
            }
            else
            {
                savInsall = Convert.ToDecimal(0.00);
                with = Convert.ToDecimal(0.00);
                penal = Convert.ToDecimal(0.00);
                vLoanTerm = 0;
                vBalance = Convert.ToDecimal(0.00);
                vLoanTerm = 0;
                vDueSavingInstallment = 0;
            }

            var result = new { LoanTerm = vLoanTerm, Balance = vBalance, savInstall = savInsall, withdrawal = with, penalty = penal, DueSavingInstallment=vDueSavingInstallment };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        private void MapDropDownList(SpecialSavingCollectionViewModel model)
        {

            if (!SessionHelper.LoginUserOfficeID.HasValue)
            {
                RedirectToAction("Login", "Account");
                return;
            }
            var Transtype = new List<SelectListItem>();
            Transtype.Add(new SelectListItem() { Text = "Cash", Value = "20", Selected = true });
            Transtype.Add(new SelectListItem() { Text = "Transfer", Value = "21" });
            Transtype.Add(new SelectListItem() { Text = "Bank", Value = "22" });

            model.cashListItems = Transtype.AsEnumerable();

            var allmember = memberService.SearchMember(Convert.ToInt16(LoginUserOfficeID), Convert.ToInt16(LoggedInOrganizationID));

            var viewMember = allmember.Select(m => new SelectListItem() { Text = string.Format("{0} - {1}", m.MemberCode, (string.IsNullOrEmpty(m.FirstName) ? "" : m.FirstName) + ' ' + (string.IsNullOrEmpty(m.MiddleName) ? "" : m.MiddleName) + ' ' + (string.IsNullOrEmpty(m.LastName) ? "" : m.LastName)), Value = m.MemberID.ToString() });

            model.memberListItems = viewMember;
            ViewData["Member"] = viewMember;

            var allpurpose = purposeService.SearchPurpose(Convert.ToInt16(LoggedInOrganizationID));

            var viewPurpose = allpurpose.Select(m => new SelectListItem() { Text = string.Format("{0} - {1}", m.PurposeCode, m.PurposeName), Value = m.PurposeID.ToString() });

            model.purposeListItems = viewPurpose;


            string vCoday = TransactionDay;


            var param1 = new { @EmpID = LoggedInEmployeeID };
            var LoanInstallMent = ultimateReportService.GetCenterROleWise(param1);

            IEnumerable<Center> allcenter;
            if (LoanInstallMent != null)
            {
                var empType = LoanInstallMent.Tables[0].Rows[0]["Name"].ToString();
                if (empType == "FO")
                {
                    allcenter = centerService.GetByOfficeId(SessionHelper.LoginUserOfficeID.Value, Convert.ToInt32(LoggedInOrganizationID), Convert.ToInt16(LoggedInEmployeeID));
                }
                else

                    allcenter = centerService.GetByOfficeId(SessionHelper.LoginUserOfficeID.Value, Convert.ToInt32(LoggedInOrganizationID));
            }

            else
                allcenter = centerService.GetByOfficeId(SessionHelper.LoginUserOfficeID.Value, Convert.ToInt32(LoggedInOrganizationID));


            //var allcenter = centerService.GetByOfficeId(SessionHelper.LoginUserOfficeID.Value, Convert.ToInt16(LoggedInOrganizationID), Convert.ToInt16(LoggedInEmployeeID));

            var viewCenter = allcenter.Select(m => new SelectListItem() { Text = string.Format("{0} - {1}", m.CenterCode, m.CenterName), Value = m.CenterID.ToString() });

            model.centerListItems = viewCenter;

            var alloffice = officeService.GetAll().Where(s => s.OfficeID == LoginUserOfficeID && s.OrgID == LoggedInOrganizationID);

            var viewOffice = alloffice.Select(m => new SelectListItem() { Text = string.Format("{0} - {1}", m.OfficeCode, m.OfficeName), Value = m.OfficeID.ToString() });

            model.officeListItems = viewOffice;

            var allSearchProd = productService.SearchProduct(0, Convert.ToInt16(LoggedInOrganizationID), "S");
            var viewProdList = allSearchProd.Select(x => x).ToList().Select(x => new SelectListItem
            {
                Value = x.ProductID.ToString(),
                Text = string.Format("{0} - {1}", x.ProductCode.ToString(), x.ProductName.ToString())
            });
            var proditems = new List<SelectListItem>();
            proditems.Add(new SelectListItem() { Text = "", Value = "", Selected = true });
            model.productListItems = proditems;


            var allmembercategory = membercategoryService.GetAll().Where(m => m.IsActive == true && m.OrgID == LoggedInOrganizationID);

            var viewmembercategory = allmembercategory.Select(m => new SelectListItem() { Text = string.Format("{0} - {1}", m.MemberCategoryID, m.CategoryName), Value = m.MemberCategoryID.ToString() });

            model.membercategoryListItems = viewmembercategory;

            IEnumerable<AccChart> allAccountCode = new List<AccChart>();
            IEnumerable<SelectListItem> viewAccountCode = new List<SelectListItem>();

            var BankCode = accChartService.GetByAccCode(applicationSettingsService.GetAll().Where(c => c.OfficeID == LoginUserOfficeID).FirstOrDefault().BankAccount);

            if (allAccountCode != null && BankCode != null)
            {
                allAccountCode = accChartService.GetAll().Where(a => a.SecondLevel == BankCode.SecondLevel && a.ModuleID == 1);
                viewAccountCode = allAccountCode.Select(x => x).ToList().Select(x => new SelectListItem
                {
                    Value = x.AccCode.ToString(),
                    Text = x.AccCode.ToString() + " " + x.AccName.ToString()
                });
            }               
            var Acc_items = new List<SelectListItem>();
            Acc_items.Add(new SelectListItem() { Text = "Please Select", Value = "0", Selected = true });
            Acc_items.AddRange(viewAccountCode);
            model.GetAccountCodeList = Acc_items;



        }
        public ActionResult GetAccountCodeList()
        {
            List<AccChartViewModel> List_ProductViewModel = new List<AccChartViewModel>();
            // var param = new { OfficeId = LoginUserOfficeID, MemberId = Member_id, CenterId = center_id };
            var BankCode = accChartService.GetByAccCode(applicationSettingsService.GetAll().Where(c => c.OfficeID == LoginUserOfficeID).FirstOrDefault().BankAccount);
            var viewAccountCode = accChartService.GetAll().Where(s => s.SecondLevel == BankCode.SecondLevel && s.ModuleID == 1);
            var accLIst = viewAccountCode.Select(c => new { DisplayText = c.AccCode + " " + c.AccName, Value = c.AccCode }).OrderBy(s => s.DisplayText);


            return Json(accLIst, JsonRequestBehavior.AllowGet);


        }
        public ActionResult GetMemberList(string memberid, string centerId)
        {
            
            var MemberByCenterSessionKey = string.Format("MemberByCenterSessionKey_{0}", centerId);
            var memberList = new List<Member>();
            if (Session[MemberByCenterSessionKey] != null)
                memberList = Session[MemberByCenterSessionKey] as List<Member>;
            else
            {
                List<Member> List_MemberViewModel = new List<Member>();
                var paramFOWISE = new { OfficeID = LoginUserOfficeID, CenterId=centerId, OrgID=LoggedInOrganizationID };
                var div_items = groupwiseReportService.GetDataDataseAccess(paramFOWISE, "GetByCenterId");

                List_MemberViewModel = div_items.Tables[0].AsEnumerable()
                .Select(row => new Member
                {
                    MemberID = row.Field<long>("MemberID"),
                    MemberCode = row.Field<string>("MemberCode"),
                    FirstName = row.Field<string>("FirstName"),
                    //MiddleName = row.Field<string>("MiddleName"),
                    LastName= row.Field<string>("LastName")
                }).ToList();

                //var mbr = memberService.GetByCenterId(int.Parse(centerId), Convert.ToInt16(LoginUserOfficeID), Convert.ToInt16(LoggedInOrganizationID)).ToList();
                Session[MemberByCenterSessionKey] = List_MemberViewModel;
                memberList = List_MemberViewModel;
            }
            var members = memberList.Where(m => 
                    string.Format("{0} - {1}", m.MemberCode, 
                    (string.IsNullOrEmpty(m.FirstName) ? "" : m.FirstName) + ' ' + 
                    (string.IsNullOrEmpty(m.LastName) ? "" : m.LastName))
                .ToLower().Contains(memberid.ToLower()))
                .Select(m1 => new { m1.MemberID, MemberName = string
                .Format("{0} - {1}", m1.MemberCode, 
                (string.IsNullOrEmpty(m1.FirstName) ? "" : m1.FirstName) + ' ' + 
                (string.IsNullOrEmpty(m1.LastName) ? "" : m1.LastName)) }).ToList();

            return Json(members, JsonRequestBehavior.AllowGet);
        }
        public Member GetMember(long memberid)
        {
            var mbr = memberService.GetByIdLong(memberid);
            return mbr;
        }
        public Center GetEmployee(int employeeid)
        {
            var mbr = centerService.GetById(employeeid);
            return mbr;
        }
        public Product GetProduct(int productid)
        {
            var mbr = productService.GetById(productid);
            return mbr;
        }
        // GET: SpecialSavingCollection
        public ActionResult Index()
        {
            return View();
        }
        // GET: SpecialSavingCollection/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }
        // GET: SpecialSavingCollection/Create
        public ActionResult Create()
        
        {
            // changed 17/08/2020
            var model = new SpecialSavingCollectionViewModel();
            //if (IsDayInitiated)
            //model.TransactionDate = TransactionDate;
            model.TransactionDate = DateTime.UtcNow;
            MapDropDownList(model);
            specialSavingCollectionService.delVoucher(LoginUserOfficeID, model.TransactionDate, LoggedInOrganizationID);
            return View(model);
        }
        // POST: SpecialSavingCollection/Create
        [HttpPost]
        public ActionResult Create(SpecialSavingCollectionViewModel model)
        {
            try
            {
                int return_value = 1;
                var return_msg = "";
                //if (!IsDayInitiated)
                //{
                //    return GetErrorMessageResult("Please run the start work process");
                //}

                var entity = Mapper.Map<SpecialSavingCollectionViewModel, DailySavingTrx>(model);

                if (entity.Balance == 0)
                {
                    entity.Balance = 0;
                }
                entity.OrgID = Convert.ToInt16(LoggedInOrganizationID);

                if (entity.TransType == 0)
                {
                    return GetErrorMessageResult("Please Transaction Type");

                }
                if (entity.ChequeNo == null)
                {
                    entity.ChequeNo = "NA";
                }
                if (entity.TransType == 22)
                {
                    if (entity.BankName == "0")
                    {
                       // entity.BankName = '';
                        return GetErrorMessageResult("Please put the BankName");
                    }
                    if (entity.ChequeNo == "NA")
                    {

                        return GetErrorMessageResult("Please put the ChequeNo");
                    }
                   
                }
                if (entity.BankName == null)
                {
                    entity.BankName = "NA";

                }
                if (entity.Balance < 0)
                {
                    return GetErrorMessageResult("Withdrawal Amount more than Balance");

                }
                if (entity.SavingInstallment<0 || entity.Withdrawal < 0 || entity.Penalty < 0)
                {
                    return GetErrorMessageResult("Negative Value are not allowed");
                }
                var param = 
                    new { 
                        OfficeID = LoginUserOfficeID,
                        CenterID = entity.CenterID,
                        MemberID = entity.MemberID,
                        ProductID = entity.ProductID,
                        NoOfAccount = entity.NoOfAccount,
                        DailySavinInstallment = entity.SavingInstallment, 
                        WithDrawal = entity.Withdrawal,
                        //lcl_BusinessDate = TransactionDate, 
                        lcl_BusinessDate = DateTime.UtcNow, 
                        CreateUser = LoggedInEmployeeID,
                        CreateDate = DateTime.UtcNow, 
                        TransType = entity.TransType, 
                        Penalty = entity.Penalty, 
                        BankName = entity.BankName,
                        CheequekNo = entity.ChequeNo };
                var div_items = ultimateReportService.PenaltyWithDailySavingBank(param);
                var getResult = div_items.Tables[0].AsEnumerable().Select(p => new SpecialSavingCollectionViewModel
                {
                    return_value = p.Field<int>("return_value"),
                    return_msg = p.Field<string>("return_msg")
                });
                return_value = getResult.AsEnumerable().First().return_value;
                return_msg = getResult.AsEnumerable().First().return_msg;
                if (return_value == 0)
                    return Json(new { return_value = return_value, return_msg = return_msg });


                //if (return_value == 1 && return_msg == "Success")
                //    return Json(new { return_value = return_value, return_msg = return_msg });
                //else
                //    return Json(new { return_value = return_value, return_msg = return_msg });
                return GetSuccessMessageResult();
            }
            catch (Exception ex)
            {
                return GetErrorMessageResult(ex);
            }
        }
        // GET: SpecialSavingCollection/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }
        // POST: SpecialSavingCollection/Edit/5
        [HttpPost]
        public ActionResult Edit(SpecialSavingCollectionViewModel Model)
        {
            try
            {
                if (!IsDayInitiated)
                {
                    return GetErrorMessageResult("Please run the start work process");
                }
                specialSavingCollectionService.delVoucher(LoginUserOfficeID, TransactionDate, LoggedInOrganizationID);

                var loanser = specialSavingCollectionService.GetByIdLong(Convert.ToInt64(Model.DailySavingTrxID));
                var entity = Mapper.Map<SpecialSavingCollectionViewModel, DailySavingTrx>(Model);
                if (ModelState.IsValid)
                {

                    var errors = specialSavingCollectionService.IsValidLoan(loanser);

                    if (errors.ToList().Count == 0)
                    {
                        if (entity.ChequeNo == null)
                        {
                            entity.ChequeNo = "NA";
                        }
                        if (entity.BankName == null)
                        {
                            entity.BankName = "NA";
                        }

                        var param = new { OfficeID = LoginUserOfficeID, CenterID = loanser.CenterID, MemberID = loanser.MemberID, ProductID = loanser.ProductID, NoOfAccount = loanser.NoOfAccount, DailySavinInstallment = entity.SavingInstallment, WithDrawal = entity.Withdrawal, lcl_BusinessDate = TransactionDate, CreateUser = LoggedInEmployeeID, CreateDate = TransactionDate, TransType = entity.TransType, Penalty = entity.Penalty, BankName = entity.BankName, CheequekNo = entity.ChequeNo };
                        var div_items = ultimateReportService.PenaltyWithDailySavingBank(param);

                        // specialSavingCollectionService.setDailySavingTrx(Convert.ToInt16(LoginUserOfficeID), Convert.ToInt16(loanser.CenterID), Convert.ToInt64(loanser.MemberID), Convert.ToInt16(loanser.ProductID), Convert.ToInt16(loanser.NoOfAccount), Convert.ToDecimal(entity.SavingInstallment), Convert.ToDecimal(entity.Withdrawal), TransactionDate, Convert.ToString(LoggedInEmployeeID), Convert.ToDateTime(TransactionDate), Convert.ToInt16(entity.TransType));
                        return GetSuccessMessageResult();
                    }
                    else
                        return Json(new { Result = "ERROR" });
                }
                return Json(new { Result = "OK" });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }
        // GET: SpecialSavingCollection/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }
        // POST: SpecialSavingCollection/Delete/5
        [HttpPost]
        public ActionResult Delete(int DailySavingTrxID, SpecialLoanCollectionViewModel model)
        {
            try
            {
                specialSavingCollectionService.delVoucher(LoginUserOfficeID, TransactionDate, LoggedInOrganizationID);
                var param1 = new { @Qtype = 2, @DailyLoanTrxID = DailySavingTrxID, @OrgID = LoggedInOrganizationID };
                var LoanInstallMent = ultimateReportService.DelSpecialLoanCollection(param1);
                  // TODO: Add delete logic here
                return Json(new { Result = "OK" });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        public JsonResult CheckMemberInDailySavingTrx(long memberID)
        {
            var result = 0;
            var message = "";
            var checkMember = dailySavingTrxService.GetMany(p => p.MemberID == memberID && p.TransType == 10);
            if (checkMember.Any()){
                result = 1;
                message = "Member belongs to regular collection.Want to collect as a special?";
                return Json( new { result = result, message = message}, JsonRequestBehavior.AllowGet);
            }
            else{

            }
            return Json(new { result = result, message = message }, JsonRequestBehavior.AllowGet);
        }

        // GET: PortalSavingsAccClose
        public ActionResult PortalSavingsAccCloseIndex()
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
                    var savingsAccCloseDetail = savingsAccClose.Skip(jtStartIndex).Take(jtPageSize).ToList();
                    return Json(new { Result = "OK", Records = savingsAccCloseMap, TotalCountRecord = savingsAccClose.Count() });
                }
                return Json(new { Result = "OK", TotalCountRecord = savingsAccClose.Count() });
            }
            catch (Exception ex)
            {

                return Json(new { Result = "Error", Message = ex.Message });
            }
        }
        // GET: PortalSavingsAccClose/Create
        //[HttpPost]
        public ActionResult CreatePortalSavingsAccClose(int id)
        {
            try
            {
                var getSavingSummaryId = portalSavingSummaryService.GetById(id);
                var model = Mapper.Map<PortalSavingSummary, SpecialSavingCollectionViewModel>(getSavingSummaryId);
                ViewBag.MemberName = string.Format("{0} - {1}", model.MemberCode, model.MemberName);
                model.TransactionDate = DateTime.UtcNow;
                MapDropDownList(model);
                specialSavingCollectionService.delVoucher(LoginUserOfficeID, model.TransactionDate, LoggedInOrganizationID);

                return View(model);
            }
            catch (Exception ex)
            {

                throw ex;
            }
                //var model = new SpecialSavingCollectionViewModel();
                //var model = new SpecialSavingCollectionViewModel();
                //model.TransactionDate = DateTime.UtcNow;
                //MapDropDownList(model);
                //specialSavingCollectionService.delVoucher(LoginUserOfficeID, LoggedInOrganizationID);
        }
    }
}
