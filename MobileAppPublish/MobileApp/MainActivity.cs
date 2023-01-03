using Android.App;
using Android.Widget;
using Android.OS;
using System;
using PMS.Droid.Helpers;
using PMS.Droid.Classes.OffLineHelpers;
using System.Linq;
using System.Collections.Generic;

namespace PMS.Droid
{
    [Activity(Label = "gBanker", Theme = "@android:style/Theme.Light", Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        private bool isValid = true;
        private List<OfficeModel> lstOffices = null;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Main);
            var btnLogin = FindViewById<Button>(Resource.Id.btnSelectOfficeOffline);
            var btnSyncMasterDataHome = FindViewById<Button>(Resource.Id.btnSyncMasterDataHome);
            btnSyncMasterDataHome.Click += BtnSyncMasterDataHome_Click;
            var btnSetupUrl = FindViewById<Button>(Resource.Id.btnSetupUrl);
            btnSetupUrl.Click += BtnSetupUrl_Click;
            SetOfficeSpinners();
            btnLogin.Click += delegate
           {
               CommonHelper.Busy(this);
               try
               {
                   var spOffice = FindViewById<Spinner>(Resource.Id.spnSelectOfficeOffline);

                   var selectedOffice = spOffice.SelectedItem.ToString();
                   if (lstOffices != null)
                   {
                       var office = lstOffices.Where(w => w.OfficeName == selectedOffice).FirstOrDefault();
                       if (office != null)
                       {
                           SessionHelper.OfficeID = office.OfficeID;
                           SessionHelper.OfficeName = office.OfficeName;
                           SessionHelper.ProductList = null;
                           SessionHelper.PurposeList = null;
                           SessionHelper.CenterList = null;
                           StartActivity(typeof(MobileHomeActivity));
                       }

                   }
               }
               catch (Exception ex)
               {
                   GeatMessage("Exception Occured. Please contact with administrator.", true);
               }
               CommonHelper.Done(this);
           };

            var btnExit = FindViewById<Button>(Resource.Id.btnExit);
            btnExit.Click += BtnExit_Click;
        }

        private void BtnSetupUrl_Click(object sender, EventArgs e)
        {
            StartActivity(typeof(SetupOrganizationActivity));
        }

        private void BtnExit_Click(object sender, EventArgs e)
        {
            this.FinishAffinity();
        }

        private void BtnSyncMasterDataHome_Click(object sender, EventArgs e)
        {
            StartActivity(typeof(SyncMasterDataActivity));
        }

        private void GeatMessage(string message, bool flag)
        {
            new CustomMessage(this, this, message, flag).Show();
        }
        private bool RequiredFieldValidation(EditText control, string message)
        {
            isValid = true;
            if (string.IsNullOrWhiteSpace(control.Text.Trim()))
            {
                new CustomMessage(this, this, message, false).Show();
                isValid = false;
                control.FindFocus();
            }
            return isValid;
        }
        private void SetOfficeSpinners()
        {

            try
            {
                //CheckDbVersion();
                var officeHelper = new OfficeOfflineHelper(this);
                var lst = officeHelper.GetAll();
                lstOffices = lst;
                var spnrOffice = FindViewById<Spinner>(Resource.Id.spnSelectOfficeOffline);
                var officeAdapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerItem, lst.Select(s => s.OfficeName).ToList());
                officeAdapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
                spnrOffice.Adapter = officeAdapter;
            }
            catch (Exception ex)
            {
                GeatMessage("Exception Occured. Please contact with administrator.", true);
            }
            CommonHelper.Done(this);
        }

        
    }
}

