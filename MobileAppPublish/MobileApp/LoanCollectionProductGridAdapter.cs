using System;
using System.Collections.Generic;
using System.Linq;

using Android.App;
using Android.Content;
using Android.Text;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using Java.Lang;
using PMS.Droid.Classes;
using PMS.Droid.Classes.OffLineHelpers;
using PMS.Droid.Helpers;
using static Android.Views.View;

namespace PMS.Droid
{

    [Activity(Label = "LoanCollectionProductGridAdapter")]
    public partial class LoanCollectionProductGridAdapter : BaseAdapter
    {
        public IList<MemberProductModel> loanProducts;
        private LayoutInflater mInflater;
        private static Activity activity;
        private static TextView todayCollectionTextBox;
        private static ListView listView;
        static Dialog dialog;
        static bool isSet = false;
        //KHALID: Local Variable

        private static double loanTotalEv = 2.0;
             
        private static double intTotalEv = 2.0;
        private static View VisibleIndex_Current;
        private static EditText CurrentAmountField;
        private static List<string> lastValues;
            //
        public LoanCollectionProductGridAdapter(Activity context,
                                                IList<MemberProductModel> results, TextView _todayCollectionTextBox, ListView lv)
        {
            activity = context;
            todayCollectionTextBox = _todayCollectionTextBox;
            loanProducts = results;
            //for (int i = 0; i < loanProducts.Count; i++)
            //{
            //    Console.WriteLine("PRODUCTS: " + loanProducts[i].ProductName);
            //}
            mInflater = (LayoutInflater)activity.GetSystemService(Context.LayoutInflaterService);
            listView = lv;
        }

