using System;

using Android.App;
using Android.OS;
using Android.Widget;
using PMS.Droid.Helpers;

namespace PMS.Droid
{
    [Activity(Label = "GB PMS - Mobile Home", Theme = "@android:style/Theme.Light", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class MobileHomeActivity : BaseActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.MobileHome);
            var lblOffice = FindViewById<TextView>(Resource.Id.lblSelectedOffice);
            var selectedOfficeText = string.Format("Selected Office: {0}", SessionHelper.OfficeName);
            lblOffice.Text = selectedOfficeText;
            var btnLoanProposal = FindViewById<Button>(Resource.Id.btnLoanProposal);
            btnLoanProposal.Click += btnLoanProposal_Click;
            var btnCollection = FindViewById<Button>(Resource.Id.btnCollection);
            btnCollection.Click += btnCollection_Click;

            var btnSelectAnotherOffice = FindViewById<Button>(Resource.Id.btnSelectAnotherOffice);
            btnSelectAnotherOffice.Click += BtnSelectAnotherOffice_Click;

            var btnLoanProposalSync = FindViewById<Button>(Resource.Id.btnLoanProposalSync);
            btnLoanProposalSync.Click += BtnLoanProposalSync_Click;

            var btnCollectionSync = FindViewById<Button>(Resource.Id.btnCollectionSync);
            btnCollectionSync.Click += btnCollectionSync_Click;

            var btnCollectionSummary = FindViewById<Button>(Resource.Id.btnCollectionSummary);
            btnCollectionSummary.Click += BtnCollectionSummary_Click;
        }

        private void BtnCollectionSummary_Click(object sender, EventArgs e)
        {
            StartActivity(typeof(CollectionSummaryActivity));
        }

        private void btnCollectionSync_Click(object sender, EventArgs e)
        {
            StartActivity(typeof(SyncCollectionActivity));            
        }

        private void BtnLoanProposalSync_Click(object sender, EventArgs e)
        {
           StartActivity(typeof(SyncLoanProposalActivity));
        }

        private void BtnSelectAnotherOffice_Click(object sender, EventArgs e)
        {
            StartActivity(typeof(MainActivity));
        }

        private void btnCollection_Click(object sender, EventArgs e)
        {
            StartActivity(typeof(CollectionActivity));
        }

        private void btnLoanProposal_Click(object sender, EventArgs e)
        {
            StartActivity(typeof(LoanProposalActivity));
        }
    }
}