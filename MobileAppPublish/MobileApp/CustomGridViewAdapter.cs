


using System.Collections.Generic;
using System.Linq;
using Android.Content;
using Android.Views;
using Android.Widget;
using Java.Lang;
using PMS.Droid.Classes.OffLineHelpers;
using static PMS.Droid.LoanCollectionGridNewAdapter;
using PMS.Droid.Classes;
using PMS.Droid.Helpers;

namespace PMS.Droid
{
    public class CustomGridViewAdapter : BaseAdapter<MemberProductModel>, IFilterable
    {
        private Context c;
        private List<MemberProductModel> loanProducts;
        private LayoutInflater inflater;
        private string currentPatientId;
        private Filter filter;
        public CustomGridViewAdapter(List<MemberProductModel> loanProducts, Context c)
        {
            this.loanProducts = loanProducts;
            this.c = c;
        }

        public override MemberProductModel this[int position]
        {
            get
            {
                return loanProducts[position];
            }
        }
        /*public override Java.Lang.Object GetItem(int position)
        {
            return patients.Get(position);
        }*/

        public override long GetItemId(int position)
        {
            return position;
        }
        public override int Count
        {
            get
            {
                return loanProducts == null ? -1 : loanProducts.Count;
            }
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {

            if (inflater == null)
            {
                inflater = (LayoutInflater)c.GetSystemService(Context.LayoutInflaterService);
            }
            if (convertView == null)
            {
                convertView = inflater.Inflate(Resource.Layout.CollectoinNewGVLayout, parent, false);
            }


           var holder = new CollectionRowViewHolder();
          //  holder.tblRwHeader = convertView.FindViewById<TableRow>(Resource.Id.tblRwHeader);
           // holder.tblRwData = convertView.FindViewById<TableRow>(Resource.Id.tblRwData);
           // holder.txtProductHeader = convertView.FindViewById<TextView>(Resource.Id.txtProductHeader);

            holder.txtLoanProductAmountCollection = convertView.FindViewById<EditText>(Resource.Id.txtLoanProductAmountCollection);
            holder.txtGS = convertView.FindViewById<EditText>(Resource.Id.txtGS);
            holder.txtVS = convertView.FindViewById<EditText>(Resource.Id.txtVS);
            holder.txtLTS = convertView.FindViewById<EditText>(Resource.Id.txtLTS);

            holder.txtMember = convertView.FindViewById<TextView>(Resource.Id.txtMember);
            holder.txtPrincipalCollection = convertView.FindViewById<TextView>(Resource.Id.txtPrincipalCollection);
            holder.txtServiceChCollection = convertView.FindViewById<TextView>(Resource.Id.txtServiceChCollection);
            holder.txtInstallmentNo = convertView.FindViewById<TextView>(Resource.Id.txtInstallmentNo);
            holder.txtPrincipalBalance = convertView.FindViewById<TextView>(Resource.Id.txtPrincipalBalance);
            holder.txtServiceChargeBalance = convertView.FindViewById<TextView>(Resource.Id.txtServiceChargeBalance);
            holder.txtTotalBalance = convertView.FindViewById<TextView>(Resource.Id.txtTotalBalance);
            holder.txtNewDue = convertView.FindViewById<TextView>(Resource.Id.txtNewDue);

            //convertView.Tag = holder;
            //holder.txtProductHeader.Tag = position;
            holder.txtLoanProductAmountCollection.Tag = position;
            holder.txtGS.Tag = position;
            holder.txtVS.Tag = position;
            holder.txtLTS.Tag = position;

            var obj = loanProducts[position];
            //Memberid = 0 means header row.
            if (obj.MemberID != 0)
            {
                holder.txtLoanProductAmountCollection.Text = obj.Recoverable.ToString();
                holder.txtGS.Text = obj.GSAmount.ToString();
                holder.txtVS.Text = obj.VSAmount.ToString();
                holder.txtLTS.Text = obj.LTSAmount.ToString();

                holder.txtMember.Text = obj.MemberID.ToString();
                holder.txtInstallmentNo.Text = obj.InstallmentNo.ToString();
                holder.txtPrincipalBalance.Text = obj.PrinBalance.ToString();
                holder.txtServiceChargeBalance.Text = obj.SerBalance.ToString();
                holder.txtTotalBalance.Text = obj.Balance.ToString();
                holder.txtNewDue.Text = obj.NewDue.ToString();

                var loanTotal = 0.0;
                var intTotal = 0.0;
                var doc = 0;
                var OrgID = 0;
                OrgID = SessionHelper.OrgID;
                
                HelperMethods.Calculate(obj.TodayCollectionAmount, obj.Duration, obj.IntDue, obj.LoanDue, obj.PrincipalLoan, obj.LoanRepaid, obj.DurationOverLoanDue, obj.DurationOverIntDue, obj.InstallmentNo, obj.CumInterestPaid, obj.CumIntCharge, obj.InterestCalculationMethod, out loanTotal, out intTotal, doc, OrgID);
                //  var txt = string.Format("P: {0} S: {1}", Math.Round(loanTotal), Math.Round(intTotal));
                holder.txtPrincipalCollection.Text = string.Format("P: {0} ", Math.Round(loanTotal));
                holder.txtServiceChCollection.Text = string.Format("S: {0}", Math.Round(intTotal));
                obj.LoanInstallment = loanTotal;
                obj.IntInstallment = intTotal;
            }

            //if (obj.MemberID != 0)
            //{
            //    holder.tblRwHeader.Visibility = ViewStates.Gone;
            //    holder.tblRwData.Visibility = ViewStates.Visible;
            //    holder.txtProductHeader.Text = "";
            //}
            //else
            //{
            //    holder.tblRwHeader.Visibility = ViewStates.Visible;
            //    holder.txtProductHeader.Text = obj.ProductName;
            //    holder.tblRwData.Visibility = ViewStates.Gone;
            //}
            holder.txtLoanProductAmountCollection.TextChanged += Amount_TextChanged;

            return convertView;
        }
        private void Amount_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            var amtEditText = (sender as EditText);
            var val = amtEditText.Text;
            var index = (int)amtEditText.Tag;

            var obj = loanProducts[index];
            if (!string.IsNullOrEmpty(val))
            {
                double d;
                if (double.TryParse(val, out d))
                    obj.TodayCollectionAmount = d;
                else
                    obj.TodayCollectionAmount = 0;
            }
            else
                obj.TodayCollectionAmount = 0;
            if (obj.ProductType == 0)
                obj.LoanInstallment = obj.TodayCollectionAmount;

            var totalCollection = loanProducts.Sum(s => s.TodayCollectionAmount);
            //if (todayCollectionTextBox != null)
            //    todayCollectionTextBox.Text = totalCollection.ToString();
            //var visibleIndex = index - listView.FirstVisiblePosition;
            //var vw = listView.GetChildAt(visibleIndex);
            //if (vw != null && obj.ProductType != 0)
            //{
            //    var loanTotal = 0.0;
            //    var intTotal = 0.0;
            //   HelperMethods.Calculate(obj.TodayCollectionAmount, obj.Duration, obj.IntDue, obj.LoanDue, obj.PrincipalLoan, obj.LoanRepaid, obj.DurationOverLoanDue, obj.DurationOverIntDue, obj.InstallmentNo, obj.CumInterestPaid, obj.CumIntCharge, obj.InterestCalculationMethod, out loanTotal, out intTotal);
            //    obj.LoanInstallment = loanTotal;
            //    obj.IntInstallment = intTotal;
            //    // var txt = string.Format("P: {0} S: {1}", Math.Round(loanTotal), Math.Round(intTotal));
            //    var pc = vw.FindViewById<TextView>(Resource.Id.txtPrincipalCollection);
            //    pc.Text = string.Format("P: {0} ", Math.Round(loanTotal));
            //    var intCol = vw.FindViewById<TextView>(Resource.Id.txtServiceChCollection);
            //    intCol.Text = string.Format("S: {0}", Math.Round(intTotal));
            //}
           // NotifyDataSetChanged();
        }
        public Filter Filter
        {
            get
            {
                return filter;
            }
        }
    }
}