        public override int Count
        {
            get { return loanProducts.Count; }
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            LoanCollectionProductGridHolder holder = null;
            var obj = loanProducts[position];
            View row = convertView;
            if (row == null)
            {
                lastValues = new List<string>();
                row = mInflater.Inflate(Resource.Layout.LoanCollectionProductGridHolder, null);
                holder = new LoanCollectionProductGridHolder(position,row, this);
                
                row.Tag = holder;
                //holder.txtLoanProductFine.Tag = position;
            }
            else
            {
                //holder = new LoanCollectionProductGridHolder(position, row, this);
                holder = row.Tag as LoanCollectionProductGridHolder;
                

            }

            holder.Amount.Tag = position;
            holder.txtLoanProductFine.Tag = position;
            
            var type = obj.ProductType == 0 ? "Savings" : "Loan";
            //holder.RecoverableTitle.Text = obj.ProductType == 0 ? "Deposit:" : "Recoverable:";
            holder.RecoverableTitle.Text = obj.ProductType == 0 ? "Installment:" : "Recoverable:"; // KHALID

            holder.Product.Text = string.Format("{0} ({1})", obj.ProductName, type);
            // holder.ProductType.Text = obj.ProductType == 0 ? "Savings" : "Loan";
            holder.Balance.Text = obj.Balance.ToString();
            holder.InstallmentNo.Text = obj.InstallmentNo.ToString();
            holder.Recoverable.Text = obj.Recoverable.ToString();
            
            holder.PrincipalBalance.Text = obj.PrinBalance.ToString();
            holder.ServiceChargeBalance.Text = obj.SerBalance.ToString();

            holder.KNewDue.Text = obj.NewDue.ToString();
            holder.KSCPaid.Text = obj.SCPaid.ToString();

            holder.accountNoVal.Text = (obj.accountNo != null) ? obj.accountNo.ToString() : "0";
            holder.fineTxtCaptionVal.Text = obj.fine.ToString();
            holder.Amount.Text = obj.TodayCollectionAmount.ToString();

            //holder.Amount.TextChanged += Amount_TextChanged;
            //holder.Amount.FocusChange += Amount_FocusChanged;
            //holder.Amount.Tag = position;

            //for saings, change label texts..
            if (obj.ProductType == 0)
                {
                
                    if (obj.ProductName.Substring(0, 2) == "23")
                    {
                        holder.txtLoanProductFine.Visibility = Android.Views.ViewStates.Visible;
                        holder.txtFineCaption.Visibility = Android.Views.ViewStates.Visible;
                        holder.PrincipalCollection.Visibility = Android.Views.ViewStates.Gone;
                    holder.ServiceChCollection.Visibility = Android.Views.ViewStates.Gone;
                }
                else
                {
                    holder.txtLoanProductFine.Visibility = Android.Views.ViewStates.Invisible;
                    holder.txtFineCaption.Visibility = Android.Views.ViewStates.Invisible;
                }
                    holder.TotalBalanceLabel.Text = "Sav. Bal:";

                    holder.PrincipalBalanceLabel.Text = "Deposit: ";
                    holder.PrincipalBalance.Text = obj.PersonalSaving.ToString();

                    holder.KNewDueLabel.Text = "APP.Interest: ";
                    holder.KNewDue.Text = obj.NewDue.ToString();   // old value



                    holder.KSCPaidLabel.Text = "P.Withdraw: "; 
                    holder.KSCPaid.Text = obj.PersonalWithdraw.ToString();

                     


                    //holder.ServiceChargeBalanceLabel.Text = "Sav. Int:";
                    holder.ServiceChargeBalanceLabel.Text = "CUR. Int:"; //KHALID
                    // holder.ServiceChargeBalance.Text = obj.CumIntCharge.ToString();
                    holder.PrincipalCollection.Text = "";
                    holder.ServiceChCollection.Text = "";
                    obj.LoanInstallment = obj.Recoverable;
                }
                else
                {
                holder.PrincipalCollection.Visibility = Android.Views.ViewStates.Visible;
                holder.ServiceChCollection.Visibility = Android.Views.ViewStates.Visible;
                holder.txtLoanProductFine.Visibility = Android.Views.ViewStates.Invisible;
                holder.txtFineCaption.Visibility = Android.Views.ViewStates.Invisible;
                holder.TotalBalanceLabel.Text = "Tot. Balance:";
                    holder.PrincipalBalanceLabel.Text = "Prin. Balance: ";
                    holder.PrincipalBalance.Text = obj.PrinBalance.ToString();
                    holder.KNewDueLabel.Text = "Due/O.Due: ";
                    holder.KNewDue.Text = obj.NewDue.ToString();
                    holder.KSCPaidLabel.Text = "SC Paid: ";
                    holder.KSCPaid.Text = obj.SCPaid.ToString();
                    holder.ServiceChargeBalanceLabel.Text = "SC Balance:";
                    
                var loanTotal = 0.0;
                    var intTotal = 0.0;
                    var OrgID = obj.OrgID; //SessionHelper.OrgID;
                    HelperMethods.Calculate(obj.TodayCollectionAmount, obj.Duration, obj.IntDue, obj.LoanDue, obj.PrincipalLoan, obj.LoanRepaid, obj.DurationOverLoanDue, obj.DurationOverIntDue, obj.InstallmentNo, obj.CumInterestPaid, obj.CumIntCharge, obj.InterestCalculationMethod, out loanTotal, out intTotal, obj.Doc, obj.OrgID);
                    //  var txt = string.Format("P: {0} S: {1}", Math.Round(loanTotal), Math.Round(intTotal));
                    holder.PrincipalCollection.Text = string.Format("P: {0} ", System.Math.Round(loanTotal)); // WAS :: holder.PrincipalCollection.Text = string.Format("P: {0} ", Math.Round(loanTotal));
                    holder.ServiceChCollection.Text = string.Format("S: {0}", System.Math.Round(intTotal)); // WAS::  holder.ServiceChCollection.Text = string.Format("S: {0}", Math.Round(intTotal));
                    obj.LoanInstallment = loanTotal;
                    obj.IntInstallment = intTotal;
                    
                }

            if (position % 2 == 0)
            {
                row.SetBackgroundResource(Resource.Drawable.list_selector);
            }
            else
            {
                row.SetBackgroundResource(Resource.Drawable.list_selector_alternate);
            }

            return row;
        }

        

            private void Amount_FocusChanged(object sender, FocusChangeEventArgs e)
            {
            try
            {
                bool hasFocus = e.HasFocus;
                if (CurrentAmountField != null)
                {
                    if (loanTotalEv + intTotalEv == 0)
                    {

                        CurrentAmountField.Text = 0.ToString();

                    }
                }
            }
            catch (System.Exception ex)
            {

            }


        } // END of Function
                
            
        public IList<MemberProductModel> GetAllData()
        {
            //var obj = LoanCollectionProductGridAdapter.loanProducts[1];
            return loanProducts;
        }

        public override Java.Lang.Object GetItem(int position)
        {
            return loanProducts[position];
        }


        public class LoanCollectionProductGridHolder : Java.Lang.Object
        {

            LoanCollectionProductGridAdapter baseAdapter;

