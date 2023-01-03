using System;
using System.Linq;

using Android.App;
using Android.OS;
using Android.Widget;
using PMS.Droid.Classes.OffLineHelpers;

namespace PMS.Droid
{
    [Activity(Label = "Collection Summary", Theme = "@style/Theme.DesignDemo", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class CollectionSummaryActivity : BaseActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.CollectionSummary);
            GenerateNavigationMenu();
            try
            {
                LoadCollectionList();
                var syncHelper = new UserOfflineHelper(this);
                var user = syncHelper.GetUser();
                if (user != null && !string.IsNullOrEmpty(user.InstallmentDate))
                {
                    DateTime dt;
                    if (DateTime.TryParse(user.InstallmentDate, out dt))
                    {
                        var lblTrDate = FindViewById<TextView>(Resource.Id.lblCollectionSummaryTitle);
                        lblTrDate.Text = "Collection Summary, Date: " + dt.ToString("MMMM dd, yyyy");
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystemError(UIItems.CollectionSummary + " - OnLoad", ex.Message, "");
                GeatMessage(ex.Message, true);
            }
            // Create your application here
        }
        private void LoadCollectionList()
        {
            var dbVals = new LoanCollectionOfflineHelper(this);
            var listItsms = dbVals.GetCollectionSummary();
            var lv = FindViewById<ListView>(Resource.Id.collectionSummaryList);
            lv.Adapter = new CollectionSummaryGridAdapter(this, listItsms.OrderBy(o => o.RecordSequence).ToList());
        }
        private void GeatMessage(string message, bool flag)
        {
            new CustomMessage(this, this, message, flag).Show();
        }
    }
}