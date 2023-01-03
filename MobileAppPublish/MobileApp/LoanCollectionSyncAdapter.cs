
using System;
using System.Collections.Generic;
using System.Linq;

using Android.App;
using Android.Content;
using Android.Views;
using Android.Widget;
using PMS.Droid.Classes.OffLineHelpers;
using PMS.Droid.Helpers;

namespace PMS.Droid
{
    [Activity(Label = "LoanCollectionSyncAdapter")]
    public partial class LoanCollectionSyncAdapter : BaseAdapter<LoanCollectionModel>
    {
        IList<LoanCollectionModel> proposalList;
        private LayoutInflater mInflater;
        private Context activity;
       

        public LoanCollectionSyncAdapter(Context context,
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
            ImageView btnDelete;
            ContactsViewHolder holder = null;
            if (convertView == null)
            {
                convertView = mInflater.Inflate(Resource.Layout.SyncCollectionGridHolder, null);
                holder = new ContactsViewHolder();

                holder.txtMemberCode = convertView.FindViewById<TextView>(Resource.Id.lr_MemberCode);
                holder.txtAmount = convertView.FindViewById<TextView>(Resource.Id.lr_Amount);
                holder.txtProduct = convertView.FindViewById<TextView>(Resource.Id.lr_Product);
                holder.txtCenter = convertView.FindViewById<TextView>(Resource.Id.lr_Center);
                btnDelete = convertView.FindViewById<ImageView>(Resource.Id.lr_deleteBtn);


                btnDelete.Click += (object sender, EventArgs e) =>
                {
                    var poldel = (int)((sender as ImageView).Tag);
                    var obj = proposalList[poldel];
                    var msg = string.Format("Are you sure to delete collection for Member: {0}", obj.MemberCode);
                    AlertDialog.Builder builder = new AlertDialog.Builder(activity);
                    AlertDialog confirm = builder.Create();
                    confirm.SetTitle("Confirm Delete");
                    confirm.SetMessage(msg);
                    confirm.SetButton("OK", (s, ev) =>
                    {
                        var delList = new List<long>();
                        foreach (var item in proposalList)
                        {
                            if (item.MemberID == obj.MemberID && item.TrxType == obj.TrxType)
                            {
                                
                                delList.Add(item.CollectionID);
                                Delete(item.CollectionID);
                            }
                        }
                        var newList = proposalList.Where(w => delList.Count(f => f == w.CollectionID) == 0).ToList();
                        proposalList = newList;
                        NotifyDataSetChanged();

                        Toast.MakeText(activity, "Loan Collection Deleted Successfully", ToastLength.Short).Show();
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

