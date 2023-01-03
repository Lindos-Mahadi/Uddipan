using System;
using System.Collections.Generic;
using System.Linq;

using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using PMS.Droid.Helpers;
using PMS.Droid.Classes.OffLineHelpers;
using System.IO;
using System.Text;
using Android.Media;
using PMS.Droid.Classes.Helpers;
using Android;
using Android.Content.PM;
using PMS.Droid.Classes;

namespace PMS.Droid
{
    [Activity(Label = "Collection", Theme = "@style/Theme.DesignDemo", WindowSoftInputMode = SoftInput.AdjustPan, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class CollectionNewActivity : BaseActivity
    {

        Dialog dialog;
        private long SelectedMemberID = 0;
        private int MemberSelectedIndex = 0;
        ListView listView;
        List<MemberProductModel> listItems;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            try
            {
                SetContentView(Resource.Layout.CollectionNew);
                GenerateNavigationMenu();
                SetSpinners();

                var syncUserHelper = new UserOfflineHelper(this);
                var user = syncUserHelper.GetUser();
                if (user != null && !string.IsNullOrEmpty(user.InstallmentDate))
                {
                    // var lblSamityAndDate = FindViewById<TextView>(Resource.Id.lblDate);
                    DateTime dt;
                    if (DateTime.TryParse(user.InstallmentDate, out dt))
                        Title = "Collection - " + dt.ToString("MMMM dd, yyyy");

                }

                var btnSave = FindViewById<Button>(Resource.Id.btnSaveCollection);
                btnSave.Click += delegate
                {
                    
                    DateTime nowDate = DateTime.Now;
                    string today = nowDate.Year + "-" + nowDate.Month + "-" + nowDate.Day;
                    string filename = nowDate.Day + "-"+ SessionHelper.LoggedInUserID;
                    
                   // bool isSDPresent = Android.OS.Environment.GetExternalStorageState(Android.OS.Environment.ExternalStorageDirectory).Equals(Android.OS.Environment.MediaMounted);
                   // bool isSDSupportedDevice = Android.OS.Environment.IsExternalStorageRemovable;
                    
                    string path = "";
                    string documentDirPath = Path.Combine(Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDocuments).AbsolutePath);
                    string downloadDirPath = Path.Combine(Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDownloads).AbsolutePath);
                    string appDataDirPath = Path.Combine(Android.OS.Environment.GetExternalStoragePublicDirectory("/Android/data/gBankerPMS.gBankerPMS").AbsolutePath);

                    bool documentDirExist = Directory.Exists(documentDirPath); ;
                    bool downloadDirExist = Directory.Exists(downloadDirPath);
                    bool appDataDirExist = Directory.Exists(appDataDirPath);
                    bool fileExist = false;

                        
                    
                    if (documentDirExist)
                    {
                        path = Path.Combine(Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDocuments).AbsolutePath, filename);
                        
                    }else if (downloadDirExist)
                    {
                        path = Path.Combine(Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDownloads).AbsolutePath, filename);
                    }
                    else if(appDataDirExist)
                    {
                        path = Path.Combine(Android.OS.Environment.GetExternalStoragePublicDirectory("/Android/data/gBankerPMS.gBankerPMS").AbsolutePath, filename);
                    }
                    else
                    {
                        Directory.CreateDirectory(appDataDirPath);
                        path = Path.Combine(Android.OS.Environment.GetExternalStoragePublicDirectory("/Android/data/gBankerPMS.gBankerPMS").AbsolutePath, filename);
                    }
                    

                    fileExist = File.Exists(path);
                   
                    

                    CommonHelper.Busy(this);
                    try
                    {
                        var isValid = true;
                        var spnMember = FindViewById<Spinner>(Resource.Id.spnMemberCollection);

                        //var txtMemberCode = FindViewById<EditText>(Resource.Id.txtMemberCode);
                        if (spnMember.SelectedItem == null)
                        {
                            GeatMessage("No Member selected.", false);
                            CommonHelper.Done(this);
                            return;
                        }
                        var memberCode = spnMember.SelectedItem.ToString();
                        var lv = FindViewById<ListView>(Resource.Id.loanProductCollectionList);
                        lv.ItemsCanFocus = true;
                        var adapter = lv.Adapter as LoanCollectionProductGridAdapter;
                        var productData = adapter.GetAllData();

                        if (isValid)
                        {
                            var spnCenter = FindViewById<Spinner>(Resource.Id.spnCenterCollection);
                            // var spnProduct = FindViewById<Spinner>(Resource.Id.spnProductCollection);

                            var officeID = SessionHelper.OfficeID;
                            if (spnCenter.SelectedItem == null)
                            {
                                GeatMessage("No Samity selected.", false);
                                CommonHelper.Done(this);
                                return;
                            }
                            var centerId = GetCenterId(spnCenter.SelectedItem.ToString());
                            var memberID = GetMemberId(spnMember.SelectedItem.ToString());
                            // var productId = 0;// GetProductId(spnProduct.SelectedItem.ToString(), memberID);
                            var syncHelper = new LoanCollectionOfflineHelper(this);
                            foreach(var item in productData)
                            {
                                var obj = item;
                                if (obj.ProductType == 0)
                                {
                                    double vSavingInstallment = 0;
                                    HelperMethods.CalculateSavings(obj.Recoverable, obj.TodayCollectionAmount, obj.Recoverable, out vSavingInstallment, obj.OrgID, Int32.Parse(obj.ProductName.Substring(0, 2)));
                                    item.LoanInstallment = vSavingInstallment;
                                }

                                if (obj.ProductType == 1)
                                {
                                    if ((obj.LoanInstallment + obj.IntInstallment) == 0 && obj.TodayCollectionAmount > 0)
                                    {
                                        throw new Exception("Please make sure is loan amount correct?");
                                        //GeatMessage("Please make sure is loan amount correct?", false);
                                        //CommonHelper.Done(this);
                                        //return;
                                    }
                                }

                                if (item.fine > 0 && (item.fine > item.Recoverable))
                                {
                                    throw new Exception("Fine amount should be less then installment");
                                    //GeatMessage("Fine amount should be less then installment", false);
                                    //CommonHelper.Done(this);
                                    //return;
                                }
                            }

                            foreach (var item in productData)
                            {
                                var obj = item;
                              

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
                                    //if (item.ProductType == 1 && item.LoanInstallment + item.IntInstallment == 0)
                                    //item.TodayCollectionAmount = 0;
                                    Amount = (item.ProductType == 1 && item.LoanInstallment + item.IntInstallment == 0) ? 0:  item.TodayCollectionAmount,
                                    DueAmount = item.Recoverable,
                                    TrxType = 1,
                                    ProductType = item.ProductType,
                                    SyncFlag = 0,
                                    SummaryID = item.SummaryID,
                                    LoanInstallment = item.LoanInstallment,
                                    IntInstallment = item.IntInstallment,
                                    IntCharge = item.IntCharge,
                                    Created = today,
                                    fine = item.fine
                                });
                            }

                            string[] columns = LoanCollectionOfflineHelper.getExportColumns();

                            var freeExternalStorage = Android.OS.Environment.ExternalStorageDirectory.UsableSpace;
                            var minimumByteSize = ((1024 * 1024)*2);
                            
                            if (freeExternalStorage >= minimumByteSize)
                            {
                                var permission = Android.Support.V4.Content.ContextCompat.CheckSelfPermission(this,Manifest.Permission.WriteExternalStorage);
                                if(permission == (int)Permission.Granted){
                                    if (fileExist)
                                    {
                                        File.Delete(path);
                                        FileHelper.SqliteToCsv(syncHelper.GetAllByCreated(today), path, false, columns);
                                    }
                                    else
                                    {
                                        FileHelper.SqliteToCsv(syncHelper.GetAllByCreated(today), path, true, columns);
                                    }
                                }
                            }
                            SetMemberSpinner(centerId);
                            GeatMessage("Collection created successfully", true);
                        }
                    }
                    catch (Exception ex)
                    {
                       // Console.WriteLine("\r\n" + ex.Message + "\r\n");
                        LogSystemError(UIItems.Collection + " - Save", ex.Message, "");
                        GeatMessage("Error occured." + ex.Message, false);
                    }
                    CommonHelper.Done(this);
                };

                var btnNext = FindViewById<Button>(Resource.Id.btnNext);
                btnNext.Click += delegate
                {
                    var spnMember = FindViewById<Spinner>(Resource.Id.spnMemberCollection);
                    var totalCount = spnMember.Adapter.Count;
                    if (MemberSelectedIndex < totalCount - 1)
                        MemberSelectedIndex = MemberSelectedIndex + 1;
                    else
                        MemberSelectedIndex = 0;
                    spnMember.SetSelection(MemberSelectedIndex);
                };
                var btnPrevious = FindViewById<Button>(Resource.Id.btnPrevious);
                btnPrevious.Click += delegate
                {
                    var spnMember = FindViewById<Spinner>(Resource.Id.spnMemberCollection);
                    var totalCount = spnMember.Adapter.Count;
                    if (MemberSelectedIndex <= 0)
                        MemberSelectedIndex = 0;
                    else
                        MemberSelectedIndex = MemberSelectedIndex - 1;
                    spnMember.SetSelection(MemberSelectedIndex);
                };
            }
            catch (Exception ex)
            {
                LogSystemError(UIItems.Collection + " - Onload", ex.Message, "");
            }
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
                var lst = syncHelper.GetCentersByOffice(officeId, 1);
                SessionHelper.CenterList = lst;
                var spnCenter = FindViewById<Spinner>(Resource.Id.spnCenterCollection);
                var adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerItem, lst.Select(s => s.CenterName).ToList());
                adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
                spnCenter.Adapter = adapter;
            }
            else
                GeatMessage("No office selected. Please select ofice and then try again.", false);
        }


        private void SpnCenter_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            var obj = ((Spinner)sender).SelectedItem.ToString();
            if (!string.IsNullOrEmpty(obj))
            {
                var centerId = GetCenterId(obj);
                MemberSelectedIndex = 0;
                SetMemberSpinner(centerId);
            }
        }

        private void SetMemberSpinner(int centerId)
        {
            var syncHelper = new MemberOfflineHelper(this);
            var lst = syncHelper.GetMembersByCenterForCollection(centerId, 1);
            var spnMember = FindViewById<Spinner>(Resource.Id.spnMemberCollection);
            var adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerItem, lst.Select(s => s.MemberName).ToList());
            adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            spnMember.Adapter = adapter;
            if (MemberSelectedIndex > 0 && lst.Count > 0)
            {
                if (MemberSelectedIndex == lst.Count)
                    MemberSelectedIndex = 0;
                else
                    MemberSelectedIndex = lst.Count - 1 > MemberSelectedIndex ? MemberSelectedIndex : lst.Count - 1;
                spnMember.SetSelection(MemberSelectedIndex);
            }
            if (lst.Count == 0)
                LoadGrid(null);
        }
        private void SpnMember_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            try
            {
                MemberSelectedIndex = e.Position;
                //var adapter =  e.Parent.GetChildAt(MemberSelectedIndex) as TextView;
                //if (adapter != null)
                //    adapter.SetBackgroundColor(Android.Graphics.Color.Black);
                var obj = ((Spinner)sender).SelectedItem.ToString();
                LoadGrid(obj);
            }
            catch (Exception ex)
            {
                LogSystemError(UIItems.Collection + " - SpnMember_ItemSelected", ex.Message, "");
                GeatMessage("Error occured. " + ex.Message, false);
            }
        }
        private void LoadGrid(string memberCode)
        {
            listItems = new List<MemberProductModel>();
            var todalCollectionTextBox = FindViewById<TextView>(Resource.Id.txtTodayCollection);
            if (!string.IsNullOrEmpty(memberCode))
            {
                var memberID = GetMemberId(memberCode);
                SelectedMemberID = memberID;
                var dbVals = new MemberProductOfflineHelper(this);
                listItems = dbVals.GetMemberProducts(memberID);
            }
            listView = FindViewById<ListView>(Resource.Id.loanProductCollectionList);
            if(listView != null)
            {
                listView.Adapter = new LoanCollectionProductGridAdapter(this, listItems, todalCollectionTextBox, listView);
                listView.ItemClick -= Lv_ItemClick;
                listView.ItemClick += Lv_ItemClick;
            }
            
            var sum = listItems.Sum(s => s.LoanRecovery);
            var totalRecovery = FindViewById<TextView>(Resource.Id.txtTotalRecovery); //txtTotalRecovery 
            totalRecovery.Text = sum.ToString();
            todalCollectionTextBox.Text = sum.ToString();
        }

        public override void OnBackPressed()
        {
            base.OnBackPressed();
            Console.WriteLine("here");
        }
        private void Lv_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            //Console.WriteLine("HERE");
            //var pos = e.Position;
            //var view = e.View;
            //var amt = view.FindViewById<TextView>(Resource.Id.txtLoanProductAmountCollection);
            //var txt = amt.Text;
            //var adapter = listView.Adapter as LoanCollectionProductGridAdapter;
            
            //AlertDialog.Builder builder = new AlertDialog.Builder(this);
            //LayoutInflater inflater = this.LayoutInflater;
            //View dialogView = inflater.Inflate(Resource.Layout.collectiondialog, null);
            ////TextView accountValue = dialogView.FindViewById<TextView>(Resource.Id.accoun);
            
            //builder.SetView(dialogView)
            //    .SetPositiveButton("Change Amount", (sendAlert, args) =>
            //    {
            //        listItems[pos].TodayCollectionAmount = 0;
            //        adapter.NotifyDataSetChanged();
            //        dialog.Dismiss();
            //    })
            //    .SetNegativeButton("Cancel", (sendAlert, args) =>
            //    {
            //        dialog.Dismiss();
            //    });
            //dialog = builder.Create();
            //dialog.Show();

        }

        private long GetMemberId(string text)
        {
            var syncHelper = new MemberOfflineHelper(this);
            var member = syncHelper.GetByName(text);
            if (member != null)
                return member.MemberID;
            return 0;
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