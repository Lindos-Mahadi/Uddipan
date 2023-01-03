using System;
using System.Collections.Generic;

using Android.App;
using Android.Content;
using Android.Views;
using Android.Widget;
using PMS.Droid.Classes.OffLineHelpers;

namespace PMS.Droid
{
    [Activity(Label = "LoanProposalSyncAdapter")]
    public partial class LoanProposalSyncAdapter : BaseAdapter<LoanProposalModel>
    {
        IList<LoanProposalModel> proposalList;
        private LayoutInflater mInflater;
        private Context activity;
        public LoanProposalSyncAdapter(Context context,
                                                IList<LoanProposalModel> results)
        {
            this.activity = context;
            proposalList = results;
            mInflater = (LayoutInflater)activity.GetSystemService(Context.LayoutInflaterService);
        }

        public override int Count
        {
            get { return proposalList.Count; }
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override LoanProposalModel this[int position]
        {
            get { return proposalList[position]; }
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            ImageView btnDelete;
            ContactsViewHolder holder = null;
            if (convertView == null)
            {
                convertView = mInflater.Inflate(Resource.Layout.SyncProposalGridHolder, null);
                holder = new ContactsViewHolder();

                holder.txtMemberCode = convertView.FindViewById<TextView>(Resource.Id.lr_MemberCode);
                holder.txtAmount = convertView.FindViewById<TextView>(Resource.Id.lr_Amount);
                holder.txtProduct = convertView.FindViewById<TextView>(Resource.Id.lr_Product);
                holder.txtCenter = convertView.FindViewById<TextView>(Resource.Id.lr_Center);
                btnDelete = convertView.FindViewById<ImageView>(Resource.Id.lr_deleteBtn);


                btnDelete.Click += (object sender, EventArgs e) =>
                {

                    AlertDialog.Builder builder = new AlertDialog.Builder(activity);
                    AlertDialog confirm = builder.Create();
                    confirm.SetTitle("Confirm Delete");
                    confirm.SetMessage("Are you sure delete?");
                    confirm.SetButton("OK", (s, ev) =>
                    {
                        var poldel = (int)((sender as ImageView).Tag);

                        var id = proposalList[poldel].ProposalID;                       

                        proposalList.RemoveAt(poldel);

                        DeleteSelectedContact(id);
                        NotifyDataSetChanged();

                        Toast.MakeText(activity, "Loan Proposal Deleted Successfully", ToastLength.Short).Show();
                    });
                    confirm.SetButton2("Cancel", (s, ev) =>
                    {

                    });

                    confirm.Show();
                };

                convertView.Tag = holder;
                btnDelete.Tag = position;
            }
            else
            {
                btnDelete = convertView.FindViewById<ImageView>(Resource.Id.lr_deleteBtn);
                holder = convertView.Tag as ContactsViewHolder;
                btnDelete.Tag = position;
            }
            holder.txtCenter.Text = proposalList[position].CenterName;
            holder.txtMemberCode.Text = proposalList[position].MemberCode.ToString();
            holder.txtAmount.Text = proposalList[position].Amount.ToString();
            holder.txtProduct.Text = proposalList[position].ProductName;

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

        public IList<LoanProposalModel> GetAllData()
        {
            return proposalList;
        }

        public class ContactsViewHolder : Java.Lang.Object
        {
            public TextView txtMemberCode { get; set; }
            public TextView txtAmount { get; set; }
            public TextView txtProduct { get; set; }
            public TextView txtCenter { get; set; }
        }

        private void DeleteSelectedContact(int id)
        {
            var _db = new LoanProposalOfflineHelper(activity);
            _db.DeleteProposal(id);
        }
    }
}

