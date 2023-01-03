using System.Linq;
using Android.App;
using Android.Views;
using Android.Widget;
using PMS.Droid.Classes.OffLineHelpers;
using System.Collections.Generic;
using PMS.Droid.Classes;
using System;
using Android.Text;
using Android.Graphics;
using PMS.Droid.Helpers;

namespace PMS.Droid
{
    public class LoanCollectionExpandableListAdapter : BaseExpandableListAdapter
    {
        private Activity context;
        private List<string> listDataHeader; // header titles
                                             // child data in format of header title, child title
        private List<CollectionNewModel> loanProducts;
        TextView txtTotalMember;

        TextView txtTotalCollectionMember;
        TextView txtTotalRecoverable;
        TextView txtTotalLoanCollection;
        TextView txtTotalGSCollection;
        TextView txtTotalVSCollection;
        TextView txtTotalLTSCollection;
        //public delegate void ButtonClickedEventHandler(object sender, object data);
        //public event ButtonClickedEventHandler EditButtonClickedEvent;

        public LoanCollectionExpandableListAdapter(Activity context, List<string> listDataHeader, List<CollectionNewModel> listChildData)
        {
            this.context = context;
            this.listDataHeader = listDataHeader;
            this.loanProducts = listChildData;
        }

        //for cchild item view
        public override Java.Lang.Object GetChild(int groupPosition, int childPosition)
        {
            var header = listDataHeader[groupPosition];
            var coll = loanProducts.Where(w => w.ProductName == header).FirstOrDefault();
            return coll.MemberProducts[childPosition];
        }

        public override long GetChildId(int groupPosition, int childPosition)
        {
            return childPosition;
        }

