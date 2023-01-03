
using System;
using System.Linq;

using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using PMS.Droid.Helpers;
using PMS.Droid.Classes.OffLineHelpers;

namespace PMS.Droid
{
    [Activity(Label = "Collection", Theme = "@style/Theme.DesignDemo", WindowSoftInputMode = SoftInput.AdjustPan, ScreenOrientation = Android.Content.PM.ScreenOrientation.UserLandscape)]
    public class CollectionNewWithGrid : BaseCollectionActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            try
            {
                SetContentView(Resource.Layout.CollectionNewWithGrid);
                GenerateNavigationMenu();

                var syncUserHelper = new UserOfflineHelper(this);
                var user = syncUserHelper.GetUser();
                if (user != null && !string.IsNullOrEmpty(user.InstallmentDate))
                {
                    // var lblSamityAndDate = FindViewById<TextView>(Resource.Id.lblDate);
                    DateTime dt;
                    if (DateTime.TryParse(user.InstallmentDate, out dt))
                        Title = "Collection - " + dt.ToString("MMMM dd, yyyy");

                }

                SetSpinners();
                //LoadGrid();
            }
            catch (Exception ex)
            {
                LogSystemError(UIItems.Collection + " - Onload", ex.Message, "");
            }
        }
        private void SetSpinners()
        {
            CommonHelper.Busy(this);
            SetCenterSpinners();
            // var spnCenter = FindViewById<Spinner>(Resource.Id.spnCenterCollection);
            CommonHelper.Done(this);
        }
        private void GeatMessage(string message, bool flag)
        {
            new CustomMessage(this, this, message, flag).Show();
        }
        private void LogException(Exception ex)
        {
            new CustomMessage(this, this, ex).Show();
        }
        //spnProduct
        private void SetCenterSpinners()
        {
            var officeId = SessionHelper.OfficeID;
            if (default(int) != officeId)
            {
                var syncHelper = new CenterOfflineHelper(this);
                var lst = syncHelper.GetCentersByOffice(officeId, 1);
                SessionHelper.CenterList = lst;
                var spnCenter = FindViewById<Spinner>(Resource.Id.spnCenterCollection);
                var adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerItem, lst.Select(s => s.CenterName).ToList());
                adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
                spnCenter.Adapter = adapter;
                spnCenter.ItemSelected += SpnCenter_ItemSelected;
            }
            else
                GeatMessage("No office selected. Please select ofice and then try again.", false);
        }
        private void SpnCenter_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            var obj = ((Spinner)sender).SelectedItem.ToString();
            if (!string.IsNullOrEmpty(obj))
            {
                var centerId = GetCenterId(obj);
                LoadGrid(centerId);
            }
        }
        private int GetCenterId(string text)
        {
            var result = 0;
            var item = SessionHelper.CenterList.Where(w => w.CenterName == text).FirstOrDefault();
            if (item != null)
                result = item.CenterID;
            return result;
        }
        private void LoadGrid(int centerId)
        {
            var dbVals = new MemberProductOfflineHelper(this);
            var listItsms = dbVals.GetCollectionRecordsAll(centerId);
            var prodList = listItsms.Select(s => s.ProductName).ToList();
            var lv = FindViewById<ExpandableListView>(Resource.Id.LoanCollectionGridNew);
            lv.SetAdapter(new LoanCollectionExpandableListAdapter(this, prodList, listItsms));

            var txtTotalMember = FindViewById<TextView>(Resource.Id.txtTotalMember);
            var txtTotalCollectionMember = FindViewById<TextView>(Resource.Id.txtTotalCollectionMember);
            var txtTotalRecoverable = FindViewById<TextView>(Resource.Id.txtTotalRecoverable); ;
            var txtTotalLoanCollection = FindViewById<TextView>(Resource.Id.txtTotalLoanCollection);
            var txtTotalGSCollection = FindViewById<TextView>(Resource.Id.txtTotalGSCollection);
            var txtTotalVSCollection = FindViewById<TextView>(Resource.Id.txtTotalVSCollection);
            var txtTotalLTSCollection = FindViewById<TextView>(Resource.Id.txtTotalLTSCollection);
            var adapter = lv.ExpandableListAdapter as LoanCollectionExpandableListAdapter;
            if (adapter != null)
            {
                adapter.SetTotalRowFields(txtTotalMember, txtTotalCollectionMember, txtTotalRecoverable, txtTotalLoanCollection, txtTotalGSCollection, txtTotalVSCollection, txtTotalLTSCollection);
                adapter.UpdateSummary();
            }
        }
    }
}