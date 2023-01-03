using System.Collections.Generic;

using Android.App;
using Android.Content;
using Android.Views;
using Android.Widget;
using PMS.Droid.Classes.OffLineHelpers;
using Android.Graphics;

namespace PMS.Droid
{

    [Activity(Label = "CollectionSummaryGridAdapter")]
    public partial class CollectionSummaryGridAdapter : BaseAdapter<LoanCollectionSummaryModel>
    {
        IList<LoanCollectionSummaryModel> collections;
        private LayoutInflater mInflater;
        private Context activity;
        public CollectionSummaryGridAdapter(Context context,
                                                IList<LoanCollectionSummaryModel> results)
        {
            this.activity = context;
            collections = results;
            mInflater = (LayoutInflater)activity.GetSystemService(Context.LayoutInflaterService);
        }

        public override int Count
        {
            get { return collections.Count; }
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override LoanCollectionSummaryModel this[int position]
        {
            get { return collections[position]; }
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            if (convertView == null)
            {
                convertView = mInflater.Inflate(Resource.Layout.CollectionSummaryGrid, null);

                var centerName = convertView.FindViewById<TextView>(Resource.Id.lr_Center);
                var productName = convertView.FindViewById<TextView>(Resource.Id.lr_ProductCode);
                var recoverable = convertView.FindViewById<TextView>(Resource.Id.lr_Receivable);
                var collection = convertView.FindViewById<TextView>(Resource.Id.lr_Collection);
                var due = convertView.FindViewById<TextView>(Resource.Id.lr_Due);

                centerName.Text = collections[position].CenterName;
                productName.Text = collections[position].ProductName;
                recoverable.Text = collections[position].Receivable;
                collection.Text = collections[position].Collection;
                due.Text = collections[position].Due;

                if (collections[position].RecordType == "D")
                {
                    centerName.SetTextColor(Color.Blue);
                    productName.SetTextColor(Color.Blue);
                    recoverable.SetTextColor(Color.Blue);
                    collection.SetTextColor(Color.Blue);
                    due.SetTextColor(Color.Blue);
                }
                else if (collections[position].RecordType == "G")
                {
                    productName.SetTextColor(Color.Brown);
                    // productName.SetTextSize(Android.Util.ComplexUnitType.Px, 20);
                    recoverable.SetTextColor(Color.Brown);
                    collection.SetTextColor(Color.Brown);
                    due.SetTextColor(Color.Brown);

                    var boldTypeface = Typeface.DefaultFromStyle(TypefaceStyle.Bold);
                    centerName.SetTypeface(boldTypeface, TypefaceStyle.Bold);
                    productName.SetTypeface(boldTypeface, TypefaceStyle.Bold);
                    recoverable.SetTypeface(boldTypeface, TypefaceStyle.Bold);
                    collection.SetTypeface(boldTypeface, TypefaceStyle.Bold);
                    due.SetTypeface(boldTypeface, TypefaceStyle.Bold);
                }
                else if (collections[position].RecordType == "S")
                {
                    productName.SetTextColor(Color.Black);
                    recoverable.SetTextColor(Color.Black);
                    collection.SetTextColor(Color.Black);
                    due.SetTextColor(Color.Black);

                }
                if (collections[position].DueAmount > 0)
                {
                    due.SetTextColor(Color.Red);
                }
            }

            if (position % 2 == 0)
            {
                convertView.SetBackgroundResource(Resource.Drawable.list_selector);
            }
            else
            {
                convertView.SetBackgroundResource(Resource.Drawable.list_selector_alternate);
            }

            return convertView;
        }


        public IList<LoanCollectionSummaryModel> GetAllData()
        {
            return collections;
        }
    }
}