        public override View GetChildView(int groupPosition, int childPosition, bool isLastChild, View convertView, ViewGroup parent)
        {
            var obj = (MemberProductModel)GetChild(groupPosition, childPosition);
            if (convertView == null)
            {
                convertView = context.LayoutInflater.Inflate(Resource.Layout.ProductChild, null);
            }
            var txtMember = convertView.FindViewById<TextView>(Resource.Id.txtMember);
            txtMember.Text = obj.MemberName;

            var txtLoanProductAmountCollection = convertView.FindViewById<EditText>(Resource.Id.txtLoanProductAmountCollection);
            var txtGS = convertView.FindViewById<EditText>(Resource.Id.txtGS);
            var txtVS = convertView.FindViewById<EditText>(Resource.Id.txtVS);
            var txtLTS = convertView.FindViewById<EditText>(Resource.Id.txtLTS);

            var txtPrincipalCollection = convertView.FindViewById<TextView>(Resource.Id.txtPrincipalCollection);
            var txtServiceChCollection = convertView.FindViewById<TextView>(Resource.Id.txtServiceChCollection);
            var txtInstallmentNo = convertView.FindViewById<TextView>(Resource.Id.txtInstallmentNo);
            var txtPrincipalBalance = convertView.FindViewById<TextView>(Resource.Id.txtPrincipalBalance);
            var txtServiceChargeBalance = convertView.FindViewById<TextView>(Resource.Id.txtServiceChargeBalance);
            var txtTotalBalance = convertView.FindViewById<TextView>(Resource.Id.txtTotalBalance);
            var txtNewDue = convertView.FindViewById<TextView>(Resource.Id.txtNewDue);
            var cbkCollect = convertView.FindViewById<CheckBox>(Resource.Id.cbkCollect);
            //Remove handler to remove multiple call of handlers.
         

            var position = new PositionHolder() { GroupPosition = groupPosition, ChildPosition = childPosition };
            txtLoanProductAmountCollection.Tag = position;
            txtGS.Tag = position;
            txtVS.Tag = position;
            txtLTS.Tag = position;
            cbkCollect.Tag = position;
            if (obj.IsCollected)
            {
                txtLoanProductAmountCollection.Text = obj.TodayCollectionAmount.ToString();
                txtGS.Text = obj.GSCollected.ToString();
                txtVS.Text = obj.VSCollected.ToString();
                txtLTS.Text = obj.LTSCollected.ToString();
            }
            else
            {
                txtLoanProductAmountCollection.Text = obj.TodayCollectionAmount > 0 ? obj.TodayCollectionAmount.ToString() : obj.Recoverable.ToString();
                txtGS.Text = obj.GSCollected > 0 ? obj.GSCollected.ToString() : obj.GSAmount.ToString();
                txtVS.Text = obj.VSCollected > 0 ? obj.VSCollected.ToString() : obj.VSAmount.ToString();
                txtLTS.Text = obj.LTSCollected > 0 ? obj.LTSCollected.ToString() : obj.LTSAmount.ToString();
            }
            txtMember.Text = obj.MemberName;
            txtInstallmentNo.Text = obj.InstallmentNo.ToString();
            txtPrincipalBalance.Text = obj.PrinBalance.ToString();
            txtServiceChargeBalance.Text = obj.SerBalance.ToString();
            txtTotalBalance.Text = obj.Balance.ToString();
            txtNewDue.Text = obj.NewDue.ToString();

            var loanTotal = 0.0;
            var intTotal = 0.0;
            var OrgId = SessionHelper.OrgID;
            if (obj.ProductType != 99) //Dummy savings row, no need to calculate
            {
                HelperMethods.Calculate(obj.TodayCollectionAmount, obj.Duration, obj.IntDue, obj.LoanDue, obj.PrincipalLoan, obj.LoanRepaid, obj.DurationOverLoanDue, obj.DurationOverIntDue, obj.InstallmentNo, obj.CumInterestPaid, obj.CumIntCharge, obj.InterestCalculationMethod, out loanTotal, out intTotal, obj.Doc, OrgId);
                //  var txt = string.Format("P: {0} S: {1}", Math.Round(loanTotal), Math.Round(intTotal));
                txtPrincipalCollection.Text = string.Format("P: {0} ", Math.Round(loanTotal));
                txtServiceChCollection.Text = string.Format("S: {0}", Math.Round(intTotal));
                obj.LoanInstallment = loanTotal;
                obj.IntInstallment = intTotal;
            }
            else
                DisableTextBox(txtLoanProductAmountCollection);

            if (obj.GSID == null)
                DisableTextBox(txtGS);
            else
                ResetBackground(txtGS);
            if (obj.VSID == null)
                DisableTextBox(txtVS);
            else
                ResetBackground(txtVS);
            if (obj.LTSID == null)
                DisableTextBox(txtLTS);
            else
                ResetBackground(txtLTS);

            cbkCollect.Checked = obj.IsCollected;

            txtLoanProductAmountCollection.TextChanged -= Amount_TextChanged;
            txtGS.TextChanged -= GS_TextChanged;
            txtVS.TextChanged -= VS_TextChanged;
            txtLTS.TextChanged -= LTS_TextChanged;
            cbkCollect.CheckedChange -= CbkCollect_CheckedChange;
            txtLoanProductAmountCollection.TextChanged += Amount_TextChanged;          
            txtGS.TextChanged += GS_TextChanged;          
            txtVS.TextChanged += VS_TextChanged;          
            txtLTS.TextChanged += LTS_TextChanged;           
            cbkCollect.CheckedChange += CbkCollect_CheckedChange;
            if (!obj.IsCollected)
            {
                EnableTextBox(txtGS);
                EnableTextBox(txtLTS);
                EnableTextBox(txtVS);
                EnableTextBox(txtLoanProductAmountCollection);
                //txtGS.Enabled = false;
                //txtLTS.Enabled = false;
                //txtVS.Enabled = false;
                //txtLoanProductAmountCollection.Enabled = false;
            }
            else
            {
                DisableTextBox(txtGS);
                DisableTextBox(txtLTS);
                DisableTextBox(txtVS);
                DisableTextBox(txtLoanProductAmountCollection);
                //txtGS.Enabled = true;
                //txtLTS.Enabled = true;
                //txtVS.Enabled = true;
                //txtLoanProductAmountCollection.Enabled = true;
            }
           
            return convertView;
        }
        private void DisableTextBox(EditText textbox)
        {
           // textbox.Text = "";
            textbox.Enabled = false;
            textbox.SetRawInputType(InputTypes.Null);
            textbox.SetBackgroundColor(Color.ParseColor("#DEDEDC"));
        }
        private void EnableTextBox(EditText textbox)
        {
           // textbox.Text = "";
            textbox.Enabled = true;
            textbox.SetRawInputType(InputTypes.ClassNumber);
            textbox.SetBackgroundColor(Color.ParseColor("#FFFFFF"));
        }
        private void ResetBackground(EditText textbox)
        {
            textbox.SetBackgroundColor(Color.ParseColor("#FFFFFF"));
        }
        #region Event Handlers
        private void LTS_TextChanged(object sender, TextChangedEventArgs e)
        {
            var amtEditText = (sender as EditText);
            var val = amtEditText.Text;
            var index = (PositionHolder)amtEditText.Tag;
            var obj = GetProduct(index.GroupPosition, index.ChildPosition);
            obj.LTSCollected = GetAmountFromTextBox(amtEditText);
        }

