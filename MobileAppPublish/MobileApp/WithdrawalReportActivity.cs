
using System;
using System.Collections.Generic;
using System.Linq;

using Android.App;
using Android.OS;
using Android.Widget;
using PMS.Droid.Classes.OffLineHelpers;
using PMS.Droid.Helpers;

namespace PMS.Droid
{
    [Activity(Label = "Withdrawals", Theme = "@style/Theme.DesignDemo", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class WithdrawalReportActivity : BaseActivity
    {
        private List<CenterModel> centers = null;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.WithdrawalReport);
            GenerateNavigationMenu();
            try
            {
                // LoadCollectionList();
                var syncHelper = new UserOfflineHelper(this);
                var user = syncHelper.GetUser();
                if (user != null && !string.IsNullOrEmpty(user.InstallmentDate))
                {
                    DateTime dt;
                    if (DateTime.TryParse(user.InstallmentDate, out dt))
                    {
                        Title = "Withdrawals - " + dt.ToString("MMM dd, yyyy");
                    }
                }
                SetCenterSpinners();
                var spnCenter = FindViewById<Spinner>(Resource.Id.spnCenterCollection);
                spnCenter.ItemSelected += SpnCenter_ItemSelected;

            }
            catch (Exception ex)
            {
                LogSystemError(UIItems.CollectionList + " - Onload", ex.Message, "");
                GeatMessage(ex.Message, true);
            }
            // Create your application here
        }

        private void SpnCenter_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            try
            {
                var spnCenter = FindViewById<Spinner>(Resource.Id.spnCenterCollection);
                if (spnCenter.SelectedItem != null)
                {
                    var centerName = spnCenter.SelectedItem.ToString();
                    var centerId = centers == null ? null : centers.Where(w => w.CenterName == centerName).FirstOrDefault();
                    if (centerId != null)
                        LoadCollectionList(centerId.CenterID);
                }
                else
                    LoadCollectionList(-1);
            }
            catch (Exception ex)
            {
                LogSystemError(UIItems.CollectionList + " - SpnCenter_ItemSelected", ex.Message, "");
            }
        }

        private void LoadCollectionList(int centerID)
        {
            var dbVals = new WithdrawalOfflineHelper(this);
            var listItsms = dbVals.GetMemberWiseWithdrawalSummary(centerID);
            var lv = FindViewById<ListView>(Resource.Id.collectionSummaryList);
            lv.Adapter = new CollectionReportGridAdapter(this, listItsms.OrderBy(o => o.RecordType).ThenBy(b => b.MemberName).ToList(), centerID, true);
        }
        private void SetCenterSpinners()
        {
            var officeId = SessionHelper.OfficeID;
            if (default(int) != officeId)
            {
                var syncHelper = new CenterOfflineHelper(this);
                var lst = syncHelper.GetCentersByOfficeForWithdrawal(officeId, true);
                centers = lst;
                var spnCenter = FindViewById<Spinner>(Resource.Id.spnCenterCollection);
                var adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerItem, lst.Select(s => s.CenterName).ToList());
                adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
                spnCenter.Adapter = adapter;
            }
            else
            {
                GeatMessage("No office selected. Please select ofice and then try again.", false);
            }
        }
        private void GeatMessage(string message, bool flag)
        {
            new CustomMessage(this, this, message, flag).Show();
        }
    }
}