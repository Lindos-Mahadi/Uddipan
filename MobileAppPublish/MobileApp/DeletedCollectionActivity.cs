using System;
using System.Linq;

using Android.App;
using Android.OS;
using Android.Widget;
using PMS.Droid.Helpers;
using PMS.Droid.Classes.OffLineHelpers;
using PMS.Droid.Classes;
using System.Threading.Tasks;

namespace PMS.Droid
    {
    [Activity(Label = "Back Data", Theme = "@style/Theme.DesignDemo", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class DeletedCollectionActivity : BaseActivity
    {

        int centerId = 0;
        int[] centerArray;
            protected override void OnCreate(Bundle savedInstanceState)
            {
                base.OnCreate(savedInstanceState);
                SetContentView(Resource.Layout.DeletedCollection);
            
            try
                {
                    GenerateNavigationMenu();
                    LoadCollectionList();
                    SetSpinners();

                var btnSyncCollectionWithServer = FindViewById<Button>(Resource.Id.btnSyncCollectionWithServer);

                    btnSyncCollectionWithServer.Click += async delegate
                    {
                        await SyncCollection();
                        //await SyncCollectionDistribute();
                    };
                    // Create your application here
                }
                catch (Exception ex)
                {
                    LogSystemError(UIItems.SyncCollection + "  - Onload", ex.Message, "");
                }
            }

            private async Task SyncCollection()
            {
                CommonHelper.Busy(this);
                var url = string.Empty;
                try
                {
                    var nc = new NetworkConnection(this);
                    if (!nc.CheckNetworkConnection())
                    {
                        GeatMessage("You do not have active internet connection. Upload will not work.", false);
                        return;
                    }
                    var syncHelper = new LoanCollectionDeletedOfflineHelper(this);
                    var withdrawHelper = new WithdrawalOfflineHelper(this);
                    url = SharedMethods.GetOrganizationUrl(this);
                    if (string.IsNullOrEmpty(url))
                    {
                        GeatMessage("Organization url is not setup.", true);
                        return;
                    }
                    var items = syncHelper.GetAll();
                 
                    if (items.Count == 0)
                        GeatMessage("No offline Collection to Upload.", true);
                    else
                    {
                        var userHelper = new UserOfflineHelper(this);
                        var currentUserId = userHelper.GetUserID();
                        if (string.IsNullOrEmpty(currentUserId))
                        {
                            GeatMessage("No logged in user found to Upload.", true);
                            return;
                        }
                        var user = userHelper.GetUser();
                        DateTime dt = DateTime.MinValue;
                        if (user != null && !string.IsNullOrEmpty(user.InstallmentDate))
                        {
                            if (!DateTime.TryParse(user.InstallmentDate, out dt))
                            {
                                GeatMessage("No collection date found", true);
                                return;
                            }
                        }
                        if (dt == DateTime.MinValue)
                        {
                            GeatMessage("No collection date found", true);
                            return;
                        }
                        var apiErrorOccured = true;
                        var collectionModel = new CollectionAPIModel() { UserId = currentUserId, APIVersion = OfflineDBConstants.APP_DATABASE_VERSION, Collections = items, CollectionDate = user.InstallmentDate };
                        var result = await MemberDataApiHelper.PostMobileCollection(collectionModel, url, currentUserId);
                        if (result != null && result.Count > 0)
                        {
                            var APIVersionChecker = result.First();
                            var errorMsg = "";
                            if (APIVersionChecker == SyncErrorTypes.APIVersionMitchMatch)
                                errorMsg = "Please upgrade your mobile APP.";
                            else if (APIVersionChecker == SyncErrorTypes.CollectionDateDoesNotMatch)
                                errorMsg = "Collection date is older than Online Collection Date.";
                            else if (APIVersionChecker == SyncErrorTypes.ErrorConvertingSystemCollectionDate)
                                errorMsg = "Error Converting System CollectionDate. Please check with IT team for more assistance.";
                            else if (APIVersionChecker == SyncErrorTypes.ErrorRetrievingCollectionDate)
                                errorMsg = "Error Retrieving CollectionDate. Please check with IT team for more assistance.";
                            // GeatMessage("Please upgrade your mobile APP.", false);
                            if (!string.IsNullOrEmpty(errorMsg))
                                GeatMessage(errorMsg, false);
                            else
                            {
                                apiErrorOccured = false;
                                foreach (var colId in result)
                                    syncHelper.DeleteCollection(colId);
                                withdrawHelper.DeleteAll();
                            }
                        }
                        if (!apiErrorOccured)
                        {
                            items = syncHelper.GetAll();
                            if (items.Count == 0)
                                GeatMessage("All Collections Uploaded successfully.", true);
                            else
                                GeatMessage("Error occured while syncing. Below items are not Uploaded.", false);
                        }
                    }

                    LoadCollectionList();
                }
                catch (Exception ex)
                {
                    GeatMessage("Error occured in Upload. " + ex.Message, false);
                    LogSystemError(UIItems.SyncCollection + " - Save", ex.Message, "");
                    LoadCollectionList();
                }
                try
                {
                    //Sync error logs..
                    var errorHelper = new SystemErrorOfflineHelper(this);
                    var errors = errorHelper.GetAll();
                    if (errors.Count > 0)
                    {
                        await LoginHelper.PostErrorLog(errors, url);
                        errorHelper.DeleteAll();
                    }
                }
                catch (Exception ex)
                {
                    LogSystemError(UIItems.SyncError + " - Post", ex.Message, "");
                }
                CommonHelper.Done(this);
            }
            private async Task SyncCollectionDistribute()
            {
                CommonHelper.Busy(this);
                var url = string.Empty;
                try
                {
                    var nc = new NetworkConnection(this);
                    if (!nc.CheckNetworkConnection())
                    {
                        GeatMessage("You do not have active internet connection. Upload will not work.", false);
                        return;
                    }
                    var syncHelper = new LoanCollectionDeletedOfflineHelper(this);
                    var withdrawHelper = new WithdrawalOfflineHelper(this);
                    url = SharedMethods.GetOrganizationUrl(this);
                    if (string.IsNullOrEmpty(url))
                    {
                        GeatMessage("Organization url is not setup.", true);
                        return;
                    }
                    var items = syncHelper.GetAll();
                    if (items.Count == 0)
                        GeatMessage("No offline Collection to Upload.", true);
                    else
                    {
                        var userHelper = new UserOfflineHelper(this);
                        var currentUserId = userHelper.GetUserID();
                        if (string.IsNullOrEmpty(currentUserId))
                        {
                            GeatMessage("No logged in user found to Upload.", true);
                            return;
                        }
                        var user = userHelper.GetUser();
                        DateTime dt = DateTime.MinValue;
                        if (user != null && !string.IsNullOrEmpty(user.InstallmentDate))
                        {
                            if (!DateTime.TryParse(user.InstallmentDate, out dt))
                            {
                                GeatMessage("No collection date found", true);
                                return;
                            }
                        }
                        if (dt == DateTime.MinValue)
                        {
                            GeatMessage("No collection date found", true);
                            return;
                        }
                        var chunkSize = 300;
                        var processed = 0;
                        var apiErrorOccured = true;

                        while (true)
                        {
                            var currentItems = items.Skip(processed).Take(chunkSize);
                            var collectionModel = new CollectionAPIModel() { UserId = currentUserId, APIVersion = OfflineDBConstants.APP_DATABASE_VERSION, Collections = currentItems.ToList(), CollectionDate = user.InstallmentDate };
                            var result = await MemberDataApiHelper.PostMobileCollection(collectionModel, url, currentUserId);
                            if (result != null && result.Count > 0)
                            {
                                var APIVersionChecker = result.First();
                                var errorMsg = "";
                                if (APIVersionChecker == SyncErrorTypes.APIVersionMitchMatch)
                                    errorMsg = "Please upgrade your mobile APP.";
                                else if (APIVersionChecker == SyncErrorTypes.CollectionDateDoesNotMatch)
                                    errorMsg = "Collection date is older than Online Collection Date.";
                                else if (APIVersionChecker == SyncErrorTypes.ErrorConvertingSystemCollectionDate)
                                    errorMsg = "Error Converting System CollectionDate. Please check with IT team for more assistance.";
                                else if (APIVersionChecker == SyncErrorTypes.ErrorRetrievingCollectionDate)
                                    errorMsg = "Error Retrieving CollectionDate. Please check with IT team for more assistance.";
                                // GeatMessage("Please upgrade your mobile APP.", false);
                                if (!string.IsNullOrEmpty(errorMsg))
                                {
                                    GeatMessage(errorMsg, false);
                                    return;
                                }
                                else
                                {
                                    apiErrorOccured = false;
                                    foreach (var colId in result)
                                        syncHelper.DeleteCollection(colId);
                                    withdrawHelper.DeleteAll();
                                }
                            }
                            processed = processed + chunkSize;
                            if (processed >= items.Count)
                                break;
                        }
                        if (!apiErrorOccured)
                        {
                            items = syncHelper.GetAll();
                            if (items.Count == 0)
                                GeatMessage("All Collections Uploaded successfully.", true);
                            else
                                GeatMessage("Error occured while syncing. Below items are not Uploaded.", false);
                        }
                    }

                    LoadCollectionList();
                }
                catch (Exception ex)
                {
                    GeatMessage("Error occured in Upload. " + ex.Message, false);
                    LogSystemError(UIItems.SyncCollection + " - Save", ex.Message, "");
                    LoadCollectionList();
                }
                try
                {
                    //Sync error logs..
                    var errorHelper = new SystemErrorOfflineHelper(this);
                    var errors = errorHelper.GetAll();
                    if (errors.Count > 0)
                    {
                        await LoginHelper.PostErrorLog(errors, url);
                        errorHelper.DeleteAll();
                    }
                }
                catch (Exception ex)
                {
                    LogSystemError(UIItems.SyncError + " - Post", ex.Message, "");
                }
                CommonHelper.Done(this);
            }

            private void LoadCollectionList()
            {

                var dbVals = new LoanCollectionDeletedOfflineHelper(this);
                var listItsms = dbVals.GetAll();
            var count = 0;
            centerArray = new int[50];
            foreach (var CenterId in listItsms)
            {
                centerArray[count] = CenterId.CenterID;
                count++;

            }

                var spnCenter = FindViewById<Spinner>(Resource.Id.spnCenterCollection);
                spnCenter.ItemSelected += SpnCenter_ItemSelected;

            if(centerId != 0)
            {
                listItsms = listItsms.Where(x => x.CenterID == centerId).ToList();
            }

            var lv = FindViewById<ListView>(Resource.Id.loanCollectionList_SyncView);
                lv.Adapter = new DeletedCollectionAdapter(this, listItsms);


        }
            private void GeatMessage(string message, bool flag)
            {
                new CustomMessage(this, this, message, flag).Show();
            }
            private void LogException(Exception ex)
            {
                new CustomMessage(this, this, ex).Show();
            }


        private void SetSpinners()
        {
            CommonHelper.Busy(this);
            SetCenterSpinners();
            var spnCenter = FindViewById<Spinner>(Resource.Id.spnCenterCollection);
            spnCenter.ItemSelected += SpnCenter_ItemSelected;
             
            CommonHelper.Done(this);
        }

        private void SetCenterSpinners()
        {
            var officeId = SessionHelper.OfficeID;
            if (default(int) != officeId)
            {
                var syncHelper = new CenterOfflineHelper(this);
                var lst = syncHelper.GetCentersByOffice(officeId, 2);
                lst = lst.Where(x => x.CenterID != 0 && centerArray.Contains(x.CenterID)).ToList();
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
                 centerId = GetCenterId(obj);
                 LoadCollectionList();

                //MemberSelectedIndex = 0;
                //SetMemberSpinner(centerId);
            }
        }
        private int GetCenterId(string text)
        {
            var result = 0;
            var item = SessionHelper.CenterList.Where(w => w.CenterName == text).FirstOrDefault();
            if (item != null)
                result = item.CenterID;
            return result;
        }



    }
         
    }



       