            public LoanCollectionProductGridHolder(int position, View convertView, LoanCollectionProductGridAdapter adapter)
            {
                this.baseAdapter = adapter;
                this.position = position;
                this.RecoverableTitle = convertView.FindViewById<TextView>(Resource.Id.lr_LoanRecoverableTitle);
                this.Product = convertView.FindViewById<TextView>(Resource.Id.lr_Product);
                // holder.ProductType = convertView.FindViewById<TextView>(Resource.Id.lr_ProductType);
                this.InstallmentNo = convertView.FindViewById<TextView>(Resource.Id.lr_InstallmentNo);
                this.Balance = convertView.FindViewById<TextView>(Resource.Id.lr_Balance);
                this.Recoverable = convertView.FindViewById<TextView>(Resource.Id.lr_LoanRecoverable);
                this.Amount = convertView.FindViewById<EditText>(Resource.Id.txtLoanProductAmountCollection);
                this.PrincipalBalance = convertView.FindViewById<TextView>(Resource.Id.lr_PrincipalBalance);
                this.ServiceChargeBalance = convertView.FindViewById<TextView>(Resource.Id.lr_ServiceChargeBalane);

                this.KNewDue = convertView.FindViewById<TextView>(Resource.Id.KNewDue);
                this.KSCPaid = convertView.FindViewById<TextView>(Resource.Id.KSCPaid);


                this.PrincipalCollection = convertView.FindViewById<TextView>(Resource.Id.txtPrincipalCollection);
                this.ServiceChCollection = convertView.FindViewById<TextView>(Resource.Id.txtServiceChCollection);


                this.PrincipalBalanceLabel = convertView.FindViewById<TextView>(Resource.Id.lr_PrincipalBalanceLabel);
                this.ServiceChargeBalanceLabel = convertView.FindViewById<TextView>(Resource.Id.lr_ServiceChargeBalanceLabel);

                this.KNewDueLabel = convertView.FindViewById<TextView>(Resource.Id.KNewDueLabel);
                this.KSCPaidLabel = convertView.FindViewById<TextView>(Resource.Id.KSCPaidLabel);

                this.TotalBalanceLabel = convertView.FindViewById<TextView>(Resource.Id.lr_TotalBalanceLabel);
                this.accountNoTxtCaption = convertView.FindViewById<TextView>(Resource.Id.accountNoTxtCaption);
                this.accountNoVal = convertView.FindViewById<TextView>(Resource.Id.accountNoVal);
                this.fineTxtCaption = convertView.FindViewById<TextView>(Resource.Id.fineTxtCaption);
                this.fineTxtCaptionVal = convertView.FindViewById<TextView>(Resource.Id.fineTxtCaptionVal);
                this.txtLoanProductFine  = convertView.FindViewById<EditText>(Resource.Id.txtLoanProductFineAmount);
                this.txtFineCaption = convertView.FindViewById<TextView>(Resource.Id.txtFineCaption);
                this.txtLoanProductFine.TextChanged += (object sender, TextChangedEventArgs e) =>
                {
                    var editText1 = (sender as EditText);
                    var val = editText1.Text;
                    var index = (int)editText1.Tag;

                    //Console.WriteLine("THERE:" + index + ", p:" + position);

                    var obj = baseAdapter.loanProducts[index];

                    //Console.WriteLine(obj.ProductName.Substring(0, 2));

                    if (obj.ProductName.Substring(0, 2).Equals("23"))
                    {
                        if (!string.IsNullOrEmpty(val))
                        {

                            double d;
                            if (double.TryParse(val, out d))
                            {
                                //if (index == position)
                                // {
                                if (d <= obj.Recoverable)
                                {
                                    obj.fine = d;
                                    baseAdapter.loanProducts[index] = obj;
                                }
                                else
                                {
                                    obj.fine = d;
                                    baseAdapter.loanProducts[index] = obj;
                                    new CustomMessage(activity.ApplicationContext, activity, "Fine amount should be less then installment", false).Show();
                                }
                                    
                              //  }


                            }

                        }
                    }
                    
                };
                
                this.Amount.FocusChange += (object sender, FocusChangeEventArgs e) =>
                {
                        try
                        {
                            if (CurrentAmountField != null)
                            {
                                if (loanTotalEv + intTotalEv == 0)
                                {

                                   // CurrentAmountField.Text = 0.ToString();

                                }
                            }
                        }
                        catch (System.Exception ex)
                        {

                        }


                    };
                
                this.Amount.TextChanged += (object sender, Android.Text.TextChangedEventArgs e) =>
                {
                    try
                    {
                        
                        var editText = (sender as EditText);
                        var val = editText.Text;

                        var index = (int)editText.Tag;

                        //Console.WriteLine("HERE:" + index + "," + position);
                        
                                var obj = baseAdapter.loanProducts[index];
                                
                                if (!string.IsNullOrEmpty(val))
                                {
                            
                                    double d;
                                    if (double.TryParse(val, out d))
                                    {
                                        
                                        obj.TodayCollectionAmount = d;
                                        if (obj.ProductType == 0)
                                        {
                                            double vSavingInstallment = 0;
                                            HelperMethods.CalculateSavings(obj.Recoverable, obj.TodayCollectionAmount, obj.Recoverable, out vSavingInstallment, obj.OrgID, Int32.Parse(obj.ProductName.Substring(0, 2)));
                                            obj.TodayCollectionAmount = vSavingInstallment;
                                            obj.LoanInstallment = obj.TodayCollectionAmount;

                                        }

                                        var totalCollection = baseAdapter.loanProducts.Sum(s => s.TodayCollectionAmount);
                                        if (todayCollectionTextBox != null)
                                            todayCollectionTextBox.Text = totalCollection.ToString();
                                        var visibleIndex = index - listView.FirstVisiblePosition;
                                        var vw = listView.GetChildAt(visibleIndex);
                                        VisibleIndex_Current = vw;

                                        if (vw != null && obj.ProductType != 0)
                                        {
                                            CurrentAmountField = vw.FindViewById<EditText>(Resource.Id.txtLoanProductAmountCollection);
                                            var loanTotal = 0.0;
                                            var intTotal = 0.0;
                                            var OrgID = obj.OrgID; //SessionHelper.OrgID;
                                            HelperMethods.Calculate(obj.TodayCollectionAmount, obj.Duration, obj.IntDue, obj.LoanDue, obj.PrincipalLoan, obj.LoanRepaid, obj.DurationOverLoanDue, obj.DurationOverIntDue, obj.InstallmentNo, obj.CumInterestPaid, obj.CumIntCharge, obj.InterestCalculationMethod, out loanTotal, out intTotal, obj.Doc, OrgID);
                                            obj.LoanInstallment = loanTotal;
                                            obj.IntInstallment = intTotal;
                                            // var txt = string.Format("P: {0} S: {1}", Math.Round(loanTotal), Math.Round(intTotal));
                                            var pc = vw.FindViewById<TextView>(Resource.Id.txtPrincipalCollection);
                                            pc.Text = string.Format("P: {0} ", System.Math.Round(loanTotal));
                                            var intCol = vw.FindViewById<TextView>(Resource.Id.txtServiceChCollection);
                                            intCol.Text = string.Format("S: {0}", System.Math.Round(intTotal));

                                            loanTotalEv = loanTotal;

                                            intTotalEv = intTotal;


                                        }
                                        baseAdapter.loanProducts[index] = obj;
                                        
                                    }
                                     
                                }
                                
                                
                            
                    }catch(System.Exception ex)
                    {
                        new CustomMessage(activity.ApplicationContext, activity, ex.Message, false).Show();
                        //Toast.MakeText(activity, ex.Message, ToastLength.Long).Show();
                    }
                    
                };

                

                
                
                
            }

            

            public int position { get; set; }

            public TextView Product { get; set; }
            public TextView ProductType { get; set; }
            public TextView InstallmentNo { get; set; }
            public TextView Balance { get; set; }
            public TextView Recoverable { get; set; }
            public TextView RecoverableTitle { get; set; }
            public EditText Amount { get; set; }
            public TextView PrincipalBalance { get; set; }
            public TextView ServiceChargeBalance { get; set; }

            public TextView KNewDue { get; set; }
            public TextView KSCPaid { get; set; }

            public TextView PrincipalCollection { get; set; }
            public TextView PrincipalBalanceLabel { get; set; }
            public TextView ServiceChargeBalanceLabel { get; set; }

            public TextView KNewDueLabel { get; set; }
            public TextView KSCPaidLabel { get; set; }


            public TextView TotalBalanceLabel { get; set; }
            public TextView ServiceChCollection { get; set; }
            public TextView accountNoTxtCaption { get; set; }

            public TextView accountNoVal { get; set; }
            public TextView fineTxtCaption { get; set; }
            public TextView fineTxtCaptionVal { get; set; }
            public EditText txtLoanProductFine { get; set; }
            public TextView txtFineCaption { get; set; }



        }       
    }
}