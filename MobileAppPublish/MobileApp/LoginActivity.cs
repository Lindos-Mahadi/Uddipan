using System;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using PMS.Droid.Classes.OffLineHelpers;
using PMS.Droid.Helpers;

namespace PMS.Droid
{
    [Activity(Label = "Login - gBanker", NoHistory = true, Theme = "@android:style/Theme.Light", Icon = "@drawable/icon", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class LoginActivity : Activity
    {
        private int loginAttempCounter = 0;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Login);

            var txtUser = FindViewById<EditText>(Resource.Id.txtUser);
            var txtPwd = FindViewById<EditText>(Resource.Id.txtPass);
            //txtUser.Text = "1316";
            //txtPwd.Text = "123456";
            var btnLogin = FindViewById<Button>(Resource.Id.btnLogin);
            btnLogin.Click += BtnLogin_Click;
            // Create your application here
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            try
            {
                var txtUser = FindViewById<EditText>(Resource.Id.txtUser);
                var txtPwd = FindViewById<EditText>(Resource.Id.txtPass);
                var msg = "";
                if (string.IsNullOrEmpty(txtUser.Text))
                    msg = "Login ID required";
                else if (string.IsNullOrEmpty(txtPwd.Text))
                    msg = "Password is required";
                if (!string.IsNullOrEmpty(msg))
                {
                    GeatMessage(msg, false);
                    return;
                }
                var userHelper = new UserOfflineHelper(this);
                if (userHelper.IsValidLogin(txtUser.Text.Trim(), txtPwd.Text))
                {
                    SessionHelper.LoggedInUserID = txtUser.Text.Trim();
                    //=========
                    var officeHelper = new OfficeOfflineHelper(this);
                    var lst = officeHelper.GetAll();
                    if (lst != null && lst.Count == 1)
                    {
                        SessionHelper.OfficeID = lst[0].OfficeID;
                        SessionHelper.OfficeName = lst[0].OfficeName;
                         
                        SessionHelper.ProductList = null;
                        SessionHelper.PurposeList = null;
                        SessionHelper.CenterList = null;
                    }
                    Intent intent = new Intent(this, typeof(UserHomeActivity));
                    StartActivity(intent);
                }
                else
                {
                    loginAttempCounter++;
                    if (loginAttempCounter <= 3)
                        GeatMessage("Incorrect credential. Please try again.", false);
                    else
                    {
                        GeatMessage("You have entered wrong password three times. System will exit.", false);
                        this.FinishAffinity();
                    }
                }

            }
            catch (Exception ex)
            {
                LogSystemError(UIItems.Login, ex.Message, "");
            }
        }
        private void GeatMessage(string message, bool flag)
        {
            new CustomMessage(this, this, message, flag).Show();
        }

        private void LogSystemError(string action, string errorText, string inputParams)
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