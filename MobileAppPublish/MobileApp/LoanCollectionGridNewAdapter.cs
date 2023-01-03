

using System;
using System.Collections.Generic;
using System.Linq;

using Android.App;
using Android.Content;
using Android.Text;
using Android.Views;
using Android.Widget;
using PMS.Droid.Classes.OffLineHelpers;

namespace PMS.Droid
{

    [Activity(Label = "LoanCollectionGridNewAdapter")]
    public partial class LoanCollectionGridNewAdapter : BaseAdapter<MemberProductModel>
    {
        IList<MemberProductModel> loanProducts;
        private LayoutInflater mInflater;
        private Context activity;
        private ListView listView;
        TextView txtTotalMember;

        TextView txtTotalCollectionMember;
        TextView txtTotalRecoverable;
        TextView txtTotalLoanCollection;
        TextView txtTotalGSCollection;
        TextView txtTotalVSCollection;
        TextView txtTotalLTSCollection;
        public LoanCollectionGridNewAdapter(Context context,
                                                IList<MemberProductModel> results, ListView lv)
        {
            this.activity = context;
            loanProducts = results;
            
            mInflater = (LayoutInflater)activity.GetSystemService(Context.LayoutInflaterService);
            this.listView = lv;
        }

        public override int Count
        {
            get { return loanProducts.Count; }
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override MemberProductModel this[int position]
        {
            get { return loanProducts[position]; }
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            CollectionRowViewHolder holder = null;
            if (convertView == null)
            {
                convertView = mInflater.Inflate(Resource.Layout.LoanCollectionGridNew, null, false);

                holder = new CollectionRowViewHolder();
                holder.tblRwHeader = convertView.FindViewById<TableRow>(Resource.Id.tblRwHeader);
                holder.tblRwData = convertView.FindViewById<TableRow>(Resource.Id.tblRwData);
                holder.txtProductHeader = convertView.FindViewById<TextView>(Resource.Id.txtProductHeader);
                
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
                holder.cbkCollect = convertView.FindViewById<CheckBox>(Resource.Id.cbkCollect);
                convertView.Tag = holder;                
                holder.txtProductHeader.Tag = position;
                holder.txtLoanProductAmountCollection.Tag = position;
                holder.txtGS.Tag = position;
                holder.txtVS.Tag = position;
                holder.txtLTS.Tag = position;
                holder.cbkCollect.Tag = position;
                var obj = loanProducts[position];
                //Memberid = 0 means header row.
                if (obj.MemberID != 0)
                {
                    holder.txtLoanProductAmountCollection.Text = obj.Recoverable.ToString();
                    holder.txtGS.Text = obj.GSAmount.ToString();
                    holder.txtVS.Text = obj.VSAmount.ToString();
                    holder.txtLTS.Text = obj.LTSAmount.ToString();

                    holder.txtMember.Text = obj.MemberName;
                    holder.txtInstallmentNo.Text = obj.InstallmentNo.ToString();
                    holder.txtPrincipalBalance.Text = obj.PrinBalance.ToString();
                    holder.txtServiceChargeBalance.Text = obj.SerBalance.ToString();
                    holder.txtTotalBalance.Text = obj.Balance.ToString();
                    holder.txtNewDue.Text = obj.NewDue.ToString();

                    var loanTotal = 0.0;
                    var intTotal = 0.0;
                    Calculate(obj.TodayCollectionAmount, obj.Duration, obj.IntDue, obj.LoanDue, obj.PrincipalLoan, obj.LoanRepaid, obj.DurationOverLoanDue, obj.DurationOverIntDue, obj.InstallmentNo, obj.CumInterestPaid, obj.CumIntCharge, obj.InterestCalculationMethod, out loanTotal, out intTotal);
                    //  var txt = string.Format("P: {0} S: {1}", Math.Round(loanTotal), Math.Round(intTotal));
                    holder.txtPrincipalCollection.Text = string.Format("P: {0} ", Math.Round(loanTotal));
                    holder.txtServiceChCollection.Text = string.Format("S: {0}", Math.Round(intTotal));
                    obj.LoanInstallment = loanTotal;
                    obj.IntInstallment = intTotal;
                }

                if (obj.MemberID != 0)
                {
                    holder.tblRwHeader.Visibility = ViewStates.Gone;
                    holder.tblRwData.Visibility = ViewStates.Visible;
                    holder.txtProductHeader.Text = "";
                }
                else
                {
                    holder.tblRwHeader.Visibility = ViewStates.Visible;
                    holder.txtProductHeader.Text = obj.ProductName;
                    holder.tblRwData.Visibility = ViewStates.Gone;
                }
                holder.cbkCollect.Checked = obj.IsCollected;
                holder.txtLoanProductAmountCollection.TextChanged += Amount_TextChanged;
                holder.txtGS.TextChanged += GS_TextChanged;
                holder.txtVS.TextChanged += VS_TextChanged;
                holder.txtLTS.TextChanged += LTS_TextChanged;
                holder.cbkCollect.CheckedChange += CbkCollect_CheckedChange;
                   
            }
            else
            {
                holder = convertView.Tag as CollectionRowViewHolder;
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

        private void LTS_TextChanged(object sender, TextChangedEventArgs e)
        {
            var amtEditText = (sender as EditText);
            var val = amtEditText.Text;
            var index = (int)amtEditText.Tag;
            var obj = loanProducts[index];
            obj.LTSCollected = GetAmountFromTextBox(amtEditText);
        }

        private void VS_TextChanged(object sender, TextChangedEventArgs e)
        {
            var amtEditText = (sender as EditText);
            var val = amtEditText.Text;
            var index = (int)amtEditText.Tag;
            var obj = loanProducts[index];
            obj.VSCollected = GetAmountFromTextBox(amtEditText);
        }

        private void GS_TextChanged(object sender, TextChangedEventArgs e)
        {
            var amtEditText = (sender as EditText);
            var val = amtEditText.Text;
            var index = (int)amtEditText.Tag;
            var obj = loanProducts[index];
            obj.GSCollected = GetAmountFromTextBox(amtEditText);
        }

        private void CbkCollect_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            var cbx = (sender as CheckBox);
            var position = (int)cbx.Tag;
            var obj = loanProducts[position];
            obj.IsCollected = cbx.Checked;

            var visibleIndex = position - listView.FirstVisiblePosition;
            var vw = listView.GetChildAt(visibleIndex);
            var txtGS = vw.FindViewById<EditText>(Resource.Id.txtGS);
            var txtLTS = vw.FindViewById<EditText>(Resource.Id.txtLTS);
            var txtVS = vw.FindViewById<EditText>(Resource.Id.txtVS);
            var txtLoanCollection = vw.FindViewById<EditText>(Resource.Id.txtLoanProductAmountCollection);
            if (!cbx.Checked)
            {
                obj.GSCollected = 0;
                obj.VSCollected = 0;
                obj.LTSCollected = 0;
                obj.TodayCollectionAmount = 0;
                txtGS.Enabled = true;
                txtLTS.Enabled = true;
                txtVS.Enabled = true;
                txtLoanCollection.Enabled = true;
            }
            else
            {  
                obj.GSCollected = GetAmountFromTextBox(txtGS);
                obj.VSCollected = GetAmountFromTextBox(txtLTS); ;
                obj.LTSCollected = GetAmountFromTextBox(txtVS);
                obj.TodayCollectionAmount = GetAmountFromTextBox(txtLoanCollection);
                txtGS.Enabled = false;
                txtLTS.Enabled = false;
                txtVS.Enabled = false;
                txtLoanCollection.Enabled = false;
            }
            UpdateSummary();
        }
        private double GetAmountFromTextBox(EditText textBox)
        {
            var val = textBox.Text;
            var returnVal = 0d;      
            if (!string.IsNullOrEmpty(val))
            {
                double d;
                if (double.TryParse(val, out d))
                    returnVal = d;
            }
            return returnVal;
        }
        private void Amount_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            var amtEditText = (sender as EditText);
            var val = amtEditText.Text;
            var index = (int)amtEditText.Tag;

            var obj = loanProducts[index];           
               obj.TodayCollectionAmount = GetAmountFromTextBox(amtEditText);
            if (obj.ProductType == 0)
                obj.LoanInstallment = obj.TodayCollectionAmount;

            var totalCollection = loanProducts.Sum(s => s.TodayCollectionAmount);
            //if (todayCollectionTextBox != null)
            //    todayCollectionTextBox.Text = totalCollection.ToString();
            var visibleIndex = index - listView.FirstVisiblePosition;
            var vw = listView.GetChildAt(visibleIndex);
            if (vw != null && obj.ProductType != 0)
            {
                var loanTotal = 0.0;
                var intTotal = 0.0;
                Calculate(obj.TodayCollectionAmount, obj.Duration, obj.IntDue, obj.LoanDue, obj.PrincipalLoan, obj.LoanRepaid, obj.DurationOverLoanDue, obj.DurationOverIntDue, obj.InstallmentNo, obj.CumInterestPaid, obj.CumIntCharge, obj.InterestCalculationMethod, out loanTotal, out intTotal);
                obj.LoanInstallment = loanTotal;
                obj.IntInstallment = intTotal;
                // var txt = string.Format("P: {0} S: {1}", Math.Round(loanTotal), Math.Round(intTotal));
                var pc = vw.FindViewById<TextView>(Resource.Id.txtPrincipalCollection);
                pc.Text = string.Format("P: {0} ", Math.Round(loanTotal));
                var intCol = vw.FindViewById<TextView>(Resource.Id.txtServiceChCollection);
                intCol.Text = string.Format("S: {0}", Math.Round(intTotal));
            }
            NotifyDataSetChanged();
        }

        public IList<MemberProductModel> GetAllData()
        {
            return loanProducts;
        }
        private void Calculate(double total, int Duration, double intdue, double loandue,
           double principalLoan, double loanRepaid, double DurationOverLoanDue, double DurationOverIntDue,
           int InstallmentNo, double CumInterestPaid, double CumIntCharge, string calcMethod, out double vLoanInstallment, out double vInterestInstallment
           )
        {
            double vcumInrerestCharge;
            double vcumInrerestPaid;
            vcumInrerestCharge = CumIntCharge;
            vcumInrerestPaid = CumInterestPaid;
            double vInterestBalance = (vcumInrerestCharge - vcumInrerestPaid);

            vLoanInstallment = 0;
            vInterestInstallment = 0;
            double vPrincipalLOan = principalLoan;
            double vloanRepaid = loanRepaid;
            double vLoan = DurationOverLoanDue;
            double vInt = DurationOverIntDue;
            double vLoanDueSCase = DurationOverLoanDue;
            double vIntDueSCase = DurationOverIntDue;
            double vTotalInstall = (vLoan + vInt);
            if (InstallmentNo > Duration)
            {
                vLoan = DurationOverLoanDue;
                vInt = DurationOverIntDue;
                vLoanDueSCase = DurationOverLoanDue;
                vIntDueSCase = DurationOverIntDue;
                vTotalInstall = vLoan + vInt;
                if (total == 0)
                {
                    vLoanInstallment = 0;
                    vInterestInstallment = 0;
                }
                else
                {
                    if (calcMethod == "D")
                    {
                        /////////////////////For LOan And Int equal  0////////////////////////
                        if ((vPrincipalLOan - vloanRepaid) >= total)
                            vLoanInstallment = total;
                        if ((vPrincipalLOan - vloanRepaid) > 0 && (vPrincipalLOan - vloanRepaid) < total)
                            vLoanInstallment = (vPrincipalLOan - vloanRepaid);
                        if ((vPrincipalLOan - vloanRepaid) == 0)
                            vLoanInstallment = 0;
                    }
                    else if (calcMethod == "A" || calcMethod == "R" || calcMethod == "D")
                    {


                        if ((vPrincipalLOan - vloanRepaid) >= total)
                        {

                            vLoanInstallment = total;
                        }
                        if ((vPrincipalLOan - vloanRepaid) > 0 && (vPrincipalLOan - vloanRepaid) < total)
                        {
                            vLoanInstallment = (vPrincipalLOan - vloanRepaid);

                        }
                        if ((vPrincipalLOan - vloanRepaid) == 0)

                            vLoanInstallment = 0;


                    }
                    else if (calcMethod == "E")
                    {

                        if (total > vInt)
                        {
                            vLoanInstallment = (total - vInt);
                        }
                        if (total <= vInt)
                        {
                            vLoanInstallment = 0;
                        }
                    }
                    else
                    {

                        /////////////////////For LOan And Int equal  0////////////////////////

                        if (vLoanDueSCase == 0 && vIntDueSCase > 0)
                        {
                            if (total >= vInterestBalance)
                            {
                                vLoanInstallment = (total - vInterestBalance);
                            }
                            if (total <= vIntDueSCase)
                            {
                                vLoanInstallment = 0;
                            }
                        }
                        else if (vLoanDueSCase == 0 && vIntDueSCase == 0)
                        {
                            if (total > vInterestBalance)
                            {
                                vLoanInstallment = (total - vInterestBalance);
                            }
                            if (total <= vInterestBalance)
                            {
                                vLoanInstallment = 0;
                            }
                        }
                        /////////////////////for General Calculation///////////////////////////////////////
                        else
                        {

                            vLoanInstallment = (vLoan * total) / vTotalInstall;
                        }
                    }

                }
                // loanPaidId = vLoanInstallment;

                if (total == 0)
                {

                    vInterestInstallment = 0;
                }
                else
                {
                    if (calcMethod == "D")
                    {

                        /////////////////////For LOan And Int equal  0////////////////////////
                        if ((vPrincipalLOan - vloanRepaid) >= total)
                        {

                            vInterestInstallment = 0;
                        }
                        //if ((parseFloat(vPrincipalLOan) - parseFloat(vloanRepaid)) > 0 && (parseFloat(vPrincipalLOan) - parseFloat(vloanRepaid)) < (parseFloat(vLoanDueSCase) + parseFloat(vIntDueSCase)))
                        if ((vPrincipalLOan - vloanRepaid) > 0 && (vPrincipalLOan - vloanRepaid) < total)
                        {
                            vInterestInstallment = total - (vPrincipalLOan - vloanRepaid);

                        }
                        if ((vPrincipalLOan - vloanRepaid) == 0)

                            vInterestInstallment = total;


                    }
                    else if (calcMethod == "A" || calcMethod == "R" || calcMethod == "D")
                    {


                        if ((vPrincipalLOan - vloanRepaid) >= total)
                        {

                            vInterestInstallment = 0;
                        }
                        //if ((parseFloat(vPrincipalLOan) - parseFloat(vloanRepaid)) > 0 && (parseFloat(vPrincipalLOan) - parseFloat(vloanRepaid)) < (parseFloat(vLoanDueSCase) + parseFloat(vIntDueSCase)))
                        if ((vPrincipalLOan - vloanRepaid) > 0 && (vPrincipalLOan - vloanRepaid) < total)
                        {
                            vInterestInstallment = total - (vPrincipalLOan - vloanRepaid);

                        }
                        if ((vPrincipalLOan - vloanRepaid) == 0)

                            vInterestInstallment = total;

                    }
                    else if (calcMethod == "E")
                    {
                        if (total > vInt)
                        {
                            vInterestInstallment = vInt;
                        }
                        if (total <= vInt)
                        {
                            vInterestInstallment = total;
                        }
                    }
                    else
                    {
                        /////////////////////For LOan And Int equal  0////////////////////////
                        if (vLoanDueSCase == 0 && vIntDueSCase > 0)
                        {
                            if (total >= vInterestBalance)
                            {
                                vInterestInstallment = vInterestBalance;
                            }
                            if (total <= vIntDueSCase)
                            {
                                vInterestInstallment = total;
                            }
                        }
                        else if (vLoanDueSCase == 0 && vIntDueSCase == 0)
                        {
                            if (total > vInterestBalance)
                            {
                                vInterestInstallment = vInterestBalance;
                            }
                            if (total <= vInterestBalance)
                            {
                                vInterestInstallment = total;
                            }
                        }
                        /////////////////////for General Calculation///////////////////////////////////////
                        else
                        {
                            vInterestInstallment = (vInt * total) / vTotalInstall;
                        }
                    }
                }
                double vLoanTotal = total;
                double vLoanBal;
                if (calcMethod != "A")
                {

                    double vCheck = (vloanRepaid + vLoanInstallment);
                    if (vCheck > vPrincipalLOan)
                    {
                        vInterestInstallment = total - (vPrincipalLOan - vloanRepaid);
                        vLoanInstallment = (vPrincipalLOan - vloanRepaid);

                    }

                    vLoanBal = (vPrincipalLOan + vInterestBalance - vloanRepaid);
                    double calIns = (vloanRepaid + vLoan);
                    if (vLoan >= vLoanBal)
                    {
                        vInterestInstallment = total - (vPrincipalLOan - vloanRepaid);
                        vLoanInstallment = (vPrincipalLOan - vloanRepaid);

                    }

                }
                else if (calcMethod == "A" || calcMethod == "R")
                {

                    vLoanBal = vPrincipalLOan - vloanRepaid;
                    var calIns = vInterestBalance + vLoanBal;
                    if (vLoan >= calIns)
                    {
                        if (calcMethod == "F")
                        {
                            vInterestInstallment = vInterestBalance;
                        }
                        else
                            vInterestInstallment = total - (vPrincipalLOan - vloanRepaid);


                    }
                }


                var vLoanBalance = vPrincipalLOan - vloanRepaid;
                var vBal = vLoanBalance + vInterestBalance;
                if (vBal <= total)
                {

                    if (vInterestBalance > 0)
                    {
                        if (calcMethod == "F")
                        {
                            vInterestInstallment = vInterestBalance;
                        }
                        else
                            vInterestInstallment = total - (vPrincipalLOan - vloanRepaid);



                    }
                    else

                        if (total > vLoanBalance)
                    {
                        if (calcMethod == "F")
                        {
                            vInterestInstallment = vInterestBalance;
                        }
                        else
                            vInterestInstallment = total - (vPrincipalLOan - vloanRepaid);
                    }
                    else
                    {
                        vInterestInstallment = 0;
                        vLoanInstallment = total;
                    }


                }
                if ((vLoanInstallment + vInterestInstallment) > total)
                {
                    vLoanInstallment = total - vInterestInstallment;

                }


                if (calcMethod == "F")
                {
                    if (total > vLoanInstallment + vInterestInstallment)
                    {
                        vLoanInstallment = 0;
                        vInterestInstallment = 0;
                        total = 0;

                    }
                }
            }

            else
            {
                vLoan = loandue;
                vInt = intdue;
                vTotalInstall = vLoan + vInt;
                if (total == 0)
                {
                    vLoanInstallment = 0;
                    vInterestInstallment = 0;
                }
                else
                {

                    if (calcMethod == "D")
                    {
                        /////////////////////For LOan And Int equal  0////////////////////////

                        if (vLoan == 0 && vInt > 0)
                        {

                            if (total > vInterestBalance)
                            {
                                vLoanInstallment = (total - vInterestBalance);
                            }
                            if (total <= vInterestBalance)
                            {
                                vLoanInstallment = 0;
                            }

                        }
                        else if (vLoan == 0 && vInt == 0)
                        {
                            if (total > vInterestBalance)
                            {
                                vLoanInstallment = (total - vInterestBalance);
                            }
                            if (total <= vInterestBalance)
                            {
                                vLoanInstallment = 0;
                            }
                        }

                        /////////////////////for General Calculation///////////////////////////////////////
                        else
                        {

                            if (total < vLoan)
                            {
                                vLoanInstallment = (vLoan * total) / vTotalInstall;
                                // vLoanInstallment = 0;
                            }
                            else
                            {
                                vLoanInstallment = (vLoan * total) / vTotalInstall;
                            }
                        }
                    }

                    else if (calcMethod == "A" || calcMethod == "R" || calcMethod == "V")
                    {

                        /////////////////////For LOan And Int equal  0////////////////////////

                        if (vLoan == 0 && vInt > 0)
                        {
                            if (total > vInt)
                            {
                                vLoanInstallment = (total - vInt);
                            }
                            if (total <= vInt)
                            {
                                vLoanInstallment = 0;
                            }
                        }
                        else if (vLoan == 0 && vInt == 0)
                        {
                            if (total > vInterestBalance)
                            {
                                vLoanInstallment = (total - vInterestBalance);
                            }
                            if (total <= vInterestBalance)
                            {
                                vLoanInstallment = 0;
                            }
                        }
                        /////////////////////for General Calculation///////////////////////////////////////
                        else
                        {
                            if (calcMethod == "A")
                            {
                                if (vInterestBalance > vInt)
                                {
                                    if (total > vInterestBalance)
                                    {
                                        vLoanInstallment = (total - vInterestBalance);
                                    }
                                    if (total <= vInterestBalance)
                                    {
                                        vLoanInstallment = 0;
                                    }
                                    // vLoanInstallment = (parseFloat(total) - parseFloat(vInterestBalance))
                                }

                                else
                                {
                                    if (total > vInt)
                                    {
                                        vLoanInstallment = (total - vInt);
                                    }
                                    if (total <= vInt)
                                    {
                                        vLoanInstallment = 0;
                                    }
                                }
                            }
                            else if (calcMethod == "R" || calcMethod == "V")
                            {
                                if (total > (vLoan + vInt) && total - (vLoan + vInt) > vInterestBalance - vInt)
                                {
                                    if (vInterestBalance > 0)
                                    {
                                        vLoanInstallment = (total - vInterestBalance);

                                    }
                                    else
                                    {
                                        vLoanInstallment = total;
                                    }


                                }
                                else
                                {
                                    if (total < (vLoan + vInt))
                                    {

                                        if (total < vInt)
                                        {
                                            vLoanInstallment = 0;
                                        }
                                        else
                                        {
                                            vLoanInstallment = (total - vInt);
                                        }

                                    }
                                    else
                                        vLoanInstallment = vLoan;

                                }


                            }

                        }



                    }
                    else if (calcMethod == "E")
                    {
                        if (total > vInt)
                        {
                            vLoanInstallment = (total - vInt);
                        }
                        if (total <= vInt)
                        {
                            vLoanInstallment = 0;
                        }
                    }
                    else
                    {

                        /////////////////////For LOan And Int equal  0////////////////////////
                        if (vLoan == 0 && vInt > 0)
                        {
                            if (total >= vInterestBalance)
                            {
                                vLoanInstallment = (total - vInterestBalance);
                            }
                            if (total <= vInt)
                            {
                                vLoanInstallment = 0;
                            }
                        }
                        else if (vLoan == 0 && vInt == 0)
                        {
                            if (total > vInterestBalance)
                            {
                                vLoanInstallment = (total - vInterestBalance);
                            }
                            if (total <= vInterestBalance)
                            {
                                vLoanInstallment = 0;
                            }
                        }
                        /////////////////////for General Calculation///////////////////////////////////////
                        else
                        {

                            vLoanInstallment = (vLoan * total) / vTotalInstall;
                        }



                    }
                }

                if (total == 0)
                {

                    vInterestInstallment = 0;
                }
                else
                {
                    if (calcMethod == "D")
                    {
                        /////////////////////For LOan And Int equal  0////////////////////////

                        if (vLoan == 0 && vInt > 0)
                        {


                            if (total > vInterestBalance)
                            {
                                vInterestInstallment = vInterestBalance;
                            }
                            if (total <= vInterestBalance)
                            {
                                vInterestInstallment = total;
                            }
                        }
                        else if (vLoan == 0 && vInt == 0)
                        {
                            if (total > vInterestBalance)
                            {
                                vInterestInstallment = vInterestBalance;
                            }
                            if (total <= vInterestBalance)
                            {
                                vInterestInstallment = total;
                            }
                        }
                        /////////////////////for General Calculation///////////////////////////////////////
                        else
                        {


                            if (total < vInt)
                            {
                                vInterestInstallment = (vInt * total) / vTotalInstall;

                            }
                            else
                            {
                                vInterestInstallment = (vInt * total) / vTotalInstall;
                            }
                        }
                    }

                    else if (calcMethod == "A" || calcMethod == "R" || calcMethod == "V")
                    {
                        /////////////////////For LOan And Int equal  0////////////////////////

                        if (vLoan == 0 && vInt > 0)
                        {
                            if (total > vInt)
                            {
                                vInterestInstallment = vInt;
                            }
                            if (total <= vInt)
                            {
                                vInterestInstallment = total;
                            }
                        }
                        else if (vLoan == 0 && vInt == 0)
                        {
                            if (total > vInterestBalance)
                            {
                                vInterestInstallment = vInterestBalance;
                            }
                            if (total <= vInterestBalance)
                            {
                                vInterestInstallment = total;
                            }
                        }
                        /////////////////////for General Calculation///////////////////////////////////////
                        else
                        {

                            if (calcMethod == "A")
                            {
                                if (vInterestBalance > vInt)
                                {
                                    if (total > vInterestBalance)
                                    {
                                        vInterestInstallment = vInterestBalance;
                                    }
                                    if (total <= vInterestBalance)
                                    {
                                        vInterestInstallment = total;
                                    }
                                    //vInterestInstallment = parseFloat(vInterestBalance)
                                }
                                else
                                {
                                    if (total > vInt)
                                    {
                                        vInterestInstallment = vInt;
                                    }
                                    if (total <= vInt)
                                    {
                                        vInterestInstallment = total;
                                    }
                                }
                            }
                            else if (calcMethod == "R" || calcMethod == "V")
                            {
                                if (total > (vLoan + vInt) && total - (vLoan + vInt) > (vInterestBalance - vInt))
                                {

                                    if (vInterestBalance > 0)
                                    {
                                        vInterestInstallment = vInterestBalance;
                                    }
                                    else
                                    {
                                        vInterestInstallment = 0;
                                    }


                                }
                                else
                                {
                                    if (total < (vLoan + vInt))
                                    {
                                        if (total < vInt)
                                        { vInterestInstallment = total; }
                                        else
                                        {
                                            vInterestInstallment = vInt;
                                        }

                                    }
                                    else
                                    {
                                        vInterestInstallment = (total - vLoan);
                                    }

                                }
                            }

                        }

                    }
                    else if (calcMethod == "E")
                    {
                        if (total > vInt)
                        {
                            vInterestInstallment = vInt;
                        }
                        if (total <= vInt)
                        {
                            vInterestInstallment = total;
                        }
                    }
                    else
                    {

                        /////////////////////For LOan And Int equal  0////////////////////////

                        if (vLoan == 0 && vInt > 0)
                        {
                            if (total >= vInterestBalance)
                            {
                                vInterestInstallment = vInterestBalance;
                            }

                            if (total < vInterestBalance)
                            {
                                vInterestInstallment = total;
                            }
                            if (total <= vInt)
                            {
                                vInterestInstallment = total;
                            }
                        }
                        else if (vLoan == 0 && vInt == 0)
                        {
                            if (total > vInterestBalance)
                            {
                                vInterestInstallment = vInterestBalance;
                            }
                            if (total <= vInterestBalance)
                            {
                                vInterestInstallment = total;
                            }
                        }
                        /////////////////////for General Calculation///////////////////////////////////////
                        else
                        {

                            vInterestInstallment = (vInt * total) / vTotalInstall;
                        }
                    }
                }

                if (calcMethod != "A")
                {
                    double vCheck = (vloanRepaid + vLoanInstallment);
                    if (vCheck > vPrincipalLOan)
                    {
                        if (calcMethod == "F")
                        {
                            vInterestInstallment = vInterestBalance;
                        }
                        else
                        {
                            vInterestInstallment = total - (vPrincipalLOan - vloanRepaid);


                            vLoanInstallment = (vPrincipalLOan - vloanRepaid);
                        }

                    }

                    double vLoanBal = (vPrincipalLOan + vInterestBalance - vloanRepaid);
                    double calIns = (vloanRepaid + vLoan);
                    if (vLoan >= vLoanBal)
                    {
                        if (calcMethod == "F")
                        {
                            vInterestInstallment = vInterestBalance;
                            vLoanInstallment = (total - vInterestBalance);

                        }
                        else
                        {
                            vInterestInstallment = total - (vPrincipalLOan - vloanRepaid);
                            vLoanInstallment = (vPrincipalLOan - vloanRepaid);
                        }

                    }
                }
                else if (calcMethod == "A" || calcMethod == "R" || calcMethod == "V")
                {
                    double vLoan1 = total;
                    double vLoanBal = vPrincipalLOan - vloanRepaid;
                    double calIns = (vloanRepaid + vLoan);
                    if (calIns >= vPrincipalLOan)
                    {
                        vLoanInstallment = (total - vInterestInstallment);

                    }
                }

                double vLoanBalance = vPrincipalLOan - vloanRepaid;
                double vBal = (vLoanBalance + vInterestBalance);
                if (vBal <= total)
                {

                    if (vInterestBalance > 0)
                    {
                        if (calcMethod == "F")
                        {
                            vInterestInstallment = vInterestBalance;
                            if ((total - vInterestBalance) >= (vPrincipalLOan - vloanRepaid))
                            {
                                vLoanInstallment = (total - vInterestBalance);
                            }


                        }
                        else
                        {
                            vInterestInstallment = total - (vPrincipalLOan - vloanRepaid);
                            vLoanInstallment = (vPrincipalLOan - vloanRepaid);
                        }


                    }
                    else
                        if (total > vLoanBalance)
                    {
                        if (calcMethod == "F")
                        {
                            vInterestInstallment = vInterestBalance;
                        }
                        else
                        {
                            vInterestInstallment = total - (vPrincipalLOan - vloanRepaid);
                            vLoanInstallment = (vPrincipalLOan - vloanRepaid);
                        }
                    }
                    else
                    {
                        vInterestInstallment = 0;
                        vLoanInstallment = total;
                    }


                }

                if ((vLoanInstallment + vInterestInstallment) > total)
                {
                    vLoanInstallment = total - vInterestInstallment;

                }
                if (calcMethod == "A" || calcMethod == "H")
                {


                    double vLoanPayable = (total - vInterestInstallment);
                    if (vLoanPayable > vPrincipalLOan)
                    {
                        double NeatLoanPay = total - vInterestBalance;
                        vLoanInstallment = NeatLoanPay;
                        double intPay = total - NeatLoanPay;
                        vInterestInstallment = intPay;
                    }
                    else
                    {
                        vLoanInstallment = vLoanPayable;

                    }

                }

                if (calcMethod == "F")
                {
                    if (vInterestInstallment > vInterestBalance)
                    {
                        vInterestInstallment = vInterestBalance;
                        vLoanInstallment = total - vInterestBalance;

                    }

                }




                if (calcMethod == "F")
                {
                    if (total > (vInterestBalance + vPrincipalLOan - vloanRepaid))
                    {
                        total = 0;
                        vLoanInstallment = 0;
                        vInterestInstallment = 0;


                    }
                }
            }
        }
        public class CollectionRowViewHolder : Java.Lang.Object
        {
            public EditText txtLoanProductAmountCollection { get; set; }
            public EditText txtGS { get; set; }
            public EditText txtVS { get; set; }
            public EditText txtLTS { get; set; }
            public TableRow tblRwHeader { get; set; }
            public TableRow tblRwData { get; set; }
            public TextView txtProductHeader { get; set; }

            public TextView txtMember { get; set; }
            public TextView txtPrincipalCollection { get; set; }
            public TextView txtServiceChCollection { get; set; }
            public TextView txtInstallmentNo { get; set; }
            public TextView txtPrincipalBalance { get; set; }
            public TextView txtServiceChargeBalance { get; set; }
            public TextView txtTotalBalance { get; set; }
            public TextView txtNewDue { get; set; }
            public CheckBox cbkCollect { get; set; }

        }
        public void SetTotalRowFields(TextView txtTotalMember, TextView txtTotalCollectionMember,
            TextView txtTotalRecoverable, TextView txtTotalLoanCollection, TextView txtTotalGSCollection,
            TextView txtTotalVSCollection, TextView txtTotalLTSCollection)
        {
            this.txtTotalMember = txtTotalMember;
            this.txtTotalCollectionMember = txtTotalCollectionMember;
            this.txtTotalRecoverable = txtTotalRecoverable;
            this.txtTotalLoanCollection = txtTotalLoanCollection;
            this.txtTotalGSCollection = txtTotalGSCollection;
            this.txtTotalVSCollection = txtTotalVSCollection;
            this.txtTotalLTSCollection = txtTotalLTSCollection;
        }
        public void UpdateSummary()
        {
            var totalMember = loanProducts.Where(w => w.MemberID != 0).Select(s => s.MemberID).Distinct().Count();
            var totalMemberCollected = loanProducts.Where(w => w.MemberID != 0 && w.IsCollected).Select(s => s.MemberID).Distinct().Count();
            var totalRecoverable = loanProducts.Sum(s => s.Recoverable);
            var totalLoanCollected = loanProducts.Where(w=>w.IsCollected).Sum(s => s.TodayCollectionAmount);
            var totalGSCollected = loanProducts.Where(w => w.IsCollected).Sum(s => s.GSCollected);
            var totalVSCollected = loanProducts.Where(w => w.IsCollected).Sum(s => s.VSCollected);
            var totalLTSCollected = loanProducts.Where(w => w.IsCollected).Sum(s => s.LTSCollected);
            this.txtTotalMember.Text = string.Format("TotM: {0}", totalMember);
            this.txtTotalCollectionMember.Text = string.Format("ColMem: {0}", totalMemberCollected);
            this.txtTotalRecoverable.Text = string.Format("RecL: {0}", totalRecoverable);
            this.txtTotalLoanCollection.Text = string.Format("ColL: {0}", totalLoanCollected);
            this.txtTotalGSCollection.Text = string.Format("GS: {0}", totalGSCollected);
            this.txtTotalVSCollection.Text = string.Format("VS: {0}", totalVSCollected);
            this.txtTotalLTSCollection.Text = string.Format("LTS: {0}", totalLTSCollected);
        }
    }
}