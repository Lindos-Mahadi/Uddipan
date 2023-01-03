using System;

using Android.App;
using Android.OS;
using Android.Widget;
using PMS.Droid.Classes.OffLineHelpers;
using PMS.Droid.Helpers;
using PMS.Droid.Classes;

namespace PMS.Droid
{
    [Activity(Label = "Sync Loan Proposal", Theme = "@style/Theme.DesignDemo", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class SyncLoanProposalActivity : BaseActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.SyncLoanProposal);
            GenerateNavigationMenu();
            try
            {
                LoadProposalList();
                var btnSyncProposalWithServer = FindViewById<Button>(Resource.Id.btnSyncProposalWithServer);

                btnSyncProposalWithServer.Click += async delegate
                {
                    CommonHelper.Busy(this);
                    try
                    {
                        var nc = new NetworkConnection(this);
                        if (!nc.CheckNetworkConnection())
                        {
                            GeatMessage("You do not have active internet connection. Sync will not work.", true);
                            return;
                        }
                        var syncHelper = new LoanProposalOfflineHelper(this);
                        var items = syncHelper.GetAll();
                        if (items.Count == 0)
                            GeatMessage("No offline proposal to sync.", true);
                        else
                        {
                            var url = SharedMethods.GetOrganizationUrl(this);
                            if (string.IsNullOrEmpty(url))
                            {
                                GeatMessage("Organization url is not setup.", true);
                                return;
                            }
                            foreach (var item in items)
                            {
                                var result = await MemberDataApiHelper.CreateLoanProposal(item.MemberCode.Trim(), item.Amount.ToString(), item.OfficeID, item.CenterID, item.ProductID, item.PurposeID, url);
                                if (result == "Success")
                                    syncHelper.DeleteProposal(item.ProposalID);
                            }
                        }
                        //Reload proposal list.
                        items = syncHelper.GetAll();
                        if (items.Count == 0)
                            GeatMessage("All proposals synced successfully.", true);
                        else
                            GeatMessage("Error occured while syncing.Below items are not synced.", false);

                        LoadProposalList();
                    }
                    catch (Exception ex)
                    {
                        GeatMessage("Error occured while syncing. ", false);
                        LogSystemError(UIItems.SyncProposal + " - Save", ex.Message, "");
                        LoadProposalList();
                    }
                    CommonHelper.Done(this);
                };
            }
            catch (Exception ex)
            {
                LogSystemError(UIItems.SyncProposal + " - Onload", ex.Message, "");
                GeatMessage("Error ocured while loading view.", false);
            }
        }
        

        private void LoadProposalList()
        {
            var dbVals = new LoanProposalOfflineHelper(this);
            var listItsms = dbVals.GetAll();
            var lv = FindViewById<ListView>(Resource.Id.loanProposalList_SyncView);
            lv.Adapter = new LoanProposalSyncAdapter(this, listItsms);
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