using System;

using Android.App;
using Android.OS;
using Android.Text;
using Android.Widget;
using PMS.Droid.Classes.OffLineHelpers;
using PMS.Droid.Helpers;

namespace PMS.Droid
{
    [Activity(Label = "Setup Organization", Theme = "@style/Theme.DesignDemo", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class SetupOrganizationActivity : BaseActivity
    {
        static int onload;
        string urlText = "";
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.SetupOrganizationUrl);
            GenerateNavigationMenu();
            try
            {
                Spinner spinner = FindViewById<Spinner>(Resource.Id.spinner);
                spinner.ItemSelected += spinner_ItemSelected;
               // spinner.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(spinner_ItemSelected);
                var adapter = ArrayAdapter.CreateFromResource(this, Resource.Array.Option_array, Android.Resource.Layout.SimpleSpinnerItem);
                adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
                spinner.Adapter = adapter;
                
                LoadOrg();
                var btnSave = FindViewById<Button>(Resource.Id.btnSave);
                btnSave.Click += BtnSave_Click;

                var Port = FindViewById<EditText>(Resource.Id.txtPort);
                Port.TextChanged += txtPort_TextChanged;

                onload = 1;

            }
            catch (Exception ex)
            {
                LogSystemError(UIItems.SetupUrl, ex.Message, "");
            }
            // Create your application here
        }

        private void txtPort_TextChanged(object sender, TextChangedEventArgs e)
        {
            var url = FindViewById<EditText>(Resource.Id.txtOrganizationUrlNew);

            //if (onload == 1)
            //{
            //    urlText = url.Text;
            //}
            //else
            //{
            //    urlText = url.Text;
            //}
            onload = onload + 1;
            
            var Port = FindViewById<EditText>(Resource.Id.txtPort);
             
            url.Text = urlText + Port.Text.Trim() + "/";

        }


        private void spinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            Spinner spinner = (Spinner)sender;
            string toast = string.Format("http://www.gbanker.{0}:", spinner.GetItemAtPosition(e.Position));
            //Toast.MakeText(this, toast, ToastLength.Long).Show();
            var url = FindViewById<EditText>(Resource.Id.txtOrganizationUrlNew);
            //var OrgID = SessionHelper.OrgID;
            url.Text = toast.Trim();

            urlText = url.Text;

            var Port = FindViewById<EditText>(Resource.Id.txtPort);
            Port.Text = "";
           // Port.Text = SessionHelper.OrgID.ToString();

        }

        private void LoadOrg()
        {
            var name = FindViewById<EditText>(Resource.Id.txtOrganizationName);
            var url = FindViewById<TextView>(Resource.Id.txtOrganizationUrl);
            var syncHelper = new OrganizationUrlOfflineHelper(this);
            var org = syncHelper.Get(); ;
            if(org!=null)
            {
                name.Text = org.Name;
                url.Text = org.OrganizationUrl;
            }
            else
                url.Text = "http://www.gbanker.net:";
            
        }
        private void BtnSave_Click(object sender, EventArgs e)
        {
            try
            {
                var name = FindViewById<EditText>(Resource.Id.txtOrganizationName);
                //var url = FindViewById<EditText>(Resource.Id.txtOrganizationUrl);
                var url = FindViewById<EditText>(Resource.Id.txtOrganizationUrlNew);
                var urlOld = FindViewById<TextView>(Resource.Id.txtOrganizationUrl);




                var isValid = RequiredFieldValidation(name, "Organization Name is required");
                isValid = RequiredFieldValidation(url, "Organization URL is required");

                if (isValid)
                {
                    var syncHelper = new OrganizationUrlOfflineHelper(this);
                    syncHelper.AddNew(new OrganizationModel() { Name = name.Text.Trim(), OrganizationUrl = url.Text.Trim() });
                    GeatMessage("Organization Url saved successfully.", true);

                    urlOld.Text = url.Text;
                    url.Text = "http://www.gbanker.net:";

                }
            }
            catch (Exception ex)
            {
                LogSystemError(UIItems.SetupUrl + " - Save", ex.Message, "");
            }
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

        /// <summary>
        /// Validate TextView
        /// </summary>
        /// <param name="control">TextView Field</param>
        /// <param name="message">String Messahe to Show</param>
        /// <returns>boolean</returns>
        private bool RequiredFieldValidation(TextView control, string message) //KHALID
        {
            var isValid = true;
            if (string.IsNullOrWhiteSpace(control.Text.Trim()))
            {
                new CustomMessage(this, this, message, false).Show();
                isValid = false;
                control.FindFocus();
            }
            if (control.Text.Trim().Length < 25)
            {
                new CustomMessage(this, this, message, false).Show();
                isValid = false;
                control.FindFocus();
            }
            return isValid;
        }

        /// <summary>
        /// Generate Message
        /// </summary>
        /// <param name="message"> string message </param>
        /// <param name="flag"> bool flag </param>
        private void GeatMessage(string message, bool flag)
        {
            new CustomMessage(this, this, message, flag).Show();
        }
    }
}