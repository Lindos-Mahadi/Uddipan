using System;
using System.Collections.Generic;

using Android.App;
using Android.Content;
using Android.Views;
using Android.Widget;
using PMS.Droid.Classes.OffLineHelpers;
using Android.Graphics;
using Android.OS;

namespace PMS.Droid
{

    [Activity(Label = "CollectionReportGridAdapter")]
    public partial class CollectionReportGridAdapter : BaseAdapter<LoanCollectionListModel>
    {
        IList<LoanCollectionListModel> collections;
        private LayoutInflater mInflater;
        private Context activity;
        private int centerID;
        private bool isWithdrawal = false;
        public CollectionReportGridAdapter(Context context,
                                                IList<LoanCollectionListModel> results, int centerId, bool isWithdrawal = false)
        {
            this.activity = context;
            collections = results;
            this.centerID = centerId;
            this.isWithdrawal = isWithdrawal;
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

        public override LoanCollectionListModel this[int position]
        {
            get { return collections[position]; }
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            ImageView btnDelete;
            ImageView btnDetails;

            CollectionReportViewHolder holder = null;
            if (convertView == null)
            {
                convertView = mInflater.Inflate(Resource.Layout.CollectionReportGrid, null);
                holder = new CollectionReportViewHolder();
                holder.lblMember = convertView.FindViewById<TextView>(Resource.Id.lblMember);
               
                holder.lblLoan = convertView.FindViewById<TextView>(Resource.Id.lblLoan);
                

                holder.lblSavings = convertView.FindViewById<TextView>(Resource.Id.lblSavings);
                holder.lblDue = convertView.FindViewById<TextView>(Resource.Id.lblDue);
                holder.lblTotal = convertView.FindViewById<TextView>(Resource.Id.lblTotal);
                btnDelete = convertView.FindViewById<ImageView>(Resource.Id.lr_deleteBtnCollectionReport);

                btnDetails = convertView.FindViewById<ImageView>(Resource.Id.lr_KDetailsBtnCollectionReport);

                btnDelete.Click += (object sender, EventArgs e) =>
                {
                    var poldel = (int)((sender as ImageView).Tag);
                    var obj = collections[poldel];
                    var msg = string.Format("Are you sure to delete collection for Member: {0}", obj.MemberName);
                    AlertDialog.Builder builder = new AlertDialog.Builder(activity);
                    AlertDialog confirm = builder.Create();
                    confirm.SetTitle("Confirm Delete");
                    confirm.SetMessage(msg);
                    confirm.SetButton("OK", (s, ev) =>
                    {
                        if (isWithdrawal)
                        {
                            DeleteMemberWithdrawal(obj.MemberID);
                            ReloadGridDataWithdrawal();
                        }
                        else
                        {
                            DeleteMemberCollection(obj.MemberID);
                            ReloadGridData();
                        }
                        NotifyDataSetChanged();
                        Toast.MakeText(activity, "Record Deleted Successfully", ToastLength.Short).Show();
                    });
                    confirm.SetButton2("Cancel", (s, ev) =>
                    {

                    });

                    confirm.Show();
                };


                btnDetails.Click += (object sender, EventArgs e) =>
                {
                    var poldel = (int)((sender as ImageView).Tag);
                    var obj = collections[poldel];
                     
                    var msg = string.Format("Are you sure to View Details for Member: {0}", obj.MemberID);
                    //Toast.MakeText(activity, msg, ToastLength.Short).Show();


                    Bundle b = new Bundle();
                    b.PutLong("MemberId", obj.MemberID); //Member id
                    b.PutString("MemberName", obj.MemberName);
                    //intent.putExtras(b); //Put your id to your next Intent


                   // Intent intent = new Intent(Application.Context, typeof(SpecialCollectionActivity));
                    Intent intent = new Intent(Application.Context, typeof(SpecialCollectionActivityParametered));
                    //intent.PutExtras(b);
                    intent.PutExtras(b);

                    // intent.PutExtra("MemberId", obj.MemberID);



                    Application.Context.StartActivity(intent);

                    //Intent intent = null;
                    //intent = new Intent(this, typeof(SpecialCollectionActivity));
                    // StartActivity(intent);
                };


                    convertView.Tag = holder;
            }
            else
            {
                btnDelete = convertView.FindViewById<ImageView>(Resource.Id.lr_deleteBtnCollectionReport);
                btnDetails = convertView.FindViewById<ImageView>(Resource.Id.lr_KDetailsBtnCollectionReport);

                holder = convertView.Tag as CollectionReportViewHolder;
                btnDelete.Tag = position;
                btnDetails.Tag = position;

            }

            var record = collections[position];
            holder.lblMember.Text = record.MemberName;
            holder.lblLoan.Text = record.LoanAmount;
            holder.lblSavings.Text = record.SavingsAmount;
            holder.lblDue.Text = record.DueAmount;
            holder.lblTotal.Text = record.TotalAmount;

            btnDelete.Tag = position;
            btnDetails.Tag = position;

            if (record.HasDue)
            {
                if (record.RecordType == "H")
                    holder.lblDue.SetTextColor(Color.Black);
                else
                    holder.lblDue.SetTextColor(Color.Red);
            }

            if (record.RecordType == "T") //Transaction ...add some colors into it.
            {
                holder.lblMember.SetTextColor(Color.Blue);
                holder.lblLoan.SetTextColor(Color.Blue);
                holder.lblSavings.SetTextColor(Color.Blue);
                holder.lblTotal.SetTextColor(Color.Blue);
                btnDelete.Visibility = ViewStates.Visible;
                btnDetails.Visibility = ViewStates.Visible;

            }
            else if (record.RecordType == "H" || record.RecordType == "Z")
            {
                holder.lblMember.SetTextColor(Color.Black);
                holder.lblLoan.SetTextColor(Color.Black);
                holder.lblSavings.SetTextColor(Color.Black);
                holder.lblTotal.SetTextColor(Color.Black);
                btnDelete.Visibility = ViewStates.Invisible;
                btnDetails.Visibility = ViewStates.Invisible;
            }

            if (isWithdrawal)
            {
                holder.lblLoan.Visibility = ViewStates.Gone;
                holder.lblDue.Visibility = ViewStates.Gone;
                if (record.RecordType == "H")
                    holder.lblSavings.Text = "Withdraw Amount.";
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


        public IList<LoanCollectionListModel> GetAllData()
        {
            return collections;
        }
        public class CollectionReportViewHolder : Java.Lang.Object
        {
            public TextView lblMember { get; set; }
            public TextView lblLoan { get; set; }
            public TextView lblSavings { get; set; }
            public TextView lblDue { get; set; }
            public TextView lblTotal { get; set; }
            public ImageView lr_deleteBtnCollectionReport { get; set; }
        }

        public void DeleteMemberCollection(long memberID)
        {
            var _db = new LoanCollectionOfflineHelper(activity);
            _db.DeleteMemberCollection(memberID);
        }
        public void DeleteMemberWithdrawal(long memberID)
        {
            var _db = new WithdrawalOfflineHelper(activity);
            _db.DeleteMemberWithdrawal(memberID);
        }
        public void ReloadGridData()
        {
            var _db = new LoanCollectionOfflineHelper(activity);
            collections = _db.GetMemberWiseCollectionSummary(this.centerID);
        }
        public void ReloadGridDataWithdrawal()
        {
            var _db = new WithdrawalOfflineHelper(activity);
            collections = _db.GetMemberWiseWithdrawalSummary(this.centerID);
        }
    }
}