        private void VS_TextChanged(object sender, TextChangedEventArgs e)
        {
            var amtEditText = (sender as EditText);
            var val = amtEditText.Text;
            var index = (PositionHolder)amtEditText.Tag;
            var obj = GetProduct(index.GroupPosition, index.ChildPosition);
            obj.VSCollected = GetAmountFromTextBox(amtEditText);
        }

        private void GS_TextChanged(object sender, TextChangedEventArgs e)
        {
            var amtEditText = (sender as EditText);
            var val = amtEditText.Text;
            var index = (PositionHolder)amtEditText.Tag;
            var obj = GetProduct(index.GroupPosition, index.ChildPosition);
            obj.GSCollected = GetAmountFromTextBox(amtEditText);
        }

       
        private void CbkCollect_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            var cbx = (sender as CheckBox);
            var index = (PositionHolder)cbx.Tag;
            var obj = GetProduct(index.GroupPosition, index.ChildPosition);
            obj.IsCollected = cbx.Checked;

            var vw = cbx.Parent as View;
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

                EnableTextBox(txtGS);
                EnableTextBox(txtLTS);
                EnableTextBox(txtVS);
                EnableTextBox(txtLoanCollection);

                //txtGS.Enabled = true;
                //txtLTS.Enabled = true;
                //txtVS.Enabled = true;
                //txtLoanCollection.Enabled = true;
                DeleteCollection(obj);
            }
            else
            {
                obj.GSCollected = GetAmountFromTextBox(txtGS);
                obj.VSCollected = GetAmountFromTextBox(txtVS); ;
                obj.LTSCollected = GetAmountFromTextBox(txtLTS);
                obj.TodayCollectionAmount = GetAmountFromTextBox(txtLoanCollection);
                DisableTextBox(txtGS);
                DisableTextBox(txtLTS);
                DisableTextBox(txtVS);
                DisableTextBox(txtLoanCollection);

                //txtGS.Enabled = false;
                //txtLTS.Enabled = false;
                //txtVS.Enabled = false;
               // txtLoanCollection.Enabled = false;
                SaveCollection(obj);
            }
            UpdateSummary();
        }

        private MemberProductModel GetProduct(int groupPosition, int childPosition)
        {
            var header = listDataHeader[groupPosition];
            var coll = loanProducts.Where(w => w.ProductName == header).FirstOrDefault();
            return coll.MemberProducts[childPosition];
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
            var index = (PositionHolder)amtEditText.Tag;
            var obj = GetProduct(index.GroupPosition, index.ChildPosition);
            obj.TodayCollectionAmount = GetAmountFromTextBox(amtEditText);
            if (obj.ProductType == 0)
                obj.LoanInstallment = obj.TodayCollectionAmount;

            // var totalCollection = loanProducts.Sum(s => s.TodayCollectionAmount);
            //if (todayCollectionTextBox != null)
            //    todayCollectionTextBox.Text = totalCollection.ToString();
            var vw = amtEditText.Parent as View;
            if (vw != null && obj.ProductType != 0)
            {
                var loanTotal = 0.0;
                var intTotal = 0.0;
                var OrgID = SessionHelper.OrgID; // KHALID
                HelperMethods.Calculate(obj.TodayCollectionAmount, obj.Duration, obj.IntDue, obj.LoanDue, obj.PrincipalLoan, obj.LoanRepaid, obj.DurationOverLoanDue, obj.DurationOverIntDue, obj.InstallmentNo, obj.CumInterestPaid, obj.CumIntCharge, obj.InterestCalculationMethod, out loanTotal, out intTotal, obj.Doc, OrgID);
                obj.LoanInstallment = loanTotal;
                obj.IntInstallment = intTotal;
                // var txt = string.Format("P: {0} S: {1}", Math.Round(loanTotal), Math.Round(intTotal));
                var pc = vw.FindViewById<TextView>(Resource.Id.txtPrincipalCollection);
                pc.Text = string.Format("P: {0} ", Math.Round(loanTotal));
                var intCol = vw.FindViewById<TextView>(Resource.Id.txtServiceChCollection);
                intCol.Text = string.Format("S: {0}", Math.Round(intTotal));
            }
            // NotifyDataSetChanged();
        }

        #endregion
        public override int GetChildrenCount(int groupPosition)
        {
            var header = listDataHeader[groupPosition];
            var coll = loanProducts.Where(w => w.ProductName == header).FirstOrDefault();
            return coll.MemberProducts.Count;
        }
        //For header view
        public override Java.Lang.Object GetGroup(int groupPosition)
        {
            return listDataHeader[groupPosition];
        }
        public override int GroupCount
        {
            get
            {
                return listDataHeader.Count;
            }
        }
        public override long GetGroupId(int groupPosition)
        {
            return groupPosition;
        }
        public override View GetGroupView(int groupPosition, bool isExpanded, View convertView, ViewGroup parent)
        {
            string headerTitle = (string)GetGroup(groupPosition);
            convertView = convertView ?? context.LayoutInflater.Inflate(Resource.Layout.ProductHeader, null);
            convertView.FindViewById<TextView>(Resource.Id.txtProductHeader).Text = headerTitle;
            return convertView;
        }
        public override bool HasStableIds
        {
            get
            {
                return false;
            }
        }
        public override bool IsChildSelectable(int groupPosition, int childPosition)
        {
            return true;
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
            var totalMember = (from f in loanProducts
                               from p in f.MemberProducts
                               select p.MemberID).Distinct().Count();
            var totalMemberCollected = (from f in loanProducts
                                        from p in f.MemberProducts
                                        where p.IsCollected
                                        select p.MemberID).Distinct().Count();          
            var totalRecoverable = (from f in loanProducts
                                    from p in f.MemberProducts
                                    select p.Recoverable).Sum();
            var totalLoanCollected = (from f in loanProducts
                                      from p in f.MemberProducts
                                      where p.IsCollected
                                      select p.TodayCollectionAmount).Sum();
            var totalGSCollected = (from f in loanProducts
                                    from p in f.MemberProducts
                                    select p.GSCollected).Sum();
            var totalVSCollected = (from f in loanProducts
                                    from p in f.MemberProducts
                                    where p.IsCollected
                                    select p.VSCollected).Sum();
            var totalLTSCollected = (from f in loanProducts
                                     from p in f.MemberProducts
                                     where p.IsCollected
                                     select p.LTSCollected).Sum();
            this.txtTotalMember.Text = string.Format("TotM: {0}", totalMember);
            this.txtTotalCollectionMember.Text = string.Format("ColMem: {0}", totalMemberCollected);
            this.txtTotalRecoverable.Text = string.Format("RecL: {0}", totalRecoverable);
            this.txtTotalLoanCollection.Text = string.Format("ColL: {0}", totalLoanCollected);
            this.txtTotalGSCollection.Text = string.Format("GS: {0}", totalGSCollected);
            this.txtTotalVSCollection.Text = string.Format("VS: {0}", totalVSCollected);
            this.txtTotalLTSCollection.Text = string.Format("LTS: {0}", totalLTSCollected);
        }
        /*
        private void BtnDeleteCC_Click(object sender, EventArgs e)
        {
            View row = ((ImageView)sender).Parent as View;
            TextView txtIndex = row.FindViewById<TextView>(Resource.Id.txtIndex);
            int position = Convert.ToInt32(txtIndex.Text);
            ExpandableListView lv = row.Parent.Parent as ExpandableListView;

            row.Animate()
                .SetDuration(500)
                .Alpha(0)
                .WithEndAction(new Java.Lang.Runnable(() =>
                {
                    listDataHeader.RemoveAt(position);
                    SetListViewHeightBasedOnChildren(lv);
                    this.NotifyDataSetChanged();

                    row.Alpha = 1f;
                }));
        }
        private void BtnEdit_Click(object sender, EventArgs e)
        {
            AlertDialog dialog = null;
            var row = ((ImageView)sender).Parent as View;
            TextView txtIndex = row.FindViewById<TextView>(Resource.Id.txtIndex);
            int position = Convert.ToInt32(txtIndex.Text);
            var selectedItem = listDataHeader[position];

            var v = context.LayoutInflater.Inflate(Resource.Layout.Investigation, null);
            InflateGenericDialogView(v, selectedItem);
            AlertDialog.Builder alert = new AlertDialog.Builder(context);

            alert.SetView(v);
            alert.SetTitle("Edit Investigation");
            alert.SetPositiveButton("DONE", (senderAlert, args) =>
            {
                var item = listDataHeader[position];
                var txt = (EditText)v.FindViewById(Resource.Id.txtDetail);
                EditButtonClickedEvent.Invoke(item, txt.Text);
                dialog.Dismiss();
            });

            alert.SetNegativeButton("CANCEL", (senderAlert, args) =>
            {
                dialog.Dismiss();
            });
            dialog = alert.Create();
            dialog.Show();
        }
        private void InflateGenericDialogView(View dialogView, string item)
        {
            var txtDetail = (EditText)dialogView.FindViewById(Resource.Id.lblListHeader);
            //if (!string.IsNullOrWhiteSpace(hint))
            //{
                txtDetail.Hint = "Enter Investigation";
            //}
            txtDetail.Text = item.ToString();
        }*/

        private void SaveCollection(MemberProductModel obj)
        {
            var syncHelper = new LoanCollectionOfflineHelper(context);
            var memberOfflineHelper = new MemberOfflineHelper(context);
            var member = memberOfflineHelper.GetById(obj.MemberID); //TODO check this 
            if (obj.ProductID != 0 && !syncHelper.CollectionSummaryExist(obj.SummaryID))
            {
             
                syncHelper.AddNewCollection(new LoanCollectionModel()
                {
                    OfficeID = member.OfficeID,
                    OfficeName = member.OfficeName,
                    CenterID = member.CenterID,
                    CenterName = member.CenterName,
                    ProductID = obj.ProductID,
                    ProductName = obj.ProductName,// spnProduct.SelectedItem.ToString(),                           
                    MemberCode =member.MemberCode,
                    MemberID = obj.MemberID,
                    Amount = obj.TodayCollectionAmount,
                    DueAmount = obj.Recoverable,
                    TrxType = 1,
                    ProductType = obj.ProductType,
                    SyncFlag = 0,
                    SummaryID = obj.SummaryID,
                    LoanInstallment = obj.LoanInstallment,
                    IntInstallment = obj.IntInstallment,
                    IntCharge = obj.IntCharge
                });
            }
            if (obj.GSID != null && !syncHelper.CollectionSummaryExist(obj.GSID.SummaryID))
                syncHelper.AddNewCollection(new LoanCollectionModel()
                {
                    OfficeID = member.OfficeID,
                    OfficeName = member.OfficeName,
                    CenterID = member.CenterID,
                    CenterName = member.CenterName,
                    ProductID = obj.GSID.ProductID,
                    ProductName = obj.GSID.ProductName,// spnProduct.SelectedItem.ToString(),                           
                    MemberCode = member.MemberCode,
                    MemberID = obj.GSID.MemberID,
                    Amount = obj.GSCollected,
                    DueAmount = obj.GSID.Recoverable,
                    TrxType = 1,
                    ProductType = obj.GSID.ProductType,
                    SyncFlag = 0,
                    SummaryID = obj.GSID.SummaryID,
                    LoanInstallment = obj.GSCollected,
                    IntInstallment = obj.GSID.IntInstallment,
                    IntCharge = obj.GSID.IntCharge
                });
            if (obj.VSID != null && !syncHelper.CollectionSummaryExist(obj.VSID.SummaryID))
                syncHelper.AddNewCollection(new LoanCollectionModel()
                {
                    OfficeID = member.OfficeID,
                    OfficeName = member.OfficeName,
                    CenterID = member.CenterID,
                    CenterName = member.CenterName,
                    ProductID = obj.VSID.ProductID,
                    ProductName = obj.VSID.ProductName,// spnProduct.SelectedItem.ToString(),                           
                    MemberCode =member.MemberCode,
                    MemberID = obj.VSID.MemberID,
                    Amount = obj.VSCollected,
                    DueAmount = 0,
                    TrxType = 1,
                    ProductType = obj.VSID.ProductType,
                    SyncFlag = 0,
                    SummaryID = obj.VSID.SummaryID,
                    LoanInstallment = obj.VSCollected,
                    IntInstallment = obj.VSID.IntInstallment,
                    IntCharge = obj.VSID.IntCharge
                });
            if (obj.LTSID != null && !syncHelper.CollectionSummaryExist(obj.LTSID.SummaryID))
                syncHelper.AddNewCollection(new LoanCollectionModel()
                {
                    OfficeID = member.OfficeID,
                    OfficeName = member.OfficeName,
                    CenterID = member.CenterID,
                    CenterName = member.CenterName,
                    ProductID = obj.LTSID.ProductID,
                    ProductName = obj.LTSID.ProductName,// spnProduct.SelectedItem.ToString(),                           
                    MemberCode = member.MemberCode,
                    MemberID = obj.LTSID.MemberID,
                    Amount = obj.LTSCollected,
                    DueAmount = 0,
                    TrxType = 1,
                    ProductType = obj.LTSID.ProductType,
                    SyncFlag = 0,
                    SummaryID = obj.LTSID.SummaryID,
                    LoanInstallment = obj.LTSCollected,
                    IntInstallment = obj.LTSID.IntInstallment,
                    IntCharge = obj.LTSID.IntCharge
                });

        }

        private void DeleteCollection(MemberProductModel obj)
        {
            var syncHelper = new LoanCollectionOfflineHelper(context);
            if (obj.ProductID != 0)
                syncHelper.DeleteCollectionBySummaryId(obj.SummaryID);
            if (obj.GSID != null)
                syncHelper.DeleteCollectionBySummaryId(obj.GSID.SummaryID);
            if (obj.VSID != null)
                syncHelper.DeleteCollectionBySummaryId(obj.VSID.SummaryID);
            if (obj.LTSID != null)
                syncHelper.DeleteCollectionBySummaryId(obj.LTSID.SummaryID);
        }
        public void SetListViewHeightBasedOnChildren(ExpandableListView listView)
        {
            var listAdapter = listView.Adapter;
            if (listAdapter == null)
            {
                // pre-condition
                return;
            }

            int totalHeight = 0;
            for (int i = 0; i < listAdapter.Count; i++)
            {
                View listItem = listAdapter.GetView(i, null, listView);
                listItem.Measure(0, 0);
                totalHeight += listItem.MeasuredHeight;
            }

            ViewGroup.LayoutParams paramsa = listView.LayoutParameters;
            paramsa.Height = totalHeight + (listView.DividerHeight * (listAdapter.Count - 1));
            listView.LayoutParameters = paramsa;
            listView.RequestLayout();
        }

        public void SetListViewHeightBasedOnChildList(ExpandableListView listView)
        {
            var adapter = listView.ExpandableListAdapter;
            if (adapter == null)
            {
                return;
            }
            var childrenCount = adapter.GetChildrenCount(listView.SelectedItemPosition);
            int totalHeight = 0;
            for (int i = 0; i < childrenCount; i++)
            {
                View listItem = adapter.GetChildView(listView.SelectedItemPosition, i, false, null, listView);
                listItem.Measure(0, 0);
                totalHeight += listItem.MeasuredHeight;
            }

            ViewGroup.LayoutParams paramsa = listView.LayoutParameters;
            paramsa.Height = totalHeight + (listView.DividerHeight * (childrenCount - 1));
            listView.LayoutParameters = paramsa;
            listView.RequestLayout();
        }

    }
    public class PositionHolder : Java.Lang.Object
    {
        public int GroupPosition { get; set; }
        public int ChildPosition { get; set; }
    }
}