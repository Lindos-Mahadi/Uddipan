using System;
using System.Linq;
using Android.App;
using Android.OS;
using Android.Widget;
using PMS.Droid.Helpers;
using PMS.Droid.Classes.OffLineHelpers;

namespace PMS.Droid
{
    [Activity(Label = "Collection", Theme = "@android:style/Theme.Light")]
    public class CollectionActivity : Activity
    {
        private long SelectedMemberID = 0;
        protected  override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Collection);
            var syncHelper = new LoanCollectionOfflineHelper(this);
            SetSpinners();
            // Create your application here
            //btnSave
            var btnSave = FindViewById<Button>(Resource.Id.btnSaveCollection);
            var btnBackToMenu = FindViewById<Button>(Resource.Id.btnBackToMenu);

            btnBackToMenu.Click += delegate
            {

                StartActivity(typeof(MobileHomeActivity));
            };
            btnSave.Click += delegate
            {
                CommonHelper.Busy(this);
                try
                {
                    var isValid = true;
                    var spnMember = FindViewById<Spinner>(Resource.Id.spnMemberCollection);

                    //var txtMemberCode = FindViewById<EditText>(Resource.Id.txtMemberCode);
                    var memberCode = GetMemberCode(spnMember.SelectedItem.ToString());
                    var lv = FindViewById<ListView>(Resource.Id.loanProductCollectionList);
                    var adapter = lv.Adapter as LoanCollectionProductGridAdapter;
                    var productData = adapter.GetAllData();
                    //var txtMemberCode = FindViewById<EditText>(Resource.Id.txtMemberCodeCollection);
                    //var txtAmount = FindViewById<EditText>(Resource.Id.txtAmountCollection);
                    //isValid = RequiredFieldValidation(txtMemberCode, "Member Code required");
                    // isValid = RequiredFieldValidation(txtAmount, "Amount is required");
                    //isValid = ValidateAmount(txtAmount, "Amount is should be a vaild number");
                    if (isValid)
                    {
                        var spnCenter = FindViewById<Spinner>(Resource.Id.spnCenterCollection);
                       // var spnProduct = FindViewById<Spinner>(Resource.Id.spnProductCollection);

                        var officeID = SessionHelper.OfficeID;
                        var centerId = GetCenterId(spnCenter.SelectedItem.ToString());
                        var memberID = GetMemberId(spnMember.SelectedItem.ToString());
                       // var productId = 0;// GetProductId(spnProduct.SelectedItem.ToString(), memberID);
                       
                        foreach (var item in productData)
                        {
                            syncHelper.AddNewCollection(new LoanCollectionModel()
                            {
                                OfficeID = SessionHelper.OfficeID,
                                OfficeName = SessionHelper.OfficeName,
                                CenterID = centerId,
                                CenterName = spnCenter.SelectedItem.ToString(),
                                ProductID = item.ProductID,
                                ProductName = item.ProductName,// spnProduct.SelectedItem.ToString(),                           
                                MemberCode = memberCode,
                                MemberID = memberID,
                                Amount = item.TodayCollectionAmount,
                                DueAmount = item.Recoverable
                            });
                        }
                        // txtAmount.Text = "";
                        SetMemberSpinner(centerId);
                        GeatMessage("Loan Collection created successfully", true);
                    }
                }
                catch (Exception ex)
                {
                    LogException(ex);
                }
                CommonHelper.Done(this);
            };
        }
        private void SetSpinners()
        {
            CommonHelper.Busy(this);
            SetCenterSpinners();
            var spnCenter = FindViewById<Spinner>(Resource.Id.spnCenterCollection);
            spnCenter.ItemSelected += SpnCenter_ItemSelected;
            var spnMember = FindViewById<Spinner>(Resource.Id.spnMemberCollection);
            spnMember.ItemSelected += SpnMember_ItemSelected;           
            CommonHelper.Done(this);
        }
       
        private void SpnProduct_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            var obj = ((Spinner)sender).SelectedItem.ToString();
            if (!string.IsNullOrEmpty(obj))
            {
                LoadProductDetails(obj);
            }
        }
        private void LoadProductDetails(string product)
        {
            var mbrProductHelper = new MemberProductOfflineHelper(this);
            var productDetails = mbrProductHelper.GetProductByMemberAndName(product, SelectedMemberID);
           // var txtSelectedProductDetails = FindViewById<EditText>(Resource.Id.txtSelectedProductDetails);
            var detailTxt = "";
            switch (productDetails.ProductType)
            {
                case 0:
                    detailTxt = string.Format("Deposit: {0}, Recoverable: {1}, Balance: {2}, Ins: {3}", productDetails.LoanRecovery, productDetails.Recoverable, productDetails.Balance, productDetails.InstallmentNo);
                    break;
                case 1:
                    detailTxt = string.Format("Recovery: {0}, Recoverable: {1}, Balance: {2}, Ins: {3}", productDetails.LoanRecovery, productDetails.Recoverable, productDetails.Balance, productDetails.InstallmentNo);
                    break;
                default:
                    break;
            }
           // txtSelectedProductDetails.Text = detailTxt;
        }
        private void SetCenterSpinners()
        {
            var officeId = SessionHelper.OfficeID;
            if (default(int) != officeId)
            {
                var syncHelper = new CenterOfflineHelper(this);
                var lst = syncHelper.GetCentersByOffice(officeId);
                SessionHelper.CenterList = lst;
                var spnCenter = FindViewById<Spinner>(Resource.Id.spnCenterCollection);
                var adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerItem, lst.Select(s => s.CenterName).ToList());
                adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
                spnCenter.Adapter = adapter;               
            }
        }

       
        private void SpnCenter_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            var obj = ((Spinner)sender).SelectedItem.ToString();
            if (!string.IsNullOrEmpty(obj))
            {
                var centerId = GetCenterId(obj);
                SetMemberSpinner(centerId);
            }
        }

        private void SetMemberSpinner(int centerId)
        {
            var syncHelper = new MemberOfflineHelper(this);
            var lst = syncHelper.GetMembersByCenterForCollection(centerId);
            var spnMember = FindViewById<Spinner>(Resource.Id.spnMemberCollection);
            var adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerItem, lst.Select(s => s.MemberName).ToList());
            adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            spnMember.Adapter = adapter;
        }
        private void SpnMember_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            var obj = ((Spinner)sender).SelectedItem.ToString();
            if (!string.IsNullOrEmpty(obj))
            {
                var memberID = GetMemberId(obj);
                SelectedMemberID = memberID;
                var todalCollectionTextBox = FindViewById<TextView>(Resource.Id.txtTodayCollection);
                var dbVals = new MemberProductOfflineHelper(this);
                var listItsms = dbVals.GetMemberProducts(memberID);               
                var lv = FindViewById<ListView>(Resource.Id.loanProductCollectionList);
                lv.Adapter = new LoanCollectionProductGridAdapter(this, listItsms, todalCollectionTextBox, lv);
                lv.ItemClick += Lv_ItemClick;
                var sum = listItsms.Sum(s => s.LoanRecovery);
                var totalRecovery = FindViewById<TextView>(Resource.Id.txtTotalRecovery); //txtTotalRecovery 
                totalRecovery.Text = sum.ToString();
                todalCollectionTextBox.Text = sum.ToString();
            }
        }

        private void Lv_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            var pos = e.Position;
            var view = e.View;
            var amt = view.FindViewById<EditText>(Resource.Id.txtLoanProductAmountCollection);
            var txt = amt.Text;
        }

        private long GetMemberId(string text)
        {
            var syncHelper = new MemberOfflineHelper(this);
            var member = syncHelper.GetByName(text);
            if (member != null)
                return member.MemberID;
            return 0;
        }

        //private void SetProductSpinners(long memberID)
        //{
        //    var syncHelper = new MemberProductOfflineHelper(this);
        //    var lst = syncHelper.GetProductsByMember(memberID);
        //    var spn = FindViewById<Spinner>(Resource.Id.spnProductCollection);
        //    var adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerItem, lst.Select(s => s.ProductName).ToList());
        //    adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
        //    spn.Adapter = adapter;
        //}
        private bool RequiredFieldValidation(EditText control, string message)
        {
            var isValid = true;
            if (string.IsNullOrWhiteSpace(control.Text.Trim()))
            {
                new CustomMessage(this, this, message, false).Show();
                isValid = false;
                control.FindFocus();
            }
            return isValid;
        }
        private bool ValidateAmount(EditText control, string text)
        {
            var isValid = true;
            var amt = 0.0d;
            if (!double.TryParse(control.Text, out amt))
            {
                new CustomMessage(this, this, text, false).Show();
                isValid = false;
                control.FindFocus();
            }
            return isValid;
        }

        private string GetMemberCode(string text)
        {
            var splits = text.Split(",".ToArray());
            if (splits.Length > 1)
                return splits[0];
            return "";
        }
        private int GetProductId(string text, long memberID)
        {
            var syncHelper = new MemberProductOfflineHelper(this);
            var item = syncHelper.GetProductByMemberAndName(text, memberID);
            var result = 0;
            if (item != null)
                result = item.ProductID;
            return result;
        }
        private int GetPurposeId(string text)
        {
            var result = 0;
            var item = SessionHelper.PurposeList.Where(w => w.PurposeName == text).FirstOrDefault();
            if (item != null)
                result = item.PurposeID;
            return result;
        }
        private int GetCenterId(string text)
        {
            var result = 0;
            var item = SessionHelper.CenterList.Where(w => w.CenterName == text).FirstOrDefault();
            if (item != null)
                result = item.CenterID;
            return result;
        }
        private void GeatMessage(string message, bool flag)
        {
            new CustomMessage(this, this, message, flag).Show();
        }
        private void LogException(Exception ex)
        {
            new CustomMessage(this, this, ex).Show();
        }
    }
}