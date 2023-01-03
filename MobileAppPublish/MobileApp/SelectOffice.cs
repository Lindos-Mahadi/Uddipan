using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.OS;
using Android.Widget;
using PMS.Droid.Helpers;
using PMS.Droid.Classes.OffLineHelpers;

namespace PMS.Droid
{
    [Activity(Label = "Select Office", Theme = "@style/Theme.DesignDemo")]
    public class SelectOffice : BaseActivity
    {
        private List<OfficeModel> lstOffices = null;
        protected  override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.SelectOffice);
             SetOfficeSpinners();
            // Create your application here
            var btnSelectOffice = FindViewById<Button>(Resource.Id.btnSelectOffice);
            btnSelectOffice.Click += btnSelectOffice_Click;
            GenerateNavigationMenu();
        }

        private void btnSelectOffice_Click(object sender, EventArgs e)
        {
            try
            {
                var spOffice = FindViewById<Spinner>(Resource.Id.spnSelectOffice);

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
                        StartActivity(typeof(UserHomeActivity));
                    }

                }
            }
            catch (Exception ex)
            {
                GeatMessage("Exception Occured. Please contact with administrator.", true);
            }
        }

        private  void SetOfficeSpinners()
        {
            try
            {
                //CheckDbVersion();
                var officeHelper = new OfficeOfflineHelper(this);
                var lst = officeHelper.GetAll();
                lstOffices = lst;
                var spnrOffice = FindViewById<Spinner>(Resource.Id.spnSelectOffice);
                var officeAdapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerItem, lst.Select(s => s.OfficeName).ToList());
                officeAdapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
                spnrOffice.Adapter = officeAdapter;
            }
            catch (Exception ex)
            {
                GeatMessage("Exception Occured. Please contact with administrator.", true);
            }
        }
        private void GeatMessage(string message, bool flag)
        {
            new CustomMessage(this, this, message, flag).Show();
        }
    }
}