using System;
using Android.Content;
using Android.Views;
using Android.OS;
using V7Toolbar = Android.Support.V7.Widget.Toolbar;
using Android.Support.V7.App;
using Android.Support.V4.Widget;
using Android.Support.Design.Widget;
using PMS.Droid.Helpers;
using PMS.Droid.Classes.OffLineHelpers;
using PMS.Droid.Classes;
using gBanker.Web.Helpers;

namespace PMS.Droid
{
    public class BaseCollectionActivity : AppCompatActivity
    {
        DrawerLayout drawerLayout;
        NavigationView navigationView;
        string OrganizationURL = "";
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
        }

        public void GenerateNavigationMenu(bool setBackground = false)
        {
            try
            {
                var user = SessionHelper.LoggedInUserID;

                var toolbar = FindViewById<V7Toolbar>(Resource.Id.toolbar);
                SetSupportActionBar(toolbar);
                SupportActionBar.SetDisplayHomeAsUpEnabled(true);
                SupportActionBar.SetDisplayShowTitleEnabled(true);
                //if (setBackground)
                //{
                //    Drawable icon = ContextCompat.GetDrawable(this, Resource.Drawable.UserHome);
                //    SupportActionBar.SetBackgroundDrawable(icon);
                //}
                SupportActionBar.SetHomeButtonEnabled(true);
                SupportActionBar.SetHomeAsUpIndicator(Resource.Drawable.ic_menu);
                drawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
                navigationView = FindViewById<NavigationView>(Resource.Id.nav_view);

                if (string.IsNullOrEmpty(user))
                {
                    navigationView.Menu.Clear();
                    navigationView.InflateMenu(Resource.Menu.menu_settings);
                }
                else
                {
                    navigationView.Menu.Clear();
                    navigationView.InflateMenu(Resource.Menu.menu);
                }
                navigationView.ItemIconTintList = null;
                navigationView.NavigationItemSelected += HomeNavigationView_NavigationItemSelected;
            }
            catch (Exception ex)
            {
                LogSystemError("Base - Generate Menu", ex.Message, "");
            }
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    drawerLayout.OpenDrawer(Android.Support.V4.View.GravityCompat.Start);
                    return true;
            }
            return base.OnOptionsItemSelected(item);
        }
        private void HomeNavigationView_NavigationItemSelected(object sender, NavigationView.NavigationItemSelectedEventArgs e)
        {
            var menuItem = e.MenuItem;
            menuItem.SetChecked(!menuItem.IsChecked);
            Intent intent = null;
            switch (menuItem.ItemId)
            {
                case Resource.Id.nav_home:
                    intent = new Intent(this, typeof(UserHomeActivity));
                    break;
                case Resource.Id.collectionEntry:
                    intent = new Intent(this, typeof(CollectionNewActivity));
                    break;
                case Resource.Id.collectionEntryGrid:
                    intent = new Intent(this, typeof(CollectionNewWithGrid));
                    break;
                //collectionEntryGrid
                //case Resource.Id.proposalEntry:
                //    intent = new Intent(this, typeof(LoanProposalActivity));
                //    break;
                case Resource.Id.summary:
                    intent = new Intent(this, typeof(CollectionSummaryActivity));
                    break;
                case Resource.Id.rptCenterWiseCollectionList:
                    intent = new Intent(this, typeof(CollectionReportActivity));
                    break;
                case Resource.Id.rptCenterWiseWithdrawalList:
                    intent = new Intent(this, typeof(WithdrawalReportActivity));
                    break;
                case Resource.Id.office:
                    intent = new Intent(this, typeof(SelectOffice));
                    break;
                case Resource.Id.syncCollection:
                    intent = new Intent(this, typeof(SyncCollectionActivity));
                    break;
                case Resource.Id.DeletedCollection:
                    intent = new Intent(this, typeof(DeletedCollectionActivity));
                    break;
                //case Resource.Id.syncProposal:
                //    intent = new Intent(this, typeof(SyncLoanProposalActivity));
                //    break;
                case Resource.Id.syncMasterData:
                    intent = new Intent(this, typeof(SyncMasterDataActivity));
                    break;
                case Resource.Id.setupUrl:
                    intent = new Intent(this, typeof(SetupOrganizationActivity));
                    break;
                case Resource.Id.specialCollectionEntry:
                    //GeatMessage("This page is under construction. Please try in next release.", true);
                    intent = new Intent(this, typeof(SpecialCollectionActivity));
                    break;
                case Resource.Id.withdrawalEntry:
                    //GeatMessage("This page is under construction. Please try in next release.", true);
                    intent = new Intent(this, typeof(WithdrawalActivity));
                    break;
                case Resource.Id.aboutgBanker:
                    var msg = string.Format("gBanker - Android App Version: {0}", OfflineDBConstants.APP_DATABASE_VERSION);
                    Android.App.AlertDialog.Builder builder = new Android.App.AlertDialog.Builder(this);
                    Android.App.AlertDialog confirm = builder.Create();
                    confirm.SetTitle("gBanker Version");
                    confirm.SetMessage(msg);
                    confirm.SetButton("OK", (s, ev) =>
                    {

                    });
                    confirm.Show();
                    drawerLayout.CloseDrawers();
                    break;
                case Resource.Id.exit:
                    Exit();
                    break;
                //=====Web links..
                case Resource.Id.memberRegistrationWeb:
                    GenerateWebRequestIntent(1, ref intent);
                    drawerLayout.CloseDrawers();
                    break;
                case Resource.Id.proposalWeb:
                    GenerateWebRequestIntent(2, ref intent);
                    drawerLayout.CloseDrawers();
                    break;
                case Resource.Id.miscCollectionWeb:
                    GenerateWebRequestIntent(3, ref intent);
                    drawerLayout.CloseDrawers();
                    break;
                case Resource.Id.subsidiaryLedgerWeb:
                    GenerateWebRequestIntent(4, ref intent);
                    drawerLayout.CloseDrawers();
                    break;
                case Resource.Id.topSheetWeb:
                    GenerateWebRequestIntent(5, ref intent);
                    drawerLayout.CloseDrawers();
                    break;
                case Resource.Id.loanLedgerWeb:
                    GenerateWebRequestIntent(6, ref intent);
                    drawerLayout.CloseDrawers();
                    break;
                case Resource.Id.savingsLedgerWeb:
                    GenerateWebRequestIntent(7, ref intent);
                    drawerLayout.CloseDrawers();
                    break;
                case Resource.Id.savingsAccOpeningWeb:
                    GenerateWebRequestIntent(8, ref intent);
                    drawerLayout.CloseDrawers();
                    break;
                default:
                    break;
            }
            if (intent != null)
                StartActivity(intent);
        }

        private void LoadOrganizationUrl()
        {
            if (string.IsNullOrEmpty(OrganizationURL))
                OrganizationURL = SharedMethods.GetOrganizationBaseUrl(this);
        }
        public void Exit()
        {
            var msg = "Are you sure to exit?";
            Android.App.AlertDialog.Builder builder = new Android.App.AlertDialog.Builder(this);
            Android.App.AlertDialog confirm = builder.Create();
            confirm.SetTitle("Confirm Exit");
            confirm.SetMessage(msg);
            confirm.SetButton("OK", (s, ev) =>
            {
                this.FinishAffinity();
            });
            confirm.SetButton2("Cancel", (s, ev) =>
            {

            });
            confirm.Show();
        }
        public void ShowMessage(string message, bool flag)
        {
            GeatMessage(message, flag);
        }
        private void GeatMessage(string message, bool flag)
        {
            new CustomMessage(this, this, message, flag).Show();
        }

        private void GenerateWebRequestIntent(int urlId, ref Intent intent)
        {
            var syncUserHelper = new UserOfflineHelper(this);
            var userInfo = syncUserHelper.GetUser();
            var officeId = SessionHelper.OfficeID;
            var result = "";
            if (default(int) != officeId && userInfo != null)
            {
                LoadOrganizationUrl();
                var expDate = DateTime.Now.AddMinutes(5);
                var minRange = (new Random()).Next(500, 1000);
                var maxRange = (new Random()).Next(1100, 1500);
                var randomNumber = (new Random()).Next(minRange, maxRange);
                var val = string.Format("{0}_{1}_{2}_{3}_{4}_{5}_{6}_{7}", urlId, userInfo.LoginID, minRange, maxRange, randomNumber, expDate.ToString("MM/dd/yyyy"), officeId, userInfo.Password);
                if (!string.IsNullOrEmpty(OrganizationURL))
                {
                    result = string.Format(@"{0}Account/LoginExternal?rd={1}", OrganizationURL, Cryptor.ActionEncrypt(val));
                    var uri = Android.Net.Uri.Parse(result);
                    intent = new Intent(Intent.ActionView, uri);
                }
            }
        }
        protected void LogSystemError(string action, string errorText, string inputParams)
        {
            try
            {
                var model = new SystemErrorModel() { ActionName = action, CreateDate = DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss tt"), ErrorText = errorText, InputParameters = inputParams, UserID = SessionHelper.LoggedInUserID };
                var errorHelper = new SystemErrorOfflineHelper(this);
                errorHelper.AddNew(model);
            }
            catch (Exception ex)
            {
            }
        }
    }
}