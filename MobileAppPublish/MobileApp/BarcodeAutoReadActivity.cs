using System;
using Android.App;
using Android.OS;
using Android.Text;
using Android.Widget;
using PMS.Droid.Classes.OffLineHelpers;
using ZXing.Mobile;

namespace PMS.Droid
{
    [Activity(Label = "Read Member Passbook", Theme = "@style/Theme.DesignDemo", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class BarcodeAutoReadActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            MobileBarcodeScanner.Initialize(Application);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.BarcodeAutoReadUI);

            // Get our button from the layout resource,
            // and attach an event to it
            var button = FindViewById<ImageButton>(Resource.Id.btnScan);
            var results = FindViewById<TextView>(Resource.Id.Results);

            button.Click += async delegate
            {
                while (true)
                {
                    var scanner = new MobileBarcodeScanner();
                    var result = await scanner.Scan();

                    if (result == null) return;

                    results.Text = result.Text;
                }
            };

        }// End of Oncreate

    }// END of Class
}// END of Namespace