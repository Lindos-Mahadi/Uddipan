


using System;
using System.Linq;

using Android.App;
using Android.OS;
using Android.Widget;
using PMS.Droid.Classes;
using PMS.Droid.Helpers;
using System.Threading.Tasks;
using PMS.Droid.Classes.OffLineHelpers;
using GBPMS.Droid.Classes;
using Android.Graphics;

namespace PMS.Droid
{
    [Activity(Label = "Download New Savings", Theme = "@style/Theme.DesignDemo", NoHistory = true, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class DownloadNewSavingsActivity : BaseActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.DownloadNewSavings);
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
                            var apiVersion = await LoginHelper.GetAPIVersion(url, OfflineDBConstants.APP_DATABASE_VERSION, txtUser.Text);
                            string[] apiInfo = apiVersion.Split('#');
                            if (OfflineDBConstants.APP_VERSION < Int32.Parse(apiInfo[1]))
                            {
                                GeatMessage("You are using an older version of APP. Please upgrade your APP and try again.", false);
                                CommonHelper.Done(this);
                                return;
                            }
                            var result = await LoginHelper.IsUserAuthenticated(txtUser.Text.Trim(), txtPwd.Text, url);
                            if (result)
                            {
                                SessionHelper.LoggedInUserID = txtUser.Text.Trim();
                                await SyncAllData(url);                                
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

        private async Task SyncAllData(string url)
        {
            try
            {
                var officeHelper = new OfficeOfflineHelper(this);
                var allOffices = officeHelper.GetAll();
                if (allOffices != null && allOffices.Count > 0)
                {
                    foreach (var office in allOffices)
                    {
                        var loginID = SessionHelper.LoggedInUserID;
                        var lst = await LookupProviderAPI.GetMobileDataNewSavingsMemberAsync(office.OfficeID, loginID, url);
                        if (lst != null && lst.Count > 0)
                        {
                            var centerSyncHelper = new CenterOfflineHelper(this);

                            var memberSyncHelper = new MemberOfflineHelper(this);

                            var memberProductSyncHelper = new MemberProductOfflineHelper(this);

                            var centerData = lst.Select(s => new { CenterID = s.CenterID, CenterName = s.CenterName, CenterCode = s.CenterCode }).Distinct();

                            foreach (var center in centerData)
                            {
                                var existing = centerSyncHelper.GetById(center.CenterID, office.OfficeID);
                                if (existing == null)
                                    centerSyncHelper.AddNew(new CenterModel() { OfficeID = office.OfficeID, OfficeName = office.OfficeName, CenterID = center.CenterID, CenterName = center.CenterName });
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
                                            OfficeID = office.OfficeID,
                                            OfficeName = office.OfficeName,
                                            CenterID = center.CenterID,
                                            CenterName = center.CenterName
                                        });

                                    var prods = lst.Where(w => w.MemberID == member.MemberID).ToList();                                  
                                    foreach (var product in prods)
                                    {
                                        var isProdExist = memberProductSyncHelper.IsProductExist(product.SummaryID, product.ProductType);
                                        if (!isProdExist)
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
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            GeatMessage("No download data returned for office : " + office.OfficeName, false);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystemError(UIItems.SyncMasterData + " - SyncAllData", ex.Message, "");
                GeatMessage("Data cannot be synced. Please contact with administrator.", false);
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