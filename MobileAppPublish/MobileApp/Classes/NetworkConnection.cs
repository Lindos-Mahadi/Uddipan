
using Android.App;
using Android.Content;
using Android.Net;

namespace PMS.Droid.Helpers
{
    public class NetworkConnection
    {
        private Context context;

        public NetworkConnection(Context context)
        {
            this.context = context;
        }

        public static bool IsConnected { get; set; }

        public bool CheckNetworkConnection()
        {
            Activity activity = (Activity)context;
            ConnectivityManager connectivityManager = (ConnectivityManager)context.GetSystemService("connectivity");
            NetworkInfo activeConnection = connectivityManager.ActiveNetworkInfo;

            if (activeConnection != null && activeConnection.IsConnected)
            {
                IsConnected = true;
            }
            else
            {
                IsConnected = false;
                //new CustomMessage(context, activity, ResourceConstants.ConnectionMessage, false).Show();
                //CommonHelper.Done(context);
            }
            return IsConnected;
        }
    }
}