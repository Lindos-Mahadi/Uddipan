using Android.App;
using Android.Graphics;
using Android.OS;
using Android.Widget;
using GBPMS.Droid.Classes;
using PMS.Droid.Classes;
using PMS.Droid.Classes.OffLineHelpers;
using PMS.Droid.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PMS.Droid
{
    [Activity(Label = "Download Master Data", Theme = "@style/Theme.DesignDemo", NoHistory = true, ScreenOrientation =Android.Content.PM.ScreenOrientation.Portrait)]
    public class SyncMasterDataActivity : BaseActivity
    {
        private List<LookupItem> lstOffices = null;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.SyncMasterData);
            GenerateNavigationMenu();
            try
            {
                var btnLogin = FindViewById<Button>(Resource.Id.btnSyncOfflineDataWithCredentials);
                var nc = new NetworkConnection(this);
                if (!nc.CheckNetworkConnection())
                {
                    GeatMessage("You do not have active internet connection. Download will not work.", true);
                    // return;
                }
                var waringMsg = "";
                var syncCollectionHelper = new LoanCollectionOfflineHelper(this);
                var collectionCount = syncCollectionHelper.GetCollectionRecordCount();               
                if (collectionCount > 0)
                    waringMsg = string.Format("WARNING: There are {0} collection records that will be deleted. ", collectionCount);

                var syncwithdrawalHelper = new WithdrawalOfflineHelper(this);
                var withdrawalCount = syncwithdrawalHelper.GetWithdrawalRecordCount();
                if (withdrawalCount > 0)
                    waringMsg = string.Format("WARNING: There are {0} withdrawal records that will be deleted. ", withdrawalCount);
                if (collectionCount > 0 && withdrawalCount > 0)
                    waringMsg = string.Format("WARNING: There are {0} collections and {1} withdrawal records that will be deleted. ", collectionCount, withdrawalCount);
                if (!string.IsNullOrEmpty(waringMsg))
                {
                    var txtStatus = FindViewById<TextView>(Resource.Id.lblSyncDataStatus);
                    txtStatus.Text = waringMsg;
                    txtStatus.SetTextColor(Color.Red);
                }
                btnLogin.Click += async delegate
                {

                    try
                    {
                        nc = new NetworkConnection(this);
                        if (!nc.CheckNetworkConnection())
                        {
                            GeatMessage("You do not have active internet connection. Download will not work.", false);
                            // CommonHelper.Done(this);
                            return;
                        }
                        var txtUser = FindViewById<EditText>(Resource.Id.txtUser);
                        var txtPwd = FindViewById<EditText>(Resource.Id.txtPass);
                        var txtStatus = FindViewById<TextView>(Resource.Id.lblSyncDataStatus);
                        txtStatus.SetTextColor(Color.Black);
                        txtStatus.Text = "";
                        var isValid = RequiredFieldValidation(txtUser, "LoginID required");
                        isValid = RequiredFieldValidation(txtPwd, "Password is required");
                        if (isValid)
                        {
                            var url = SharedMethods.GetOrganizationUrl(this);
                            if (string.IsNullOrEmpty(url))
                            {
                                GeatMessage("Organization url is not setup.", false);
                                //CommonHelper.Done(this);
                                return;
                            }
                            CommonHelper.Busy(this);
                            txtStatus.Text = "Checking API version.";
                            var apiVersion = await LoginHelper.GetAPIVersion(url, OfflineDBConstants.APP_VERSION, txtUser.Text);
                            string[] infos = apiVersion.Split('#');
                            //if(infos.Length < 2)
                            //{
                            //    if (infos.Length == 1 && Int32.Parse(infos[0]) != OfflineDBConstants.APP_VERSION)
                            //    {
                            //        GeatMessage("You are using an older version of APP. Please upgrade your APP from playstore.", false);
                            //        CommonHelper.Done(this);
                            //        return;
                            //    }
                                
                            //}
                            
                            //if (OfflineDBConstants.APP_VERSION < Int32.Parse(infos[1]))
                            //{
                            //    GeatMessage("You are using an older version of APP. Please upgrade your APP from playstore.", false);
                            //    CommonHelper.Done(this);
                            //    return;
                            //}
                            var result = await LoginHelper.IsUserAuthenticated(txtUser.Text.Trim(), txtPwd.Text, url);
                            if (result)
                            {

                                Android.Content.ISharedPreferences prefs = Android.Preferences.PreferenceManager.GetDefaultSharedPreferences(Android.App.Application.Context);
                                Android.Content.ISharedPreferencesEditor editor = prefs.Edit();
                                editor.PutString("uploadUrl", infos[0]);
                                editor.Apply();

                                SessionHelper.LoggedInUserID = txtUser.Text.Trim();
                                //Delete existing collection data data...
                                var collectionHelper = new LoanCollectionOfflineHelper(this);
                                collectionHelper.DeleteAll();
                                //delete existing withdrawal data.
                                var withdrawalHelper = new WithdrawalOfflineHelper(this);
                                withdrawalHelper.DeleteAll();
                                txtStatus.Text = "Downloading User....";
                                SyncUser();
                                txtStatus.Text = "Downloading Office....";
                                await SyncOfficeData(url);
                                txtStatus.Text = "Downloading Center and others....";
                                await SyncAllData(url);
                                //txtStatus.Text = "Syncing Member data....";
                                //await SyncMemberData(url);
                                //txtStatus.Text = "Syncing Member Product....";
                                //await SyncMemberProductData(url);
                                //txtStatus.Text = "Syncing Purpose....";
                                await SyncPurposeData(url);
                                txtStatus.Text = "Downloading Product....";
                                await SyncProductData(url);
                                txtStatus.Text = "Downloading Completed.";
                                GeatMessage("Downloading Completed", true);
                                StartActivity(typeof(UserHomeActivity));
                            }
                            else
                            {
                                GeatMessage("Invalid Credentials", false);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        LogSystemError(UIItems.SyncMasterData + " - Save", ex.Message, "");
                        GeatMessage("Error occured on Downloading: Please contact with administrator.", false);
                    }
                    CommonHelper.Done(this);
                };
            }
            catch (Exception ex)
            {
                LogSystemError(UIItems.SyncMasterData + " - Onload", ex.Message, "");
                GeatMessage("Error occured.", false);
            }
        }

        private void SyncUser()
        {
            try
            {
                var syncHelper = new UserOfflineHelper(this);
                syncHelper.DeleteAll();

                var password = FindViewById<EditText>(Resource.Id.txtPass).Text;
                syncHelper.AddNew(SessionHelper.LoggedInUserID, password, SessionHelper.LoggedInUserID, "");
            }
            catch (Exception ex)
            {
                GeatMessage("User cannot be Downloaded. Please contact with administrator.", true);
            }
        }
        private async Task SyncOfficeData(string url)
        {
            try
            {

                var lst = await LoginHelper.GetEmployeeOfficeListListAsync(SessionHelper.LoggedInUserID, url);
                if (lst != null && lst.Count > 0)
                {
                    lstOffices = lst;
                    var syncHelper = new OfficeOfflineHelper(this);
                    syncHelper.DeleteAll();
                    foreach (var item in lst)
                    {
                        syncHelper.AddNew(new OfficeModel() { OfficeID = int.Parse(item.Value), OfficeName = item.Text });
                    }
                }
            }
            catch (Exception ex)
            {
                GeatMessage("Office cannot be downloaded. Please contact with administrator.", true);
            }
        }
        private async Task SyncAllData(string url)
        {
            try
            {
                if (lstOffices != null)
                {
                    foreach (var office in lstOffices)
                    {
                        var loginID = SessionHelper.LoggedInUserID;
                        var lst = await LookupProviderAPI.GetMobileDataSyncAsync(int.Parse(office.Value), loginID, url);
                        if (lst != null && lst.Count > 0)
                        {
                            var installmentDateObj = lst.Where(w => !string.IsNullOrEmpty(w.InstallmentDate)).FirstOrDefault();
                            if (installmentDateObj == null)
                            {
                                LogSystemError("SyncMasterData- InstallmentDate", "Installment data is null", "");
                                GeatMessage("Day initial not found, sync failed.", false);
                                return;
                            }
                            else
                            {
                                var syncUserHelper = new UserOfflineHelper(this);
                                if (syncUserHelper.IsUserExist(loginID))
                                    syncUserHelper.Update(installmentDateObj.InstallmentDate, loginID, lst[0].EmployeeName);
                            }
                            var centerSyncHelper = new CenterOfflineHelper(this);
                            centerSyncHelper.DeleteAll();

                            var memberSyncHelper = new MemberOfflineHelper(this);
                            memberSyncHelper.DeleteAll();

                            var memberProductSyncHelper = new MemberProductOfflineHelper(this);
                            memberProductSyncHelper.DeleteAll();

                            var centerData = lst.Select(s => new { CenterID = s.CenterID, CenterName = s.CenterName, CenterCode = s.CenterCode }).Distinct();

                            foreach (var center in centerData)
                            {
                                var existing = centerSyncHelper.GetById(center.CenterID, int.Parse(office.Value));
                                if (existing == null)
                                    centerSyncHelper.AddNew(new CenterModel() { OfficeID = int.Parse(office.Value), OfficeName = office.Text, CenterID = center.CenterID, CenterName = center.CenterName });
                                //member data sync for the center.
                                var members = lst.Where(m => m.CenterID == center.CenterID).Select(s => new { MemberID = s.MemberID, MemberCode = s.MemberCode.Trim(), MemberName = s.MemberName.Trim() }).Distinct().ToList();
                                //add member records...
                                //has some issue on Member Name, so 
                                foreach (var member in members)
                                {
                                    var existingMember = memberSyncHelper.GetById(member.MemberID);
                                    if (existing == null)
                                        memberSyncHelper.AddNew(new MemberModel()
                                        {
                                            MemberID = member.MemberID,
                                            MemberName = member.MemberName,
                                            MemberCode = member.MemberCode,
                                            OfficeID = int.Parse(office.Value),
                                            OfficeName = office.Text,
                                            CenterID = center.CenterID,
                                            CenterName = center.CenterName
                                        });

                                    var prods = lst.Where(w => w.MemberID == member.MemberID).ToList();
                                    if (member.MemberID == 149892)
                                    {
                                        var cnt = prods.Count;
                                    }
                                    foreach (var product in prods)
                                    {
                                        var prod = new MemberProductModel()
                                        {
                                            MemberID = product.MemberID,
                                            ProductID = product.ProductID,
                                            ProductName = product.ProductName,
                                            ProductType = product.ProductType,
                                            InstallmentNo = product.InstallmentNo,
                                            LoanRecovery = product.LoanRecovery,
                                            Recoverable = product.Recoverable,
                                            Balance = product.Balance,
                                            PrinBalance = product.PrinBalance,
                                            SerBalance = product.SerBalance,
                                            TrxType = product.TrxType,
                                            SummaryID = product.SummaryID,
                                            CumIntCharge = product.CumIntCharge,
                                            CumInterestPaid = product.CumInterestPaid,
                                            Duration = product.Duration,
                                            DurationOverIntDue = product.DurationOverIntDue,
                                            DurationOverLoanDue = product.DurationOverLoanDue,
                                            IntDue = product.IntDue,
                                            InterestCalculationMethod = product.InterestCalculationMethod,
                                            LoanDue = product.LoanDue,
                                            LoanRepaid = product.LoanRepaid,
                                            PrincipalLoan = product.PrincipalLoan,
                                            IntCharge = product.IntCharge,
                                            NewDue = product.NewDue,
                                            MainProductCode = product.MainProductCode,
                                            CenterID = product.CenterID,
                                            MemberName = product.MemberName,
                                            Doc = product.Doc,
                                            OrgID = product.OrgID,                                        // KHALID
                                            accountNo = product.accountNo,
                                            fine = product.fine,
                                            PersonalSaving = product.PersonalSaving,
                                            PersonalWithdraw = product.PersonalWithdraw
                                            
                                        };
                                        memberProductSyncHelper.AddNew(prod);
                                        SessionHelper.OrgID = prod.OrgID;
                                    }
                                }
                            }
                        }
                        else
                        {
                            GeatMessage("No download data returned for office : " + office.Text, false);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystemError(UIItems.SyncMasterData + " - SyncAllData", ex.Message, "");
                GeatMessage("Data cannot be synced. Please contact with administrator."+ ex.Message, false);
            }
        }
        private async Task SyncCenterData(string loginID, string url)
        {
            try
            {
                if (lstOffices != null)
                {
                    var syncHelper = new CenterOfflineHelper(this);
                    syncHelper.DeleteAll();
                    foreach (var office in lstOffices)
                    {
                        var lst = await LookupProviderAPI.GetCenterListAsync(int.Parse(office.Value), loginID, url);
                        if (lst != null && lst.Count > 0)
                        {
                            foreach (var center in lst)
                            {
                                var existing = syncHelper.GetById(int.Parse(center.Value), int.Parse(office.Value));
                                if (existing == null)
                                    syncHelper.AddNew(new CenterModel() { OfficeID = int.Parse(office.Value), OfficeName = office.Text, CenterID = int.Parse(center.Value), CenterName = center.Text });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                GeatMessage("Center cannot be downloaded. Please contact with administrator.", true);
            }
        }

        private async Task SyncMemberData(string url)
        {
            try
            {
                var centerSyncHelper = new CenterOfflineHelper(this);
                var allCenters = centerSyncHelper.GetAll();

                if (allCenters != null && allCenters.Count > 0)
                {
                    var syncHelper = new MemberOfflineHelper(this);
                    syncHelper.DeleteAll();
                    foreach (var center in allCenters)
                    {
                        var lst = await LookupProviderAPI.GetMemberByCenterAsync(center.OfficeID, center.CenterID, url);
                        if (lst != null && lst.Count > 0)
                        {
                            foreach (var member in lst)
                            {
                                var existing = syncHelper.GetById(long.Parse(member.Value));
                                if (existing == null)
                                    syncHelper.AddNew(new MemberModel()
                                    {
                                        MemberID = long.Parse(member.Value),
                                        MemberName = member.Text,
                                        MemberCode = member.Text,
                                        OfficeID = center.OfficeID,
                                        OfficeName = center.OfficeName,
                                        CenterID = center.CenterID,
                                        CenterName = center.CenterName
                                    });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                GeatMessage("Member cannot be downloaded. Please contact with administrator.", true);
            }
        }

        private async Task SyncMemberProductData(string url)
        {
            try
            {
                var memberSyncHelper = new MemberOfflineHelper(this);
                var allmembers = memberSyncHelper.GetAll();

                if (allmembers != null && allmembers.Count > 0)
                {
                    var syncHelper = new MemberProductOfflineHelper(this);
                    syncHelper.DeleteAll();
                    foreach (var item in allmembers)
                    {
                        var lst = await LookupProviderAPI.GetMemberProductsAsync(item.OfficeID, item.CenterID, item.MemberID, url);
                        if (lst != null && lst.Count > 0)
                        {
                            foreach (var product in lst)
                            {
                                syncHelper.AddNew(product);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                GeatMessage("Member Product cannot be downloaded. Please contact with administrator.", true);
            }
        }

        private async Task SyncProductData(string url)
        {
            try
            {
                var lst = await LookupProviderAPI.GetProductListAsync(url);
                if (lst != null && lst.Count > 0)
                {
                    var syncHelper = new ProductOfflineHelper(this);
                    syncHelper.DeleteAll();
                    foreach (var item in lst)
                    {
                        syncHelper.AddNew(new ProductModel() { ProductID = int.Parse(item.Value), ProductName = item.Text });
                    }
                }
            }
            catch (Exception ex)
            {
                GeatMessage("Product cannot be downloaded. Please contact with administrator.", true);
            }
        }
        private async Task SyncPurposeData(string url)
        {
            try
            {
                var lst = await LookupProviderAPI.GetPurposeListAsync(url);
                if (lst != null && lst.Count > 0)
                {
                    var syncHelper = new PurposeOfflineHelper(this);
                    syncHelper.DeleteAll();
                    foreach (var item in lst)
                    {
                        syncHelper.AddNew(new PurposeModel() { PurposeID = int.Parse(item.Value), PurposeName = item.Text });
                    }
                }
            }
            catch (Exception ex)
            {
                GeatMessage("Purpose cannot be downloaded.. Please contact with administrator.", true);
            }
        }
        private void GeatMessage(string message, bool flag)
        {
            new CustomMessage(this, this, message, flag).Show();
        }
        private bool RequiredFieldValidation(EditText control, string message)
        {
            var isValid = true;
            if (string.IsNullOrWhiteSpace(control.Text.Trim()))
            {
                new CustomMessage(this, this, message, false).Show();
                isValid = false;
                control.FindFocus();
            }
            return isValid;
        }
        private void LogException(Exception ex)
        {
            new CustomMessage(this, this, ex).Show();
        }
    }
}