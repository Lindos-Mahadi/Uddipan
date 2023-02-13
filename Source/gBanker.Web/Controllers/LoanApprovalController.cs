using AutoMapper;
using gBanker.Data.CodeFirstMigration.Db;
using gBanker.Data.DBDetailModels;
using gBanker.Service;
using gBanker.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using gBanker.Web.Core.Extensions;
using gBanker.Web.Helpers;
using gBanker.Service.ReportServies;
using System.IO;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using System.Data;
using gBanker.Data.CodeFirstMigration;
using System.Data.Entity.Validation;
using gBanker.Service.StoredProcedure;
using System.Text;
using System.Web.ApplicationServices;

namespace gBanker.Web.Controllers
{
    public class LoanApprovalController : BaseController
    {
        #region Variables
        private readonly IExpireInfoService expireInfoService;
        private readonly IBranchService branchService;
        private readonly IOfficeService officeService;
        private readonly ILoanSummaryService loansSummaryService;
        private readonly IProductService productService;
        private readonly IMemberCategoryService membercategoryService;
        private readonly ICenterService centerService;
        private readonly IPurposeService purposeService;
        private readonly IMemberService memberService;
        private readonly ILoanApprovalService loanapprovalService;
        private readonly IInvestorService investorService;
        private readonly IMemberPassBookRegisterService memberPassBookRegisterService;
        private readonly IUltimateReportService ultimateReportService;
        private readonly IWeeklyReportService weeklyReportService;
        private readonly IEmployeeSPService employeeSPService;
        private readonly IOrganizationService organizationService;
        private readonly IApproveCellingService ApproveCellingService;
        private readonly IGroupwiseReportService groupwiseReportService;
        private readonly IPortalLoanSummaryService portalLoanSummaService;
        private readonly IFileService fileService;

        // GET: LoanApproval
        public LoanApprovalController(
            ILoanSummaryService loansSummaryService, 
            IUltimateReportService ultimateReportService,
            ILoanApprovalService loanapprovalService,
            IProductService productService,
            IMemberCategoryService membercategoryService,
            IOfficeService officeService,
            ICenterService centerService,
            IPurposeService purposeService,
            IMemberService memberService,
            IInvestorService investorService,
            IMemberPassBookRegisterService memberPassBookRegisterService,
            IWeeklyReportService weeklyReportService,
            IExpireInfoService expireInfoService,
            IOrganizationService organizationService,
            IEmployeeSPService employeeSPService,
            IApproveCellingService ApproveCellingService,
            IGroupwiseReportService groupwiseReportService,
            IPortalLoanSummaryService portalLoanSummaryService,
            IFileService fileService)
        {
            this.loansSummaryService = loansSummaryService;
            this.productService = productService;
            this.membercategoryService = membercategoryService;
            this.officeService = officeService;
            this.centerService = centerService;
            this.purposeService = purposeService;
            this.memberService = memberService;
            this.loanapprovalService = loanapprovalService;
            this.investorService = investorService;
            this.memberPassBookRegisterService = memberPassBookRegisterService;
            this.ultimateReportService = ultimateReportService;
            this.weeklyReportService = weeklyReportService;
            this.expireInfoService = expireInfoService;
            this.organizationService = organizationService;
            this.employeeSPService = employeeSPService;
            this.ApproveCellingService = ApproveCellingService;
            this.groupwiseReportService = groupwiseReportService;
            this.portalLoanSummaService = portalLoanSummaryService;
            this.fileService = fileService;
        }
        #endregion

        #region Methods
        [HttpGet]
        public ActionResult Request()
        {
            if (Session["type"] != null && Session["resulttype"] != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Request");
            }
        }

        public JsonResult GetLoanApprovals(int jtStartIndex, int jtPageSize, string jtSorting, string filterColumn, string filterValue)
        {
            try
            {
                long totalCount;

                var param1 = new { @EmpID = LoggedInEmployeeID };
                var LoanInstallMent = ultimateReportService.GetCenterROleWise(param1);

                IEnumerable<DBLoanApproveDetailModel> allSavingsummary;
                if (LoanInstallMent != null)
                {
                    var empType = LoanInstallMent.Tables[0].Rows[0]["Name"].ToString();
                    if (empType == "FO")
                    {
                        allSavingsummary = loansSummaryService.GetLoanApproveDetailPaged(SessionHelper.LoginUserOfficeID.Value, filterColumn, filterValue, jtStartIndex, jtPageSize, jtSorting, out totalCount, TransactionDate, Convert.ToInt16(LoggedInOrganizationID), Convert.ToInt16(LoggedInEmployeeID));
                    }
                    else

                        allSavingsummary = loansSummaryService.GetLoanApproveDetailPaged(SessionHelper.LoginUserOfficeID.Value, filterColumn, filterValue, jtStartIndex, jtPageSize, jtSorting, out totalCount, TransactionDate, Convert.ToInt16(LoggedInOrganizationID));
                }
                else
                    allSavingsummary = loansSummaryService.GetLoanApproveDetailPaged(SessionHelper.LoginUserOfficeID.Value, filterColumn, filterValue, jtStartIndex, jtPageSize, jtSorting, out totalCount, TransactionDate, Convert.ToInt16(LoggedInOrganizationID));
                var currentPageRecords = Mapper.Map<IEnumerable<DBLoanApproveDetailModel>, IEnumerable<LoanApprovalViewModel>>(allSavingsummary);
                return Json(new { Result = "OK", Records = currentPageRecords, TotalRecordCount = totalCount });

            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }

        }

