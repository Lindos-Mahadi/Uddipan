using System;
using System.Linq;

using Android.App;
using Android.OS;
using Android.Widget;
using PMS.Droid.Helpers;
using PMS.Droid.Classes.OffLineHelpers;
using PMS.Droid.Classes;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using PMS.Droid.Classes.Helpers;

namespace PMS.Droid
{
    [Activity(Label = "Emergency Export", Theme = "@style/Theme.DesignDemo", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class ExportCollectionActivity : BaseActivity
    {
        private static int statusCode = 0;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.ExportCollection);
            statusCode = 0;
            try
            {
                GenerateNavigationMenu();
                LoadCollectionList();
                var btnSyncCollectionWithFTPServer = FindViewById<Button>(Resource.Id.btnSyncCollectionWithFTPServer);

                btnSyncCollectionWithFTPServer.Click += async delegate
                {
                    GeatMessage("Please Wait Processing", true);
                    await Task.Run(() =>
                    {
                        UploadToServer();
                    });
                    if (statusCode < 0)
                    {
                        GeatMessage("Sorry! Unable to export. Please try again later", true);
                    }
                    else
                    {
                        GeatMessage("File Transfered Successfully", true);
                    }
                    
                };
                //Create your application here
            }
            catch (Exception ex)
            {
                LogSystemError(UIItems.SyncCollection + "  - Onload" , ex.Message, "");
            }
        }

        private void UploadToServer()
        {
            DateTime nowDate = DateTime.Now;
            string filename = nowDate.Day + "-" + SessionHelper.LoggedInUserID;
            string dirPath = Path.Combine(Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDocuments).AbsolutePath);
            string path = "";

            //if (File.Exists(dirPath))
            //{
            //    path = Path.Combine(Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDocuments).AbsolutePath, filename);
            //}
            //else
            //{
            //    path = Path.Combine(Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDownloads).AbsolutePath, filename);
            //}

            //string path = "";
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

            }
            else if (downloadDirExist)
            {
                path = Path.Combine(Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDownloads).AbsolutePath, filename);
            }
            else if (appDataDirExist)
            {
                path = Path.Combine(Android.OS.Environment.GetExternalStoragePublicDirectory("/Android/data/gBankerPMS.gBankerPMS").AbsolutePath, filename);
            }
            else
            {
                Directory.CreateDirectory(appDataDirPath);
                path = Path.Combine(Android.OS.Environment.GetExternalStoragePublicDirectory("/Android/data/gBankerPMS.gBankerPMS").AbsolutePath, filename);
            }


            fileExist = File.Exists(path);


            try
            {

                //Console.WriteLine("FS:"+path);
                if (fileExist)
                {
                    FtpUpload(filename, path);
                }
                else
                {
                    var syncHelper = new LoanCollectionOfflineHelper(this);
                    string[] columns = LoanCollectionOfflineHelper.getExportColumns();
                    if (syncHelper.GetAll().Any())
                    {
                        FileHelper.SqliteToCsv(syncHelper.GetAll("Created DESC"), path, true, columns);
                        FtpUpload(filename, path);
                        statusCode = 1;
                    }
                    else
                    {
                        statusCode = -1;
                    }
                    
                    
                }
               
            }
            catch (Exception ex)
            {
                statusCode = -1;
                LogSystemError(UIItems.SyncCollection + "  - Onload", ex.Message, "");
                //Console.WriteLine("ERROR: " + ex.Message);
            }
            
        }

        private void FtpUpload(string filename,string src)
        {
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create("ftp://162.241.219.116/" + filename);
            request.Credentials = new NetworkCredential("raisuddin@socialbusinesspedia.org", "raisuddin");
            request.UseBinary = true;

            request.Method = WebRequestMethods.Ftp.UploadFile;
            using (Stream fileStream = File.OpenRead(src))
            using (Stream ftpStream = request.GetRequestStream())
            {
                fileStream.CopyTo(ftpStream);
            }
        }

        //private async Task SyncCollection()
        //{
        //    CommonHelper.Busy(this);
        //    var url = string.Empty;
        //    try
        //    {
        //        var nc = new NetworkConnection(this);
        //        if (!nc.CheckNetworkConnection())
        //        {
        //            GeatMessage("You do not have active internet connection. Upload will not work.", false);
        //            return;
        //        }
        //        var syncHelper = new LoanCollectionOfflineHelper(this);
        //        var withdrawHelper = new WithdrawalOfflineHelper(this);
        //        url = SharedMethods.GetOrganizationUrl(this);
        //        if (string.IsNullOrEmpty(url))
        //        {
        //            GeatMessage("Organization url is not setup.", true);
        //            return;
        //        }
        //        var items = syncHelper.GetAll();
        //        if (items.Count == 0)
        //            GeatMessage("No offline Collection to Upload.", true);
        //        else
        //        {
        //            var userHelper = new UserOfflineHelper(this);
        //            var currentUserId = userHelper.GetUserID();
        //            if (string.IsNullOrEmpty(currentUserId))
        //            {
        //                GeatMessage("No logged in user found to Upload.", true);
        //                return;
        //            }
        //            var user = userHelper.GetUser();
        //            DateTime dt = DateTime.MinValue;
        //            if (user != null && !string.IsNullOrEmpty(user.InstallmentDate))
        //            {
        //                if (!DateTime.TryParse(user.InstallmentDate, out dt))
        //                {
        //                    GeatMessage("No collection date found", true);
        //                    return;
        //                }
        //            }
        //            if (dt == DateTime.MinValue)
        //            {
        //                GeatMessage("No collection date found", true);
        //                return;
        //            }
        //            var apiErrorOccured = true;
        //            var collectionModel = new CollectionAPIModel() { UserId = currentUserId, APIVersion = OfflineDBConstants.APP_DATABASE_VERSION, Collections = items, CollectionDate = user.InstallmentDate };
        //            var result = await MemberDataApiHelper.PostMobileCollection(collectionModel, url, currentUserId);
        //            if (result != null && result.Count > 0)
        //            {
        //                var APIVersionChecker = result.First();
        //                var errorMsg = "";
        //                if (APIVersionChecker == SyncErrorTypes.APIVersionMitchMatch)
        //                    errorMsg = "Please upgrade your mobile APP.";
        //                else if (APIVersionChecker == SyncErrorTypes.CollectionDateDoesNotMatch)
        //                    errorMsg = "Collection date is older than Online Collection Date.";
        //                else if (APIVersionChecker == SyncErrorTypes.ErrorConvertingSystemCollectionDate)
        //                    errorMsg = "Error Converting System CollectionDate. Please check with IT team for more assistance.";
        //                else if (APIVersionChecker == SyncErrorTypes.ErrorRetrievingCollectionDate)
        //                    errorMsg = "Error Retrieving CollectionDate. Please check with IT team for more assistance.";
        //                // GeatMessage("Please upgrade your mobile APP.", false);
        //                if (!string.IsNullOrEmpty(errorMsg))
        //                    GeatMessage(errorMsg, false);
        //                else
        //                {

        //                    var syncHelperLoanCollectionDeleted = new LoanCollectionDeletedOfflineHelper(Android.App.Application.Context);


        //                    var items_Collections = items;

        //                    foreach (var item in items_Collections)
        //                    {
        //                        //NEW WHEN Deleted Send to Deleted Table

        //                        syncHelperLoanCollectionDeleted.AddNewCollection(new LoanCollectionModel()
        //                        {
        //                            OfficeID = item.OfficeID,
        //                            OfficeName = item.OfficeName,
        //                            CenterID = item.CenterID,
        //                            CenterName = item.CenterName,
        //                            ProductID = item.ProductID,
        //                            ProductName = item.ProductName,// spnProduct.SelectedItem.ToString(),                           
        //                            MemberCode = item.MemberCode,
        //                            MemberID = item.MemberID,
        //                            Amount = item.Amount,
        //                            DueAmount = item.DueAmount,
        //                            CollectionType = 1 // 1 for collected, 2 for not collected yet
        //                        });
        //                    }


        //                    var items_Withdraws = withdrawHelper.GetAll();


        //                    foreach (var item in items_Withdraws)
        //                    {
        //                        //NEW WHEN Deleted Send to Deleted Table

        //                        syncHelperLoanCollectionDeleted.AddNewCollection(new LoanCollectionModel()
        //                        {
        //                            OfficeID = item.OfficeID,
        //                            OfficeName = item.OfficeName,
        //                            CenterID = item.CenterID,
        //                            CenterName = item.CenterName,
        //                            ProductID = item.ProductID,
        //                            ProductName = item.ProductName,// spnProduct.SelectedItem.ToString(),                           
        //                            MemberCode = item.MemberCode,
        //                            MemberID = item.MemberID,
        //                            Amount = item.Amount,
        //                            DueAmount = item.DueAmount,
        //                            CollectionType = 2 // 1 for collected, 2 for not collected yet
        //                        });
        //                    }
 
        //                    // END 



        //                    apiErrorOccured = false;
        //                    foreach (var colId in result)
        //                        syncHelper.DeleteCollection(colId);
        //                    withdrawHelper.DeleteAll();
        //                }
        //            }
        //            if (!apiErrorOccured)
        //            {
        //                items = syncHelper.GetAll();
        //                if (items.Count == 0)
        //                    GeatMessage("All Collections Uploaded successfully.", true);
        //                else
        //                    GeatMessage("Error occured while syncing. Below items are not Uploaded.", false);
        //            }
        //        }

        //        LoadCollectionList();
        //    }
        //    catch (Exception ex)
        //    {
        //        GeatMessage("Error occured in Upload. " + ex.Message, false);
        //        LogSystemError(UIItems.SyncCollection + " - Save", ex.Message, "");
        //        LoadCollectionList();
        //    }
        //    try
        //    {
        //        //Sync error logs..
        //        var errorHelper = new SystemErrorOfflineHelper(this);
        //        var errors = errorHelper.GetAll();
        //        if (errors.Count > 0)
        //        {
        //            await LoginHelper.PostErrorLog(errors, url);
        //            errorHelper.DeleteAll();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        LogSystemError(UIItems.SyncError + " - Post", ex.Message, "");
        //    }
        //    CommonHelper.Done(this);
        //}
        //private async Task SyncCollectionDistribute()
        //{
        //    CommonHelper.Busy(this);
        //    var url = string.Empty;
        //    try
        //    {
        //        var nc = new NetworkConnection(this);
        //        if (!nc.CheckNetworkConnection())
        //        {
        //            GeatMessage("You do not have active internet connection. Upload will not work.", false);
        //            return;
        //        }
                
        //        var syncHelper = new LoanCollectionOfflineHelper(this);
        //        var withdrawHelper = new WithdrawalOfflineHelper(this);
        //        url = SharedMethods.GetOrganizationUrl(this);
        //        if (string.IsNullOrEmpty(url))
        //        {
        //            GeatMessage("Organization url is not setup.", true);
        //            return;
        //        }
        //        var items = syncHelper.GetAll();
        //        if (items.Count == 0)
        //            GeatMessage("No offline Collection to Upload.", true);
        //        else
        //        {
        //            var userHelper = new UserOfflineHelper(this);
        //            var currentUserId = userHelper.GetUserID();
        //            if (string.IsNullOrEmpty(currentUserId))
        //            {
        //                GeatMessage("No logged in user found to Upload.", true);
        //                return;
        //            }
        //            var user = userHelper.GetUser();
        //            DateTime dt = DateTime.MinValue;
        //            if (user != null && !string.IsNullOrEmpty(user.InstallmentDate))
        //            {
        //                if (!DateTime.TryParse(user.InstallmentDate, out dt))
        //                {
        //                    GeatMessage("No collection date found", true);
        //                    return;
        //                }
        //            }
        //            if (dt == DateTime.MinValue)
        //            {
        //                GeatMessage("No collection date found", true);
        //                return;
        //            }
        //            var chunkSize = 300;
        //            var processed = 0;
        //            var apiErrorOccured = true;

        //            while (true)
        //            {
        //                var currentItems = items.Skip(processed).Take(chunkSize);
        //                var collectionModel = new CollectionAPIModel() { UserId = currentUserId, APIVersion = OfflineDBConstants.APP_DATABASE_VERSION, Collections = currentItems.ToList(), CollectionDate = user.InstallmentDate };
        //                var result = await MemberDataApiHelper.PostMobileCollection(collectionModel, url, currentUserId);
        //                if (result != null && result.Count > 0)
        //                {
        //                    var APIVersionChecker = result.First();
        //                    var errorMsg = "";
        //                    if (APIVersionChecker == SyncErrorTypes.APIVersionMitchMatch)
        //                        errorMsg = "Please upgrade your mobile APP.";
        //                    else if (APIVersionChecker == SyncErrorTypes.CollectionDateDoesNotMatch)
        //                        errorMsg = "Collection date is older than Online Collection Date.";
        //                    else if (APIVersionChecker == SyncErrorTypes.ErrorConvertingSystemCollectionDate)
        //                        errorMsg = "Error Converting System CollectionDate. Please check with IT team for more assistance.";
        //                    else if (APIVersionChecker == SyncErrorTypes.ErrorRetrievingCollectionDate)
        //                        errorMsg = "Error Retrieving CollectionDate. Please check with IT team for more assistance.";
        //                    // GeatMessage("Please upgrade your mobile APP.", false);
        //                    if (!string.IsNullOrEmpty(errorMsg))
        //                    {
        //                        GeatMessage(errorMsg, false);
        //                        return;
        //                    }
        //                    else
        //                    {
        //                        apiErrorOccured = false;
        //                        foreach (var colId in result)
        //                            syncHelper.DeleteCollection(colId);
        //                        withdrawHelper.DeleteAll();
        //                    }
        //                }
        //                processed = processed + chunkSize;
        //                if (processed >= items.Count)
        //                    break;
        //            }                   
        //            if (!apiErrorOccured)
        //            {
        //                items = syncHelper.GetAll();
        //                if (items.Count == 0)
        //                    GeatMessage("All Collections Uploaded successfully.", true);
        //                else
        //                    GeatMessage("Error occured while syncing. Below items are not Uploaded.", false);
        //            }
        //        }

        //        LoadCollectionList();
        //    }
        //    catch (Exception ex)
        //    {
        //        GeatMessage("Error occured in Upload. " + ex.Message, false);
        //        LogSystemError(UIItems.SyncCollection + " - Save", ex.Message, "");
        //        LoadCollectionList();
        //    }
        //    try
        //    {
        //        //Sync error logs..
        //        var errorHelper = new SystemErrorOfflineHelper(this);
        //        var errors = errorHelper.GetAll();
        //        if (errors.Count > 0)
        //        {
        //            await LoginHelper.PostErrorLog(errors, url);
        //            errorHelper.DeleteAll();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        LogSystemError(UIItems.SyncError + " - Post", ex.Message, "");
        //    }
        //    CommonHelper.Done(this);
        //}

        private void LoadCollectionList()
        {
            var dbVals = new LoanCollectionOfflineHelper(this);
            var listItsms = dbVals.GetAll();
            var lv = FindViewById<ListView>(Resource.Id.loanCollectionList_SyncView);
            lv.Adapter = new LoanCollectionSyncAdapter(this, listItsms);
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
    //class SyncErrorTypes
    //{
    //    public const int APIVersionMitchMatch = -99;
    //    public const int CollectionDateDoesNotMatch = -100;
    //    public const int ErrorConvertingSystemCollectionDate = -101;
    //    public const int ErrorRetrievingCollectionDate = -102;
    //}
}