using System;

using Android.App;
using Android.Content;
using Android.OS;
using System.Threading.Tasks;
using PMS.Droid.Classes.OffLineHelpers;

namespace PMS.Droid
{
    [Activity(MainLauncher = true, Label = "gBanker Plus", NoHistory = true, Theme = "@android:style/Theme.Light", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class WelcomeActivity : Activity
    {
        protected async override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Welcome);
            CheckDbVersion();
            Title = "Welcome";
            await StartMenuActivity();
            // Create your application here
        }
        private async Task StartMenuActivity()
        {
            try
            {
                await Task.Delay(3000); //60 minutes 
               // CheckDbVersion();
                Intent intent = null;
                var userHelper = new UserOfflineHelper(this);
                var currentUserId = userHelper.GetUserID();
                if (string.IsNullOrEmpty(currentUserId))
                    intent = new Intent(this, typeof(UserHomeActivity));
                else
                    intent = new Intent(this, typeof(LoginActivity));

                StartActivity(intent);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
        private void CheckDbVersion()
        {
            var offlineUpgradeHelper = new OfflineDbUpgradeHelper(this);
            var db = offlineUpgradeHelper.WritableDatabase;
        }
    }
}