        public JsonResult GetPortalLoanApprovals(int jtStartIndex, int jtPageSize, string jtSorting, string filterColumn, string filterValue)
        {
            try
            {
                //long totalCount;

                //var param1 = new { @EmpID = LoggedInEmployeeID };
                //var LoanInstallMent = ultimateReportService.GetCenterROleWise(param1);

                //IEnumerable<DBLoanApproveDetailModel> allSavingsummary;
                //if (LoanInstallMent != null)
                //{
                //    var empType = LoanInstallMent.Tables[0].Rows[0]["Name"].ToString();
                //    if (empType == "FO")
                //    {
                //        allSavingsummary = loansSummaryService.GetLoanApproveDetailPaged(SessionHelper.LoginUserOfficeID.Value, filterColumn, filterValue, jtStartIndex, jtPageSize, jtSorting, out totalCount, TransactionDate, Convert.ToInt16(LoggedInOrganizationID), Convert.ToInt16(LoggedInEmployeeID));
                //    }
                //    else

                //        allSavingsummary = loansSummaryService.GetLoanApproveDetailPaged(SessionHelper.LoginUserOfficeID.Value, filterColumn, filterValue, jtStartIndex, jtPageSize, jtSorting, out totalCount, TransactionDate, Convert.ToInt16(LoggedInOrganizationID));
                //}
                //else
                //    allSavingsummary = loansSummaryService.GetLoanApproveDetailPaged(SessionHelper.LoginUserOfficeID.Value, filterColumn, filterValue, jtStartIndex, jtPageSize, jtSorting, out totalCount, TransactionDate, Convert.ToInt16(LoggedInOrganizationID));
                //var currentPageRecords = Mapper.Map<IEnumerable<DBLoanApproveDetailModel>, IEnumerable<LoanApprovalViewModel>>(allSavingsummary);
                //return Json(new { Result = "OK", Records = currentPageRecords, TotalRecordCount = totalCount });

                //var portalLoans = portalLoanSummaService.GetAll().Take(jtPageSize).Skip(jtStartIndex);
                var portalLoans = portalLoanSummaService.GetMany(x => x.ApprovalStatus == true && x.OfficeID == LoginUserOfficeID).Take(jtPageSize).Skip(jtStartIndex);
                var totalCount = portalLoans.Count();

                var currentPageRecords = Mapper.Map<IEnumerable<PortalLoanSummary>, List<LoanApprovalViewModel>>(portalLoans.ToList());
                return Json(new { Result = "OK", Records = currentPageRecords, TotalRecordCount = totalCount });

            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }

        }
        private void MapDropDownList(LoanApprovalViewModel model)
        {
            if (!SessionHelper.LoginUserOfficeID.HasValue)
            {
                RedirectToAction("Login", "Account");
                return;
            }
            var allmember = memberService.SearchMember(Convert.ToInt16(LoginUserOfficeID), Convert.ToInt16(LoggedInOrganizationID));
            var viewMember = allmember.Select(m => new SelectListItem() { Text = string.Format("{0} - {1}", m.MemberCode, (string.IsNullOrEmpty(m.FirstName) ? "" : m.FirstName) + ' ' + (string.IsNullOrEmpty(m.MiddleName) ? "" : m.MiddleName) + ' ' + (string.IsNullOrEmpty(m.LastName) ? "" : m.LastName) + ' ' + (string.IsNullOrEmpty(m.RefereeName) ? "" : m.RefereeName)), Value = m.MemberID.ToString() });
            model.memberListItems = viewMember;
            ViewData["Member"] = viewMember;


            var allpurpose = purposeService.SearchPurpose(Convert.ToInt16(LoggedInOrganizationID));
            var viewPurpose = allpurpose.Select(m => new SelectListItem() { Text = string.Format("{0} - {1}", m.PurposeCode, m.PurposeName), Value = m.PurposeID.ToString() });
            model.purposeListItems = viewPurpose;

            // Center Starts

            var param1 = new { @EmpID = LoggedInEmployeeID };
            var LoanInstallMent = ultimateReportService.GetCenterROleWise(param1);
            List<CenterViewModel> List_CenterViewModel = new List<CenterViewModel>();
            DataSet centerInfo;
            var paramC = new { OfficeId = LoginUserOfficeID };

            if (LoanInstallMent != null)
            {
                var empType = LoanInstallMent.Tables[0].Rows[0]["Name"].ToString();
                if (empType == "FO")
                {
                    var paramFOWISE = new { OfficeId = LoginUserOfficeID, EmpID = LoggedInEmployeeID, empType = empType };
                    centerInfo = groupwiseReportService.GetDataDataseAccess(paramFOWISE, "GetOnlyCenterFOWISE");
                }
                else

                    centerInfo = groupwiseReportService.GetDataDataseAccess(paramC, "GetOnlyCenter");
            }
            else
            {
                paramC = new { OfficeId = LoginUserOfficeID };
                centerInfo = groupwiseReportService.GetDataDataseAccess(paramC, "GetOnlyCenter");
            }

            List_CenterViewModel = centerInfo.Tables[0].AsEnumerable()
            .Select(row => new CenterViewModel
            {
                CenterID = row.Field<int>("CenterID"),
                CenterCode = row.Field<string>("CenterCode"),
                CenterName = row.Field<string>("CenterName")
            }).ToList();

            var viewCenter = List_CenterViewModel.Select(x => x).ToList().Select(x => new SelectListItem
            {
                Value = x.CenterID.ToString(),
                Text = x.CenterCode.ToString() + ", " + x.CenterName.ToString()
            });
            var center_items = new List<SelectListItem>();
            if (viewCenter.ToList().Count > 0)
            {
                center_items.Add(new SelectListItem() { Text = "Select All", Value = "0", Selected = true });
            }
            center_items.AddRange(viewCenter);
            //var centerList = allcenter.Select(m => new SelectListItem() { Text = string.Format("{0} - {1}", m.CenterCode, m.CenterName), Value = m.CenterID.ToString() });
            model.centerListItems = center_items;

            // Center Ends

            var alloffice = officeService.GetAll().Where(l => l.OfficeID == SessionHelper.LoginUserOfficeID.Value && l.OrgID == LoggedInOrganizationID);
            var viewOffice = alloffice.Select(m => new SelectListItem() { Text = string.Format("{0} - {1}", m.OfficeCode, m.OfficeName), Value = m.OfficeID.ToString() });
            model.officeListItems = viewOffice;

            List<ProductViewModel> List_MemberPassBookRegisterViewModel = new List<ProductViewModel>();
            var param = new { PaymentFrq = string.IsNullOrEmpty(model.frequencyMode) ? "0" : model.frequencyMode };
            var div_items = ultimateReportService.GetProductListByFrequencyMode(param);

            var viewProduct = div_items.Tables[0].AsEnumerable().Select(row => new SelectListItem
            {
                Value = row.Field<Int16>("ProductID").ToString(),
                Text = row.Field<string>("ProductCode") + ' ' + row.Field<string>("ProductName")
            });
            var proditems = new List<SelectListItem>();
            proditems.Add(new SelectListItem() { Text = "Please Select", Value = "0", Selected = true });
            proditems.AddRange(viewProduct);
            model.productListItems = proditems;

            var allInvestor = investorService.GetAll().Where(i => i.IsActive == true && i.OrgID == LoggedInOrganizationID).OrderBy(i => i.InvestorCode);
            var viewInvestor = allInvestor.Select(m => new SelectListItem() { Text = string.Format("{0} - {1}", m.InvestorCode, m.InvestorName), Value = m.InvestorID.ToString() });
            model.investorListItems = viewInvestor;
            var paymentMode = new List<SelectListItem>();

            if (LoggedInOrganizationID == 82)
            {
                paymentMode.Add(new SelectListItem() { Text = "Cash", Value = "101", Selected = true });
                paymentMode.Add(new SelectListItem() { Text = "Bank", Value = "102" });
                paymentMode.Add(new SelectListItem() { Text = "ProductLoan", Value = "103" });
            }
            else
            {
                paymentMode.Add(new SelectListItem() { Text = "Cash", Value = "101", Selected = true });
                paymentMode.Add(new SelectListItem() { Text = "Bank", Value = "102" });
            }


            model.paymentMode = paymentMode;
            var disType = new List<SelectListItem>();
            disType.Add(new SelectListItem() { Text = "Once at a time", Value = "1", Selected = true });
            model.disType = disType;


            var mempassBook = memberPassBookRegisterService.GetAll().Where(i => i.IsActive == true && i.OrgID == LoggedInOrganizationID).OrderBy(i => i.MemberPassBookNO);
            var viewmempassBook = mempassBook.Select(x => x).ToList().Select(x => new SelectListItem
            {
                Value = x.MemberPassBookRegisterID.ToString(),
                Text = string.Format("{0} ", x.MemberPassBookNO.ToString())
            });
            var mempassBookitems = new List<SelectListItem>();
            mempassBookitems.Add(new SelectListItem() { Text = "Please Select", Value = "0", Selected = true });
            mempassBookitems.AddRange(viewmempassBook);
            model.MemberPassBookNOListItems = mempassBookitems;
            ViewData["memberPass"] = viewmempassBook;


            var paymentFrequency = new List<SelectListItem>();
            paymentFrequency.Add(new SelectListItem() { Text = "Please Select", Value = "0", Selected = true });
            paymentFrequency.Add(new SelectListItem() { Text = "Weekly", Value = "W" });
            paymentFrequency.Add(new SelectListItem() { Text = "Monthly", Value = "M" });
            paymentFrequency.Add(new SelectListItem() { Text = "Fortnightly", Value = "F" });
            paymentFrequency.Add(new SelectListItem() { Text = "Daily", Value = "D" });
            model.freqMode = paymentFrequency;
        }

