using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;
using PMS.Droid.Helpers;
using Newtonsoft.Json;
using PMS.Droid.Classes.OffLineHelpers;

namespace GBPMS.Droid.Classes
{
    public class LookupProviderAPI
    {
        public static async Task<List<LookupItem>> GetUserOfficeListAsync(string url)
        {
            HttpClient client = null;
            client = SecurityHelpers.ConfigHeader(client);

            var currentAction = "lookupapi/getalloffice";
            var uri = new Uri(string.Format(url, currentAction));
            var lst = new List<LookupItem>();
            try
            {
                var response = await client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    lst = JsonConvert.DeserializeObject<List<LookupItem>>(content);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return lst;
        }
        public static async Task<List<LookupItem>> GetProductListAsync(string url)
        {

            HttpClient client = null;
            client = SecurityHelpers.ConfigHeader(client);

            var currentAction = "lookupapi/getproduct";
            var uri = new Uri(string.Format(url, currentAction));
            //Console.WriteLine("PRODUCT LIST: URL:"+string.Format(url, currentAction));
            var lst = new List<LookupItem>();
            try
            {
                var response = await client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    lst = JsonConvert.DeserializeObject<List<LookupItem>>(content);
                    
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return lst;

        }
        public static async Task<List<LookupItem>> GetCenterListAsync(int officeID, string loginID, string url)
        {

            HttpClient client = null;
            client = SecurityHelpers.ConfigHeader(client);

            var currentAction = string.Format("lookupapi/getcenterbyoffice?officeID={0}&loginID={1}", officeID, loginID);
            var uri = new Uri(string.Format(url, currentAction));
            var lst = new List<LookupItem>();
            try
            {
                var response = await client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    lst = JsonConvert.DeserializeObject<List<LookupItem>>(content);                   
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return lst;

        }
        public static async Task<List<MobileOfficeDataSyncModel>> GetMobileDataSyncAsync(int officeID, string loginID, string url)
        {

            HttpClient client = null;
            client = SecurityHelpers.ConfigHeader(client);

            var currentAction = string.Format("lookupapi/GetOfficeSyncData?officeID={0}&loginID={1}", officeID, loginID);
            var uri = new Uri(string.Format(url, currentAction));
            //Console.WriteLine("ALL DATA: "+string.Format(url, currentAction));
            var lst = new List<MobileOfficeDataSyncModel>();
            try
            {
                var response = await client.GetAsync(uri);
                var content = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                    lst = JsonConvert.DeserializeObject<List<MobileOfficeDataSyncModel>>(content);
                else
                {
                    throw new Exception("API throws exception: " + content);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return lst;

        }
        public static async Task<List<MobileOfficeDataSyncModel>> GetMobileDataNewSavingsMemberAsync(int officeID, string loginID, string url)
        {

            HttpClient client = null;
            client = SecurityHelpers.ConfigHeader(client);

            var currentAction = string.Format("lookupapi/getofficesyncdatanewsavings?officeID={0}&loginID={1}", officeID, loginID);
            var uri = new Uri(string.Format(url, currentAction));
            var lst = new List<MobileOfficeDataSyncModel>();
            try
            {
                var response = await client.GetAsync(uri);
                var content = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                    lst = JsonConvert.DeserializeObject<List<MobileOfficeDataSyncModel>>(content);
                else
                {
                    throw new Exception("API throws exception: " + content);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return lst;

        }
        public static async Task<List<LookupItem>> GetMemberByCenterAsync(int officeID, int centerID, string url)
        {

            HttpClient client = null;
            client = SecurityHelpers.ConfigHeader(client);

            var currentAction = string.Format("lookupapi/GetMemberListbyCenter?OfficeID={0}&CenterID={1}", officeID, centerID);
            var uri = new Uri(string.Format(url, currentAction));
            var lst = new List<LookupItem>();
            try
            {
                var response = await client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    lst = JsonConvert.DeserializeObject<List<LookupItem>>(content);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return lst;

        }
        public static async Task<List<MemberProductModel>> GetMemberProductsAsync(int officeID, int centerID, long memberID, string url)
        {

            HttpClient client = null;
            client = SecurityHelpers.ConfigHeader(client);

            var currentAction = string.Format("lookupapi/GetProductsForMember?memberID={0}", memberID);
            var uri = new Uri(string.Format(url, currentAction));
            var lst = new List<MemberProductModel>();
            try
            {
                var response = await client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    lst = JsonConvert.DeserializeObject<List<MemberProductModel>>(content);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return lst;

        }
        public static async Task<List<LookupItem>> GetPurposeListAsync( string url)
        {

            HttpClient client = null;
            client = SecurityHelpers.ConfigHeader(client);

            var currentAction = "lookupapi/GetPurpose";
            var uri = new Uri(string.Format(url, currentAction));
            //Console.WriteLine("PurposeListURL: " + string.Format(url, currentAction));
            var lst = new List<LookupItem>();
            try
            {
                var response = await client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    lst = JsonConvert.DeserializeObject<List<LookupItem>>(content);
                   
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return lst;

        }
    }

    public class LookupItem
    {
        public string Text { get; set; }
        public string Value { get; set; }
    }
    public class MobileOfficeDataSyncModel
    {
        public long MemberID { get; set; }
        public string MemberCode { get; set; }
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public int ProductType { get; set; }
        public double LoanRecovery { get; set; }
        public double Recoverable { get; set; }
        public double Balance { get; set; }
        public double PrinBalance { get; set; }
        public double SerBalance { get; set; }
        public int InstallmentNo { get; set; }
        //public int OfficeID { get; set; }
        //public string OfficeCode { get; set; }
        //public string OfficeName { get; set; }
        public int CenterID { get; set; }
        public string CenterCode { get; set; }
        public string CenterName { get; set; }
        public string MemberName { get; set; }
        public string InstallmentDate { get; set; }
        public string EmployeeName { get; set; }
        public int TrxType { get; set; }
        public long SummaryID { get; set; }
        public string InterestCalculationMethod { get; set; }
        public int Duration { get; set; }
        public double DurationOverLoanDue { get; set; }
        public double DurationOverIntDue { get; set; }
        public double LoanDue { get; set; }

        public double IntDue { get; set; }
        public double CumIntCharge { get; set; }
        public double CumInterestPaid { get; set; }

        public double PrincipalLoan { get; set; }
        public double LoanRepaid { get; set; }
        public double IntCharge
        {
            get; set;
        }
        public double NewDue { get; set; }
        public string MainProductCode { get; set; }
        public int Doc { get; set; }

        public int OrgID { get; set; }
        public string accountNo { get; set; }
        public double fine { get; set; }
        public double PersonalSaving { get; set; }
        public double PersonalWithdraw { get; set; }
    }
}