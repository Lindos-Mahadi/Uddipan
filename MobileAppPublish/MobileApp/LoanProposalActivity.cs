using System;
using System.Linq;
using Android.App;
using Android.OS;
using Android.Widget;
using PMS.Droid.Helpers;
using PMS.Droid.Classes.OffLineHelpers;

namespace PMS.Droid
{
    [Activity(Label = "Loan Proposal", Theme = "@style/Theme.DesignDemo", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class LoanProposalActivity : BaseActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.LoanProposal);
            GenerateNavigationMenu();
            try
            {
                SetSpinners();
                var btnSave = FindViewById<Button>(Resource.Id.btnSaveLoanProposal);
                btnSave.Click += delegate
               {
                   CommonHelper.Busy(this);
                   try
                   {
                       var isValid = false;
                       var spnMember = FindViewById<Spinner>(Resource.Id.spnMemberLoanProposal);

                       //var txtMemberCode = FindViewById<EditText>(Resource.Id.txtMemberCode);
                       var memberCode = spnMember.SelectedItem.ToString();
                       var txtAmount = FindViewById<EditText>(Resource.Id.txtAmount);
                       //isValid = RequiredFieldValidation(memberCode, "Member  required");
                       isValid = RequiredFieldValidation(txtAmount, "Amount required");
                       isValid = ValidateAmount(txtAmount, "Amount should be a vaild number");
                       if (isValid)
                       {
                           var spnCenter = FindViewById<Spinner>(Resource.Id.spnCenter);
                           var spnProduct = FindViewById<Spinner>(Resource.Id.spnProduct);
                           var spnPurpose = FindViewById<Spinner>(Resource.Id.spnPurpose);

                           var officeID = SessionHelper.OfficeID;
                           var centerId = GetCenterId(spnCenter.SelectedItem.ToString());
                           var purposeID = GetPurposeId(spnPurpose.SelectedItem.ToString());
                           var productId = GetProductId(spnProduct.SelectedItem.ToString());

                           var syncHelper = new LoanProposalOfflineHelper(this);
                           syncHelper.AddNewProposal(new Classes.OffLineHelpers.LoanProposalModel()
                           {
                               OfficeID = SessionHelper.OfficeID,
                               OfficeName = SessionHelper.OfficeName,
                               CenterID = centerId,
                               CenterName = spnCenter.SelectedItem.ToString(),
                               ProductID = productId,
                               ProductName = spnProduct.SelectedItem.ToString(),
                               PurposeID = purposeID,
                               PurposeName = spnPurpose.SelectedItem.ToString(),
                               MemberCode = memberCode,
                               Amount = double.Parse(txtAmount.Text)
                           });

                           txtAmount.Text = "";
                           GeatMessage("Loan Proposal created successfully", true);
                       }
                   }
                   catch (Exception ex)
                   {
                       LogSystemError(UIItems.Proposal + " - Save", ex.Message, "");
                       LogException(ex);
                   }
                   CommonHelper.Done(this);
               };
            }
            catch (Exception ex)
            {
                LogSystemError(UIItems.Proposal + " - OnLoad", ex.Message, "");
            }
        }

        private void SetSpinners()
        {
            CommonHelper.Busy(this);
            SetCenterSpinners();
            SetProductSpinners();
            SetPurposeSpinners();            
            CommonHelper.Done(this);
        }
        private void SetCenterSpinners()
        {
            var officeId = SessionHelper.OfficeID;
            if (default(int) != officeId)
            {
                var syncHelper = new CenterOfflineHelper(this);
                var lst = syncHelper.GetCentersByOffice(officeId, 99);
                SessionHelper.CenterList = lst;
                var spnCenter = FindViewById<Spinner>(Resource.Id.spnCenter);
                var adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerItem, lst.Select(s => s.CenterName).ToList());
                adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
                spnCenter.Adapter = adapter;
                spnCenter.ItemSelected += SpnCenter_ItemSelected;
            }
            else
                GeatMessage("No office selected. Please select office and try again.", false);
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
            var lst = syncHelper.GetMembersByCenter(centerId);
            var spnMember = FindViewById<Spinner>(Resource.Id.spnMemberLoanProposal);
            var adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerItem, lst.Select(s => s.MemberName).ToList());
            adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            spnMember.Adapter = adapter;           
        }       

        private void SetProductSpinners()
        {
            var syncHelper = new ProductOfflineHelper(this);
            var lst = syncHelper.GetAll();
            SessionHelper.ProductList = lst;
            var spn = FindViewById<Spinner>(Resource.Id.spnProduct);
            var adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerItem, lst.Select(s => s.ProductName).ToList());
            adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            spn.Adapter = adapter;
        }
        private void SetPurposeSpinners()
        {
            var syncHelper = new PurposeOfflineHelper(this);
            var lst = syncHelper.GetAll();
            SessionHelper.PurposeList = lst;
            var spn = FindViewById<Spinner>(Resource.Id.spnPurpose);
            var adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerItem, lst.Select(s => s.PurposeName).ToList());
            adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            spn.Adapter = adapter;
        }
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
        private int GetProductId(string text)
        {
            var result = 0;
            var item = SessionHelper.ProductList.Where(w => w.ProductName == text).FirstOrDefault();
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

        private long GetMemberId(string text)
        {
            var syncHelper = new MemberOfflineHelper(this);
            var member = syncHelper.GetByName(text);
            if (member != null)
                return member.MemberID;
            return 0;
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