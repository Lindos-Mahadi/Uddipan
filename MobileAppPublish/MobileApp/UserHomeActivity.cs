using System;

using Android.App;
using Android.OS;
using Android.Widget;
using PMS.Droid.Helpers;
using PMS.Droid.Classes.OffLineHelpers;
using System.Linq;

namespace PMS.Droid
{
    [Activity(Label = "Home", Theme = "@style/Theme.DesignDemo", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class UserHomeActivity : BaseActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.UserHome);

            GenerateNavigationMenu(true);
            // Create your application here
            try
            {
                var syncHelper = new UserOfflineHelper(this);
                var user = syncHelper.GetUser();
                if (user != null)
                {
                    if (!string.IsNullOrEmpty(user.InstallmentDate))
                    {
                        var lblTrDate = FindViewById<TextView>(Resource.Id.lblTransactionDate);
                        DateTime dt;
                        if (DateTime.TryParse(user.InstallmentDate, out dt))
                            lblTrDate.Text = "Transaction Date: " + dt.ToString("MMMM dd, yyyy");
                    }

                    var txtUser = FindViewById<TextView>(Resource.Id.lblLoggedinAs);
                    if (!string.IsNullOrEmpty(SessionHelper.LoggedInUserID))
                        txtUser.Text = string.Format("Logged-In As: {0} - {1} ", SessionHelper.LoggedInUserID, user.Name);
                }
                var office = FindViewById<TextView>(Resource.Id.lblSelectedOffice);
                if (!string.IsNullOrEmpty(SessionHelper.OfficeName))
                {
                    office.Text = string.Format("Selected Office: {0}", SessionHelper.OfficeName);
                }
                else
                    office.Text = "Selected Office: N/A";
                LoadCollectionList();


            }
            catch (Exception ex)
            {
                ShowMessage("Error occured. Please contact with Administrator", false);
            }
        }

        private void LoadCollectionList()
        {
            var dbVals = new LoanCollectionOfflineHelper(this);
            var listItsms = dbVals.GetCollectionSummary();
            var lv = FindViewById<ListView>(Resource.Id.collectionSummaryList);
            lv.Adapter = new CollectionSummaryGridAdapter(this, listItsms.OrderBy(o => o.RecordSequence).ToList());
        }
    }
}