        public JsonResult GetCenterList()
        {
            var param1 = new { @EmpID = LoggedInEmployeeID };
            var LoanInstallMent = ultimateReportService.GetCenterROleWise(param1);
            List<CenterViewModel> List_CenterViewModel = new List<CenterViewModel>();
            DataSet div_items;
            var param = new { OfficeId = LoginUserOfficeID };

            if (LoanInstallMent != null)
            {
                var empType = LoanInstallMent.Tables[0].Rows[0]["Name"].ToString();
                if (empType == "FO")
                {
                    var paramFOWISE = new { OfficeId = LoginUserOfficeID, EmpID = LoggedInEmployeeID, empType = empType };
                    div_items = groupwiseReportService.GetDataDataseAccess(paramFOWISE, "GetOnlyCenterFOWISE");
                }
                else

                    div_items = groupwiseReportService.GetDataDataseAccess(param, "GetOnlyCenter");
            }
            else
            {
                param = new { OfficeId = LoginUserOfficeID };
                div_items = groupwiseReportService.GetDataDataseAccess(param, "GetOnlyCenter");
            }

            List_CenterViewModel = div_items.Tables[0].AsEnumerable()
            .Select(row => new CenterViewModel
            {
                CenterID = row.Field<int>("CenterID"),
                CenterCode = row.Field<string>("CenterCode"),
                CenterName = row.Field<string>("CenterName")
            }).ToList();

            var viewCenter = List_CenterViewModel.Select(x => x).ToList().Select(x => new SelectListItem
            {
                Value = x.CenterID.ToString(),
                Text = x.CenterCode.ToString() + ", " + x.CenterName.ToString()
            });
            var center_items = new List<SelectListItem>();
            if (viewCenter.ToList().Count > 0)
            {
                center_items.Add(new SelectListItem() { Text = "Select All", Value = "0", Selected = true });
            }
            center_items.AddRange(viewCenter);
            return Json(center_items, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetMemberPassBookList(string Member_id)
        {
            List<MemberPassBookRegisterViewModel> List_MemberPassBookRegisterViewModel = new List<MemberPassBookRegisterViewModel>();
            var param = new { OfficeId = LoginUserOfficeID, MemberId = Member_id };
            var div_items = ultimateReportService.GetMemberPasBookList(param);

            List_MemberPassBookRegisterViewModel = div_items.Tables[0].AsEnumerable()
            .Select(row => new MemberPassBookRegisterViewModel
            {
                MemberPassBookRegisterID = row.Field<long>("MemberPassBookRegisterID"),
                MemberPassBookNO = row.Field<long>("MemberPassBookNO")

            }).ToList();

            var viewProduct = List_MemberPassBookRegisterViewModel.Select(x => x).ToList().Select(x => new SelectListItem
            {
                Value = x.MemberPassBookRegisterID.ToString(),
                Text = x.MemberPassBookNO.ToString()
            });
            var d_items = new List<SelectListItem>();
            d_items.Add(new SelectListItem() { Text = "Please Select", Value = "0" });
            d_items.AddRange(viewProduct);
            return Json(d_items, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetProductListByFreq(string Freq_id)
        {
            List<ProductViewModel> List_MemberPassBookRegisterViewModel = new List<ProductViewModel>();
            var param = new { PaymentFrq = Freq_id, officeID = LoginUserOfficeID };
            var div_items = ultimateReportService.GetProductListByFrequencyModeAccoringTOOffice(param);
            var viewProduct = div_items.Tables[0].AsEnumerable().Select(row => new SelectListItem
            {
                Value = row.Field<Int16>("ProductID").ToString(),
                Text = row.Field<string>("ProductCode") + ' ' + row.Field<string>("ProductName")
            });
            var d_items = new List<SelectListItem>();
            d_items.Add(new SelectListItem() { Text = "Please Select", Value = "0" });
            d_items.AddRange(viewProduct);
            return Json(d_items, JsonRequestBehavior.AllowGet);
        }
        //khalid: Load Main Product in ddl
        public JsonResult GetMainProductList(string Freq_id)
        {
            List<ProductViewModel> List_MemberPassBookRegisterViewModel = new List<ProductViewModel>();
            var param = new { PaymentFrq = Freq_id, OfficeID = LoginUserOfficeID };
            var div_items = ultimateReportService.GetMainProductListAccordingToOffice(param);
            var viewProduct = div_items.Tables[0].AsEnumerable().Select(row => new SelectListItem
            {
                Value = row.Field<string>("mainproductcode").ToString(),
                Text = row.Field<string>("mainproductcode") + ' ' + row.Field<string>("mainitemname")
            });
            var d_items = new List<SelectListItem>();
            d_items.Add(new SelectListItem() { Text = "Please Select", Value = "0" });
            d_items.AddRange(viewProduct);
            return Json(d_items, JsonRequestBehavior.AllowGet);
        }
        //khalid: Load Main Product in ddl
        public JsonResult GetSubMainProductList(string MainProductCode, string freq)
        {
            List<ProductViewModel> List_MemberPassBookRegisterViewModel = new List<ProductViewModel>();
            var param = new { MainProductCode = MainProductCode, freq = freq };
            var div_items = ultimateReportService.GetSubMainProductList(param);
            var viewProduct = div_items.Tables[0].AsEnumerable().Select(row => new SelectListItem
            {
                Value = row.Field<string>("submaincategory").ToString(),
                Text = row.Field<string>("submaincategory").ToString() //+ ' ' + row.Field<string>("mainitemname")
            });
            var d_items = new List<SelectListItem>();
            d_items.Add(new SelectListItem() { Text = "Please Select", Value = "0" });
            d_items.AddRange(viewProduct);
            return Json(d_items, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetMainList(string MainProductCode, string freq)
        {
            List<ProductViewModel> List_MemberPassBookRegisterViewModel = new List<ProductViewModel>();
            var param = new { MainProductCode = MainProductCode, freq = freq };
            var div_items = ultimateReportService.GetProductList(param);
            var viewProduct = div_items.Tables[0].AsEnumerable().Select(row => new SelectListItem
            {
                Value = row.Field<string>("submaincategory").ToString(),
                Text = row.Field<string>("submaincategory").ToString() //+ ' ' + row.Field<string>("mainitemname")
            });
            var d_items = new List<SelectListItem>();
            d_items.Add(new SelectListItem() { Text = "Please Select", Value = "0" });
            d_items.AddRange(viewProduct);
            return Json(d_items, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetMainListT(string MainProductCode, string freq)
        {
            List<ProductViewModel> List_MemberPassBookRegisterViewModel = new List<ProductViewModel>();
            var param = new { MainProductCode = MainProductCode, freq = freq, OfficeID = LoginUserOfficeID };
            var div_items = ultimateReportService.GetProductListTAccordingToOffice(param);
            var viewProduct = div_items.Tables[0].AsEnumerable().Select(row => new SelectListItem
            {
                Value = row.Field<Int16>("ProductID").ToString(),
                Text = row.Field<string>("ProductCode") + ' ' + row.Field<string>("ProductName")
            });

            var d_items = new List<SelectListItem>();
            d_items.Add(new SelectListItem() { Text = "Please Select", Value = "0" });
            d_items.AddRange(viewProduct);
            return Json(d_items, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetProductListBySubCat(string SubMain, string freq, string MainProductCode)
        {
            List<ProductViewModel> List_MemberPassBookRegisterViewModel = new List<ProductViewModel>();
            var param = new { SubMain = SubMain, freq = freq, MainProductCode = MainProductCode, officeId = LoginUserOfficeID };
            var div_items = ultimateReportService.GetProductListBySubCatAccordingToOffice(param);
            var viewProduct = div_items.Tables[0].AsEnumerable().Select(row => new SelectListItem
            {
                Value = row.Field<Int16>("ProductID").ToString(),
                Text = row.Field<string>("ProductCode") + ' ' + row.Field<string>("ProductName")
            });
            var d_items = new List<SelectListItem>();
            d_items.Add(new SelectListItem() { Text = "Please Select", Value = "0" });
            d_items.AddRange(viewProduct);
            return Json(d_items, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetInstallment(int productid, decimal principal)
        {

            decimal vLoanInstallment = 0;
            decimal vInterestInstallment = 0;
            decimal vtotal = 0;
            int vDuration;
            var pbr = productService.GetById(productid);
            vtotal = Convert.ToDecimal((pbr.LoanInstallment * principal)) + Convert.ToDecimal((pbr.InterestInstallment * principal));


            vInterestInstallment = Math.Round(vtotal - Math.Round(Convert.ToDecimal((pbr.LoanInstallment * principal))));
            vLoanInstallment = Math.Round(vtotal - vInterestInstallment);
            vDuration = Convert.ToInt16(pbr.Duration);
            var result = new { loan = vLoanInstallment.ToString(), interest = vInterestInstallment.ToString(), duration = vDuration.ToString() };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetOrgID(string officeId)
        {
            int vLoanTerm;
            var model = new LoanApprovalViewModel();
            var result = new { LoanTerm = LoggedInOrganizationID.ToString() };
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetMaxLoanTerm(string officeId, string centerId, string MemId, string ProdId)
        {
            int vLoanTerm;
            var model = new LoanApprovalViewModel();
            model.OfficeID = Convert.ToInt16(SessionHelper.LoginUserOfficeID.Value);
            model.CenterID = Convert.ToInt32(centerId);
            model.MemberID = Convert.ToInt64(MemId);
            model.ProductID = Convert.ToInt16(ProdId);


            var entity = Mapper.Map<LoanApprovalViewModel, LoanSummary>(model);
            entity.OrgID = Convert.ToInt16(LoggedInOrganizationID);
            if (LoggedInOrganizationID == 54)
            {
                var param = new { OfficeID = LoginUserOfficeID, MemberID = entity.MemberID, ProductId = entity.ProductID };
                var div_items = ultimateReportService.GetMaxLoanTermMainCodeWise(param);
                vLoanTerm = Convert.ToInt16(div_items.Tables[0].Rows[0]["LoanTerm"].ToString());

            }
            else
            {
                var mlt = loanapprovalService.getMaxLoanterm(entity);
                vLoanTerm = Convert.ToInt16(mlt);
            }


            var result = new { LoanTerm = vLoanTerm.ToString() };
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetLoanDisbursementType(string officeId, string centerId, string MemId, string ProdId)
        {


            List<ProductViewModel> List_MemberPassBookRegisterViewModel = new List<ProductViewModel>();
            var param = new { ProductID = Convert.ToInt16(ProdId) };
            var div_items = ultimateReportService.GetLoanDisburseTypeList(param);
            var viewProduct = div_items.Tables[0].AsEnumerable().Select(row => new SelectListItem
            {
                Value = row.Field<string>("DisValue").ToString(),
                Text = row.Field<string>("Distype").ToString() //+ ' ' + row.Field<string>("mainitemname")
            });
            var d_items = new List<SelectListItem>();
            d_items.AddRange(viewProduct);
            return Json(d_items, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCoapplicantName(string officeId, string centerId, string MemId)
        {
            string vLoanTerm;
            var model = new LoanApprovalViewModel();
            model.OfficeID = Convert.ToInt16(SessionHelper.LoginUserOfficeID.Value);
            model.CenterID = Convert.ToInt32(centerId);
            model.MemberID = Convert.ToInt64(MemId);
            var mbr = memberService.GetByIdLong(Convert.ToInt64(MemId));
            vLoanTerm = (string.IsNullOrEmpty(mbr.CoApplicantName) ? "" : mbr.CoApplicantName);
            var result = new { LoanTerm = vLoanTerm.ToString() };
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetMaxLoanTermEdit(string officeId, string centerId, string MemId, string ProdId)
        {
            int vLoanTerm;
            var model = new LoanApprovalViewModel();
            model.OfficeID = Convert.ToInt16(SessionHelper.LoginUserOfficeID.Value);
            model.CenterID = Convert.ToInt32(centerId);
            model.MemberID = Convert.ToInt64(MemId);
            model.ProductID = Convert.ToInt16(ProdId);


            var entity = Mapper.Map<LoanApprovalViewModel, LoanSummary>(model);
            entity.OrgID = Convert.ToInt16(LoggedInOrganizationID);
            var mlt = loanapprovalService.getMaxLoantermEdit(entity);
            vLoanTerm = Convert.ToInt16(mlt);
            var result = new { LoanTerm = vLoanTerm.ToString() };
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetMemberList(string memberid, string centerId)
        {

            var MemberByCenterSessionKey = string.Format("MemberByCenterSessionKey_{0}", centerId);
            var memberList = new List<Member>();
            if (Session[MemberByCenterSessionKey] != null)
                memberList = Session[MemberByCenterSessionKey] as List<Member>;
            else
            {
                var mbr = memberService.GetByCenterId(int.Parse(centerId), Convert.ToInt16(LoginUserOfficeID), Convert.ToInt16(LoggedInOrganizationID)).ToList();
                Session[MemberByCenterSessionKey] = mbr;
                memberList = mbr;
            }
            var members = memberList.Where(m => string.Format("{0} - {1}", m.MemberCode, (string.IsNullOrEmpty(m.FirstName) ? "" : m.FirstName) + ' ' + (string.IsNullOrEmpty(m.MiddleName) ? "" : m.MiddleName) + ' ' + (string.IsNullOrEmpty(m.LastName) ? "" : m.LastName) + ' ' + (string.IsNullOrEmpty(m.RefereeName) ? "" : m.RefereeName)).ToLower().Contains(memberid.ToLower())).Select(m1 => new { m1.MemberID, MemberName = string.Format("{0} - {1}", m1.MemberCode, (string.IsNullOrEmpty(m1.FirstName) ? "" : m1.FirstName) + ' ' + (string.IsNullOrEmpty(m1.MiddleName) ? "" : m1.MiddleName) + ' ' + (string.IsNullOrEmpty(m1.LastName) ? "" : m1.LastName) + ' ' + (string.IsNullOrEmpty(m1.RefereeName) ? "" : m1.RefereeName)) }).ToList();
            return Json(members, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetPuposeList(string purposeid)
        {
            var purposeList = new List<Purpose>();
            var mbr = purposeService.GetAll().ToList();
            purposeList = mbr;
            var members = purposeList.Where(m => string.Format("{0} - {1}", m.PurposeCode, (string.IsNullOrEmpty(m.PurposeName) ? "" : m.PurposeName)).ToLower().Contains(purposeid.ToLower())).Select(m1 => new { m1.PurposeID, PurposeName = string.Format("{0} - {1}", m1.PurposeCode, (string.IsNullOrEmpty(m1.PurposeName) ? "" : m1.PurposeName)) }).ToList();
            return Json(members, JsonRequestBehavior.AllowGet);
        }
        public Member GetMember(long memberid)
        {
            var mbr = memberService.GetByMemberId(memberid);
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
        public Purpose GetPurpose(int purposeid)
        {
            var pbr = purposeService.GetById(purposeid);
            return pbr;
        }
        public MemberPassBookRegister GetMemPassBook(long mPassid)
        {
            var mps = memberPassBookRegisterService.GetByIdLong(mPassid);
            return mps;
        }
        public MemberCategory GetCategory(long mCategoryID)
        {
            var mcs = membercategoryService.GetByIdLong(mCategoryID);
            return mcs;
        }
        #endregion

        #region Events

        public ActionResult Index()
        {

            return View();

        }

        public ActionResult PortalLoanProposalIndex()
        {
            return View();
        }
        public ActionResult GetApprovePortalImageFromDatabase(long? Id)
        {
            try
            {
                var member = memberService.GetByIdLong((long)Id);
                return View();


            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public ActionResult ApprovePortalLoanRequest(int portalLoanId)
        {
            Guid id = Guid.NewGuid();
            var MyGuid = id.ToString();

            ViewData["MyGuid"] = MyGuid;

            var OrgId = (int)LoggedInOrganizationID;
            ViewData["OrgId"] = OrgId;

            var portalLoanSummary = portalLoanSummaService.GetById(portalLoanId);
            var model = Mapper.Map<PortalLoanSummary, LoanApprovalViewModel>(portalLoanSummary);

            var purposeSummary = portalLoanSummaService.GetById(portalLoanId);
            var purposeModel = Mapper.Map<PortalLoanSummary, LoanApprovalViewModel>(purposeSummary);
            model.PurposeID= purposeModel.PurposeID;

            var product = productService.GetById(model.ProductID);

            model.frequencyMode = product.PaymentFrequency;
            model.MainProductCode= product.MainProductCode;
            //var model = new LoanApprovalViewModel();
            if (IsDayInitiated)
                model.ApproveDate = TransactionDate;
            MapDropDownList(model);


            var OrgInfo = organizationService.GetById((int)SessionHelper.LoginUserOrganizationID);
            //var v = OrgInfo.MemberAge;
            var GuarantorAge = 60;

            if (OrgInfo.GuarantorAge == null)
            {

            }
            else
            {
                GuarantorAge = (int)OrgInfo.GuarantorAge;
            }
            ViewData["GuarantorAge"] = GuarantorAge;

            // for Member Name
            var member = GetMember(Convert.ToInt64(model.MemberID));
            ViewBag.MemberName = string.Format("{0} - {1}", member.MemberCode, member.FirstName);

            var entity = Mapper.Map<LoanApprovalViewModel, LoanSummary>(model);
            entity.OrgID = Convert.ToInt16(LoggedInOrganizationID);
            
            // for purpose 
            var purpose = GetPurpose(Convert.ToInt32(model.PurposeID));
            ViewBag.purposeList = string.Format("{0} - {1}", purpose.PurposeCode, purpose.PurposeName);

            var mlt = loanapprovalService.getMaxLoanterm(entity);

            model.LoanTerm = (byte)mlt;


            return View("Create", model);

        }
        // GET: LoanApproval/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }
        // GET: LoanApproval/Create
        public ActionResult Create()
        {
            Guid id = Guid.NewGuid();
            var MyGuid = id.ToString();

            ViewData["MyGuid"] = MyGuid;

            var OrgId = (int)LoggedInOrganizationID;
            ViewData["OrgId"] = OrgId;

            var model = new LoanApprovalViewModel();
            if (IsDayInitiated)
                model.ApproveDate = TransactionDate;
            MapDropDownList(model);


            var OrgInfo = organizationService.GetById((int)SessionHelper.LoginUserOrganizationID);
            //var v = OrgInfo.MemberAge;
            var GuarantorAge = 60;

            if (OrgInfo.GuarantorAge == null)
            {

            }
            else
            {
                GuarantorAge = (int)OrgInfo.GuarantorAge;
            }
            ViewData["GuarantorAge"] = GuarantorAge;
            return View(model);
        }
        // POST: LoanApproval/Create
        [HttpPost]
        public ActionResult Create(LoanApprovalViewModel model, FormCollection form)
        {
            try
            {
                //if (!IsDayInitiated)
                //{
                //    return GetErrorMessageResult("Please run the start work process");
                //}
                if (LoggedInOrganizationID == 126)
                {
                    int roleID = SessionHelper.LoginUserRoleId;
                    var approveCellingInfo = ApproveCellingService.GetApproveCellingbyroleAndproductId(roleID, model.ProductID);

                    if (approveCellingInfo.MinRange > model.ApprovedAmount || approveCellingInfo.MaxRange < model.ApprovedAmount)
                        return GetErrorMessageResult($"The given Approved Ammount must be in {approveCellingInfo.MinRange} to {approveCellingInfo.MaxRange}");
                }

                if (model.txtMaleFullTimeP1 == null)
                {
                    model.txtMaleFullTimeP1 = 0;
                }
                if (model.txtFeMaleFullTimeP1 == null)
                {
                    model.txtFeMaleFullTimeP1 = 0;
                }
                if (model.txtMalePartTimeP1 == null)
                {
                    model.txtMalePartTimeP1 = 0;
                }
                if (model.txtFeMalePartTimeP1 == null)
                {
                    model.txtFeMalePartTimeP1 = 0;
                }
                if (model.txtMaleFullTimeP2 == null)
                {
                    model.txtMaleFullTimeP2 = 0;
                }
                if (model.txtFeMaleFullTimeP2 == null)
                {
                    model.txtFeMaleFullTimeP2 = 0;
                }
                if (model.txtMalePartTimeP2 == null)
                {
                    model.txtMalePartTimeP2 = 0;
                }
                if (model.txtFeMalePartTimeP2 == null)
                {
                    model.txtFeMalePartTimeP2 = 0;
                }
                if (model.txtMaleFullTimeP3 == null)
                {
                    model.txtMaleFullTimeP3 = 0;
                }
                if (model.txtFeMaleFullTimeP3 == null)
                {
                    model.txtFeMaleFullTimeP3 = 0;
                }
                if (model.txtMalePartTimeP3 == null)
                {
                    model.txtMalePartTimeP3 = 0;
                }
                if (model.txtFeMalePartTimeP3 == null)
                {
                    model.txtFeMalePartTimeP3 = 0;
                }



                var entity = Mapper.Map<LoanApprovalViewModel, LoanSummary>(model);
                var member = GetMember(Convert.ToInt64(entity.MemberID));
                int membercaregoryid = member.MemberCategoryID;
                var mCateCode = membercategoryService.GetAll().Where(r => r.MemberCategoryID == membercaregoryid).FirstOrDefault();
                var mCategoryCode = mCateCode.MemberCategoryCode;

                var mProdCode = productService.GetAll().Where(p => p.ProductID == entity.ProductID).FirstOrDefault();
                var mProductCode = mProdCode.MainProductCode;
                if (LoggedInOrganizationID != 54)
                {
                    if (LoggedInOrganizationID == 11)
                    {
                        if (mProductCode == "02.00" || mProductCode == "03.00")
                        {
                            if (mCategoryCode != mProductCode)
                            {
                                return GetErrorMessageResult("Pls. Check Category");
                            }
                        }
                    }
                    else
                    {
                        if (mProductCode == "01.00")
                        {
                            if (mCategoryCode != mProductCode)
                            {
                                return GetErrorMessageResult("Pls. Check Category");
                            }
                        }

                        if (mProductCode == "02.00")
                        {
                            if (mCategoryCode != mProductCode)
                            {
                                return GetErrorMessageResult("Pls. Check Category");
                            }
                        }
                        if (mProductCode == "03.00")
                        {
                            if (mCategoryCode != mProductCode)
                            {
                                return GetErrorMessageResult("Pls. Check Category");
                            }
                        }
                    }
                }
                var employee = GetEmployee(entity.CenterID);
                int employeeid = employee.EmployeeId;
                if (LoggedInOrganizationID == 5)
                {
                    if (model.Guarantor == "")
                    {
                        return GetErrorMessageResult("Co-applicant NID/Smart Card is not Given.");
                    }
                }
                if (employeeid == 0)
                {
                    return GetErrorMessageResult("Pls.Check Employee in Center");
                }
                var prod = GetProduct(entity.ProductID);
                decimal intrate = Convert.ToDecimal(prod.InterestRate);
                decimal vMinLimit = Convert.ToDecimal(prod.MinLimit);
                decimal vMaxLimit = Convert.ToDecimal(prod.MaxLimit);
                if (entity.PrincipalLoan > vMaxLimit || entity.PrincipalLoan < vMinLimit)
                {
                    return GetErrorMessageResult();
                }

                if (prod.Duration != entity.Duration)
                {
                    return GetErrorMessageResult("Pls. Check Duartion");
                }

                var vtotal = Math.Round(Convert.ToDecimal((prod.LoanInstallment * Convert.ToDecimal(entity.PrincipalLoan))) + Convert.ToDecimal((prod.InterestInstallment * Convert.ToDecimal(entity.PrincipalLoan))));
                var entityTotal = Math.Round(Convert.ToDecimal(entity.LoanInstallment) + Convert.ToDecimal(entity.IntInstallment));

                if (Convert.ToDecimal(vtotal) != Convert.ToDecimal(entityTotal))
                {

                    return GetErrorMessageResult("Pls. Check Installment");
                }

                entity.EmployeeId = Convert.ToInt16(employeeid);
                entity.MemberCategoryID = Convert.ToByte(membercaregoryid);
                entity.InterestRate = intrate;
                entity.LoanStatus = 1;
                entity.LoanNo = Convert.ToString(entity.MemberID);
                entity.IsActive = true;
                entity.OrgID = Convert.ToInt16(LoggedInOrganizationID);
                entity.IsApproved = false;
                if (entity.DisbursementType == null)
                {
                    return GetErrorMessageResult("Please put the DisbursementType");
                }
                if (entity.DisbursementType == 2)
                {
                    entity.TransType = entity.TransType;
                }
                else
                {
                    entity.TransType = entity.TransType;
                }

                if (entity.OrgID == 4)
                {
                    if (entity.PrincipalLoan > 99000)
                    {
                        if (entity.SecurityBankName == null || entity.SecurityBankName == "")
                        {
                            return GetErrorMessageResult("Please put the Bank Name");
                        }
                        if (entity.SecurityBankBranchName == null || entity.SecurityBankBranchName == "")
                        {
                            return GetErrorMessageResult("Please put the Branch  Name");
                        }
                        if (entity.SecurityBankCheckNo == null || entity.SecurityBankCheckNo == "")
                        {
                            return GetErrorMessageResult("Please put the Check  No.");
                        }
                    }
                }
                if (entity.OrgID == 5)
                {
                    if (entity.Guarantor == null || entity.Guarantor == "")
                    {
                        return GetErrorMessageResult("Please put the CoApplicant NID/SmartCard");
                    }
                }

                var errors = new { OfficeID = LoginUserOfficeID, MemberID = entity.MemberID, CenterID = entity.CenterID, MainProductCode = mProductCode, ProductId = entity.ProductID, PrincipalLoan = entity.PrincipalLoan, Duration = entity.Duration };
                var div_items = ultimateReportService.validateLOanProposal(errors);
                if (div_items.Tables[0].Rows.Count > 0)
                {
                    string vErr = div_items.Tables[0].Rows[0]["ErrorCode"].ToString();
                    if (vErr == "1")
                    {
                        if (entity.PurposeID == 0)
                        {
                            return GetErrorMessageResult("Please put the Purpose");
                        }

                        if (entity.Duration == 0)
                        {
                            return GetErrorMessageResult("Please put the Duration");
                        }

                        entity.ApprovedAmount = model.ApprovedAmount;
                        if (LoggedInOrganizationID == 54 || LoggedInOrganizationID == 6)
                        {
                            var LoanAccount = new { Qtype = 1, OfficeID = LoginUserOfficeID, MemberID = entity.MemberID, ProductID = entity.ProductID, loanTerm = entity.LoanTerm };
                            var Loan_items = ultimateReportService.GenerateLoanAndSavingAccount(LoanAccount);
                            if (Loan_items.Tables[0].Rows.Count > 0)
                            {
                                entity.LoanAccountNo = Loan_items.Tables[0].Rows[0][0].ToString();
                            }
                        }
                        LoanSummary ls = new LoanSummary();
                        ls = loansSummaryService.Create(entity);
                        var lsID = ls.LoanSummaryID;
                        var MyGuid = model.MyGuid;
                        var param = new
                        {
                            @lsID = lsID,
                            @MyGuid = MyGuid
                        };
                        var empList = employeeSPService.GetDataWithParameter(param, "SP_SetLoanSummaryId");
                        if (model.txtMaleFullTimeP1 != null && model.txtMaleFullTimeP1 != 0)// Changed for if Data is NULL.
                        {
                            var paramEmploymentInfo = new
                            {
                                LoanSummaryID = ls.LoanSummaryID,
                                OfficeId = LoginUserOfficeID,
                                MemberId = entity.MemberID,
                                CenterId = entity.CenterID,
                                txtMaleFullTimeP1 = model.txtMaleFullTimeP1,
                                txtFeMaleFullTimeP1 = model.txtFeMaleFullTimeP1,
                                txtMalePartTimeP1 = model.txtMalePartTimeP1,
                                txtFeMalePartTimeP1 = model.txtFeMalePartTimeP1,
                                txtMaleFullTimeP2 = model.txtMaleFullTimeP2,
                                txtFeMaleFullTimeP2 = model.txtFeMaleFullTimeP2,
                                txtMalePartTimeP2 = model.txtMalePartTimeP2,
                                txtFeMalePartTimeP2 = model.txtFeMalePartTimeP2,
                                txtMaleFullTimeP3 = model.txtMaleFullTimeP3,
                                txtFeMaleFullTimeP3 = model.txtFeMaleFullTimeP3,
                                txtMalePartTimeP3 = model.txtMalePartTimeP3,
                                txtFeMalePartTimeP3 = model.txtFeMalePartTimeP3,
                                isActive = 1

                            };

                            var empListEmploymentInfo = employeeSPService.GetDataWithParameter(paramEmploymentInfo, "SetEmploymentDetails");
                        }
                        if(model.PortalLoanSummaryID != null)
                        {
                            var portalLoanSummary = portalLoanSummaService.GetById((int)model.PortalLoanSummaryID);
                            if(portalLoanSummary != null)
                            {
                                portalLoanSummary.ApprovalStatus = true;
                                portalLoanSummary.LoanStatus = 2;
                                portalLoanSummaService.Update(portalLoanSummary);
                            }

                            // Redirect To Action
                            return RedirectToAction("Index", "LoanApproval");
                        }
                        return GetSuccessMessageResult();
                    }
                    else
                    {
                        return GetErrorMessageResult(div_items.Tables[0].Rows[0]["ErrorName"].ToString());
                    }
                }
                else
                    return GetErrorMessageResult();
            }

            catch (Exception ex)
            {
                return GetErrorMessageResult(ex);
            }
            //catch (DbEntityValidationException e)
            //{
            //    foreach (var eve in e.EntityValidationErrors)
            //    {
            //        Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
            //            eve.Entry.Entity.GetType().Name, eve.Entry.State);
            //        foreach (var ve in eve.ValidationErrors)
            //        {
            //            Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
            //                ve.PropertyName, ve.ErrorMessage);
            //        }
            //    }
            //    throw;
            //return GetErrorMessageResult(ex);
            //}
        }
        // GET: LoanApproval/Edit/5
        public ActionResult Edit(int id)
        {
            var loanapproval = loanapprovalService.GetById(id);
            var member = GetMember(Convert.ToInt64(loanapproval.MemberID));
            var mPassBook = GetMemPassBook(Convert.ToInt64(loanapproval.MemberPassBookRegisterID));
            var entity = Mapper.Map<LoanSummary, LoanApprovalViewModel>(loanapproval);
            ViewBag.MemberName = string.Format("{0} - {1}", member.MemberCode, member.FirstName);
            //ViewBag.memberPass = string.Format("{0}", (string.IsNullOrEmpty(mPassBook.MemberPassBookNO) ? "0" : mPassBook.MemberPassBookNO));
            var purpose = GetPurpose(Convert.ToInt32(loanapproval.PurposeID));
            ViewBag.purposeList = string.Format("{0} - {1}", purpose.PurposeCode, purpose.PurposeName);
            var prod = GetProduct(loanapproval.ProductID);
            ViewBag.frequencyMode = prod.PaymentFrequency;
            entity.frequencyMode = prod.PaymentFrequency;
            MapDropDownList(entity);
            return View(entity);
        }
        // POST: LoanApproval/Edit/5
        [HttpPost]
        public ActionResult Edit(LoanApprovalViewModel model)
        {
            try
            {
                if (!IsDayInitiated)
                {
                    return GetErrorMessageResult("Please run the start work process");
                }

                var entity = Mapper.Map<LoanApprovalViewModel, LoanSummary>(model);

                var getLoanSummary = loanapprovalService.GetByIdLong(Convert.ToInt64(model.LoanSummaryID));

                string msg = "";
                var member = GetMember(Convert.ToInt64(entity.MemberID));
                int membercaregoryid = member.MemberCategoryID;

                var employee = GetEmployee(entity.CenterID);
                int employeeid = employee.EmployeeId;

                var prod = GetProduct(entity.ProductID);
                decimal intrate = Convert.ToDecimal(prod.InterestRate);

                entity.EmployeeId = Convert.ToInt16(employeeid);
                entity.MemberCategoryID = Convert.ToByte(membercaregoryid);
                entity.InterestRate = intrate;
                var mCateCode = membercategoryService.GetAll().Where(r => r.MemberCategoryID == membercaregoryid).FirstOrDefault();
                var mCategoryCode = mCateCode.MemberCategoryCode;

                var mProdCode = productService.GetAll().Where(p => p.ProductID == entity.ProductID).FirstOrDefault();
                var mProductCode = mProdCode.MainProductCode;
                if (LoggedInOrganizationID != 54)
                {
                    if (LoggedInOrganizationID == 11)
                    {
                        if (mProductCode == "02.00" || mProductCode == "03.00")
                        {
                            if (mCategoryCode != mProductCode)
                            {
                                return GetErrorMessageResult("Pls. Check Category");
                            }
                        }
                    }
                    else
                    {
                        if (mProductCode == "01.00" || mProductCode == "02.00" || mProductCode == "03.00")
                        {
                            if (mCategoryCode != mProductCode)
                            {
                                return GetErrorMessageResult("Pls. Check Category");
                            }
                        }
                    }
                }

                decimal vMinLimit = Convert.ToDecimal(prod.MinLimit);
                decimal vMaxLimit = Convert.ToDecimal(prod.MaxLimit);
                if (entity.PrincipalLoan > vMaxLimit || entity.PrincipalLoan < vMinLimit)
                {
                    return GetErrorMessageResult();
                }

                var vtotal = Math.Round(Convert.ToDecimal((prod.LoanInstallment * Convert.ToDecimal(entity.PrincipalLoan))) + Convert.ToDecimal((prod.InterestInstallment * Convert.ToDecimal(entity.PrincipalLoan))));
                var entityTotal = Math.Round(Convert.ToDecimal(entity.LoanInstallment) + Convert.ToDecimal(entity.IntInstallment));

                if (Convert.ToDecimal(vtotal) != Convert.ToDecimal(entityTotal))
                {
                    return GetErrorMessageResult("Pls. Check Installment");
                }

                getLoanSummary.PrincipalLoan = entity.PrincipalLoan;
                getLoanSummary.Duration = entity.Duration;
                getLoanSummary.LoanInstallment = entity.LoanInstallment;
                getLoanSummary.IntInstallment = entity.IntInstallment;
                getLoanSummary.ProductID = entity.ProductID;
                getLoanSummary.PurposeID = entity.PurposeID;
                // getLoanSummary.MemberID = entity.MemberID;
                getLoanSummary.EmployeeId = entity.EmployeeId;
                getLoanSummary.MemberCategoryID = entity.MemberCategoryID;
                if (entity.DisbursementType == null)
                {
                    return GetErrorMessageResult("Please put the DisbursementType");
                }

                if (entity.DisbursementType == 2)
                {
                    entity.TransType = 101;
                }
                else
                {
                    getLoanSummary.TransType = entity.TransType;
                }

                getLoanSummary.ApproveDate = TransactionDate;
                getLoanSummary.OrgID = Convert.ToInt16(LoggedInOrganizationID);
                getLoanSummary.IsActive = true;
                if (getLoanSummary.OrgID == 4)
                {
                    if (getLoanSummary.PrincipalLoan > 50000)
                    {
                        if (getLoanSummary.SecurityBankName == null || getLoanSummary.SecurityBankName == "")
                        {
                            return GetErrorMessageResult("Please put the Bank Name");
                        }
                        if (getLoanSummary.SecurityBankBranchName == null || getLoanSummary.SecurityBankBranchName == "")
                        {
                            return GetErrorMessageResult("Please put the Branch  Name");
                        }
                        if (getLoanSummary.SecurityBankCheckNo == null || getLoanSummary.SecurityBankCheckNo == "")
                        {
                            return GetErrorMessageResult("Please put the Check  No.");
                        }
                    }

                }

                if (ModelState.IsValid)
                {
                    var errors = new { OfficeID = LoginUserOfficeID, MemberID = entity.MemberID, CenterID = entity.CenterID, MainProductCode = mProductCode, ProductId = entity.ProductID, PrincipalLoan = entity.PrincipalLoan, Duration = entity.Duration };
                    var div_items = ultimateReportService.validateLOanProposal(errors);
                    if (div_items.Tables[0].Rows.Count > 0)
                    {
                        string vErr = div_items.Tables[0].Rows[0]["ErrorCode"].ToString();
                        if (vErr == "1")
                        {
                            if (model.PurposeID == 0)
                            {
                                return GetErrorMessageResult("Please put the Purpose");
                            }
                            if (entity.Duration == 0)
                            {
                                return GetErrorMessageResult("Please put the Duration");
                            }
                            getLoanSummary.CoApplicantName = entity.CoApplicantName;
                            getLoanSummary.Guarantor = entity.Guarantor;
                            getLoanSummary.MemberPassBookRegisterID = entity.MemberPassBookRegisterID;
                            loanapprovalService.Update(getLoanSummary);
                            return GetSuccessMessageResult();
                        }
                        else
                        {
                            return GetErrorMessageResult(div_items.Tables[0].Rows[0]["ErrorName"].ToString());
                        }
                    }
                    else
                        return GetErrorMessageResult();
                }
                return GetErrorMessageResult();
            }
            catch (Exception ex)
            {
                return GetErrorMessageResult(ex);
            }
        }
        // GET: LoanApproval/Delete/5
        public ActionResult Delete(int id)
        {
            loanapprovalService.Inactivate(id, null);
            return RedirectToAction("Index");
        }
        // POST: LoanApproval/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                loanapprovalService.Inactivate(id, null);
                // TODO: Add delete logic here
                // UpdateMethod(id, System.DateTime.Now);
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
        public JsonResult LoadNEWGUID()
        {
            Guid id = Guid.NewGuid();
            var MyGuid = id.ToString();

            var model = new LoanApprovalViewModel();

            List<LoanApprovalViewModel> List_ViewModel = new List<LoanApprovalViewModel>();

            LoanApprovalViewModel LoanApprovalInfo = new LoanApprovalViewModel();

            LoanApprovalInfo.MyGuid = MyGuid;

            List_ViewModel.Add(LoanApprovalInfo);


            return Json(List_ViewModel.ToList(), JsonRequestBehavior.AllowGet);
        }




        public JsonResult SetGuarantorInfo(
           string GuarantorId,
           string MemberId,
           string GuarantorName = "",
           string FatherName = "",
           string Relation = "",
           string DateOfBirth = "",
           string AgeDetails = "",
           string Address = "",
           string MyGuid = "",
            string LoanSummaryID = ""

           )
        {
            string result = "OK";
            try
            {
                var param = new
                {
                    @GuarantorId = GuarantorId,
                    @MemberId = MemberId,
                    @GuarantorName = GuarantorName,
                    @FatherName = FatherName,
                    @Relation = Relation,
                    @DateOfBirth = DateOfBirth,
                    @AgeDetails = AgeDetails,
                    @Address = Address,
                    @MyGuid = MyGuid,
                    @LoanSummaryID = LoanSummaryID

                };
                //var empList = ultimateReportService.GetDataWithParameter(param, "SP_SetGuarantorInfo");
                var empList = employeeSPService.GetDataWithParameter(param, "SP_SetGuarantorInfo");

            }
            catch (Exception ex)
            {
                Response.StatusCode = 403;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteGuarantorInfo(
          string GuarantorId
          )
        {
            string result = "OK";
            try
            {
                var param = new
                {
                    @GuarantorId = GuarantorId

                };
                //var empList = ultimateReportService.GetDataWithParameter(param, "SP_SetGuarantorInfo");
                var empList = employeeSPService.GetDataWithParameter(param, "SP_DeleteGuarantorInfo");

            }
            catch (Exception ex)
            {
                Response.StatusCode = 403;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetGuarantorList(string MemberId, string GuarantorId, string LoanSummaryId, int jtStartIndex, int jtPageSize, string jtSorting, string filterColumn, string filterValue)
        {
            try
            {
                StringBuilder sb = new StringBuilder();

                if (GuarantorId != null) //"0"
                {
                    if (GuarantorId != "0")
                        sb.Append(" AND g.GuarantorId =" + GuarantorId);

                }
                if (MemberId != null) //"0"
                {
                    if (MemberId.Length > 2)
                    {
                        sb.Append(" AND g.MemberId =" + MemberId);

                    }
                }


                if (LoanSummaryId != null) //"0"
                {
                    if (LoanSummaryId != "0")
                        sb.Append(" AND g.LoanSummaryID = " + LoanSummaryId);

                }

                List<GurantorViewModel> List_ViewModel = new List<GurantorViewModel>();
                var param = new { AndCondition = sb.ToString() };
                //  var empList = ultimateReportService.GetDataWithParameter(param, "SP_Get_Guarantor_List");
                var empList = employeeSPService.GetDataWithParameter(param, "SP_Get_Guarantor_List");

                List_ViewModel = empList.Tables[0].AsEnumerable()
                .Select(row => new GurantorViewModel
                {
                    rowSl = row.Field<Int64>("rowSl"),
                    GuarantorId = row.Field<Int64>("GuarantorId"),
                    MemberId = row.Field<Int64>("MemberId"),
                    GuarantorName = row.Field<string>("GuarantorName"),
                    FatherName = row.Field<string>("FatherName"),
                    Relation = row.Field<string>("Relation"),
                    DateOfBirth = row.Field<string>("DateOfBirth"),
                    AgeDetails = row.Field<string>("AgeDetails"),
                    Address = row.Field<string>("Address"),


                }).ToList();

                if (GuarantorId != null && GuarantorId != "0")
                {
                    return Json(List_ViewModel.ToList(), JsonRequestBehavior.AllowGet);
                }

                //var currentPageRecords = List_ViewModel.Skip(jtStartIndex).Take(jtPageSize);

                //return Json(new { Result = "OK", Records = currentPageRecords, TotalRecordCount = List_ViewModel.LongCount(), JsonRequestBehavior.AllowGet });


                var detail = List_ViewModel.ToList();
                var TotCount = detail.Count();
                var currentPageRecords = detail.ToList();
                return Json(new { Result = "OK", Records = currentPageRecords, TotalRecordCount = TotCount });

            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }

        }// End Function




        #endregion
    }
}
