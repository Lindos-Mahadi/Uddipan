using Android.App;
using Android.Content;
using Android.Views;
using Android.Widget;
using Android.Graphics;
using System;
using System.IO;

namespace PMS.Droid
{
    public class CustomMessage : Toast
    {
        private AlertDialog exceptionDialog = null;

        public CustomMessage(Context context)
            :base(context)
        {

        }
       
        public CustomMessage(Context context, Activity activity, string message, bool flag)
            :base(context)
        {
            LayoutInflater inflater = activity.LayoutInflater;

            View view = inflater.Inflate(Resource.Layout.CustomMessage, null);

            ImageView img = view.FindViewById<ImageView>(Resource.Id.imgCustomToast);
            TextView txt = view.FindViewById<TextView>(Resource.Id.txtCustomToast);                        

            if (flag)
            {
                img.SetImageResource(Resource.Drawable.Information);
                view.SetBackgroundColor(Color.White);
            }
            else
            {
                img.SetImageResource(Resource.Drawable.Information);
                view.SetBackgroundColor(Color.LightGray);
                txt.SetTextColor(Color.Red);
            }

            txt.Text = message;            
            SetGravity(GravityFlags.Top | GravityFlags.CenterHorizontal, 0, 0);
            Duration = ToastLength.Long;
            View = view;
        }

        public CustomMessage(Context context, Activity activity, Exception ex) : base(context)
        {
            /*var exSummaryView = LayoutInflater.Inflate(Resource.Layout.ExceptionSummary, null);
            var txtException = exSummaryView.FindViewById<TextView>(Resource.Id.txtException);*/

            WriteToFile(ex.ToString());

            AlertDialog.Builder alert = new AlertDialog.Builder(context);


            //alert.SetView(exSummaryView);
            alert.SetTitle(ex.Message);
            alert.SetMessage(ex.ToString());


            alert.SetNegativeButton("Close", (senderAlert, args) =>
            {
                exceptionDialog.Dismiss();
            });
            exceptionDialog = alert.Create();
            exceptionDialog.Show();
        }

        public void WriteToFile(string content)
        {
            try
            {
                var documents =
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                var filename = System.IO.Path.Combine(documents, "log.txt");
                File.AppendAllText(filename, Environment.NewLine + DateTime.Now + Environment.NewLine + content + Environment.NewLine);
            }
            catch (Exception ex)
            {
                //throw ex;
            }
        }

        public string ReadLogFile()
        {
            try
            {
                var documents =
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                var filename = System.IO.Path.Combine(documents, "log.txt");
                var text = File.ReadAllText(filename);
                return text.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void ClearLogFile()
        {
            try
            {
                var documents =
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                var filename = System.IO.Path.Combine(documents, "log.txt");
                File.WriteAllText(filename, string.Empty);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }        

    }
}