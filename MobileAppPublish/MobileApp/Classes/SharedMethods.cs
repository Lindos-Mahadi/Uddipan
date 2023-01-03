using Android.Content;
using Android.Preferences;
using PMS.Droid.Classes.OffLineHelpers;

namespace PMS.Droid.Classes
{
    public class SharedMethods
    {
        public static string GetOrganizationUrl(Context ctx)
        {
            var urlhelper = new OrganizationUrlOfflineHelper(ctx);
            var urlObj = urlhelper.Get();
            if (urlObj != null)
            {
                var url = urlObj.OrganizationUrl;
                if (!url.EndsWith("/"))
                    url = url + "/";
                ///api/{0}
                ///
                url = url + "api/{0}";
                return url;
            }
            else
            {
                return "";
            }
        }
        public static string GetOrganizationBaseUrl(Context ctx)
        {
            var urlhelper = new OrganizationUrlOfflineHelper(ctx);
            var urlObj = urlhelper.Get();
            if (urlObj != null)
            {
                var url = urlObj.OrganizationUrl;
                if (!url.EndsWith("/"))
                    url = url + "/";
                return url;
            }
            else
            {
                return "";
            }
        }

        public static string GetUploadUrl(Context ctx,string defaultUrl)
        {
            ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(ctx);
            string strUrl = prefs.GetString("uploadUrl", defaultUrl);
            return strUrl;
        }
    }
}