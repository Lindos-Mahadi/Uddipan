
using Android.App;
using Android.Content;

namespace PMS.Droid
{
    public static class CommonHelper
    {
        private static ProgressDialog progress = null;
        
        public static void Busy(Context context)
        {            
            progress = new ProgressDialog(context, ProgressDialog.ThemeHoloLight);
            progress.Indeterminate = true;
            progress.SetProgressStyle(ProgressDialogStyle.Spinner);            
            progress.SetMessage("Loading...");
            progress.SetCancelable(false);
            progress.Show();
        
        /*progress = new ProgressDialog(context, Resource.Style.ProgressTheme);
        progress.Indeterminate = true;
        progress.SetProgressStyle(ProgressDialogStyle.Spinner);            
        progress.SetMessage("Loading.");
        progress.SetCancelable(true);
        progress.Show();*/
        //return progress;
    }
        public static void Done(Context context)
        {
            if (progress != null)
            {
                progress.Dismiss();
                progress.Dispose();
                progress = null;
            }
        }

    }
}