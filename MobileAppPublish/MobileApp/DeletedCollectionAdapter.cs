
using System;
using System.Collections.Generic;
using System.Linq;

using Android.App;
using Android.Content;
using Android.Views;
using Android.Widget;
using PMS.Droid.Classes.OffLineHelpers;

namespace PMS.Droid
{
    [Activity(Label = "DeletedCollectionAdapter")]
    public partial class DeletedCollectionAdapter : BaseAdapter<LoanCollectionModel>
    {
        IList<LoanCollectionModel> proposalList;
        private LayoutInflater mInflater;
        private Context activity;
        public DeletedCollectionAdapter(Context context,
                                                IList<LoanCollectionModel> results)
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

        public override LoanCollectionModel this[int position]
        {
            get { return proposalList[position]; }
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            //ImageView btnDelete;
            ContactsViewHolder holder = null;
            if (convertView == null)
            {
                convertView = mInflater.Inflate(Resource.Layout.DeletedCollectionGridHolder, null);
                holder = new ContactsViewHolder();

                holder.txtMemberCode = convertView.FindViewById<TextView>(Resource.Id.lr_MemberCode);
                holder.txtAmount = convertView.FindViewById<TextView>(Resource.Id.lr_Amount);
                holder.txtProduct = convertView.FindViewById<TextView>(Resource.Id.lr_Product);
                holder.txtCenter = convertView.FindViewById<TextView>(Resource.Id.lr_Center);
                  
                convertView.Tag = holder;
            }
            else
            {
                //btnDelete = convertView.FindViewById<ImageView>(Resource.Id.lr_deleteBtn);
                holder = convertView.Tag as ContactsViewHolder;
                //btnDelete.Tag = position;

            }
            var special = proposalList[position].TrxType == 2 ? "  - (S.C.)" : "";
            holder.txtCenter.Text = proposalList[position].CenterName;
            holder.txtMemberCode.Text = proposalList[position].MemberCode.ToString();
            holder.txtAmount.Text = proposalList[position].Amount.ToString();
            holder.txtProduct.Text = proposalList[position].ProductName + special;

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

        public IList<LoanCollectionModel> GetAllData()
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

        private void Delete(long id)
        {
            var _db = new LoanCollectionOfflineHelper(activity);
            _db.DeleteCollection(id);
        }
    }
}

