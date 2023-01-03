using System;
using System.Collections.Generic;
using System.Text;
using PMS.Droid.Helpers;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using PMS.Droid.Classes.OffLineHelpers;

namespace PMS.Droid.Classes
{
    public class MemberDataApiHelper
    {
        public static async Task<string> CreateLoanProposal(string memberCode, string amount, int officeID, int centerID, int productID, int purposeID, string url)
        {

            var status = "Error";

            HttpClient client = null;
            client = SecurityHelpers.ConfigHeader(client);

            var currentAction = string.Format("LoanProposalAPI/GetCreateLoanProposalStatus?memberCode={0}&amount={1}&officeID={2}&centerID={3}&productID={4}&purposeID={5}", memberCode, amount, officeID, centerID, productID, purposeID);
            var uri = new Uri(string.Format(url, currentAction));

            try
            {
                var response = await client.GetAsync(uri);
                var content = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {

                    var result = JsonConvert.DeserializeObject<string>(content);
                    return result;
                }
                else
                    throw new Exception(content);

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public static async Task<string> CreateLoanCollection(string memberCode, string amount, int officeID, int centerID, int productID, string url, string loggedInUser)
        {

            var status = "Error";

            HttpClient client = null;
            client = SecurityHelpers.ConfigHeader(client);

            var currentAction = string.Format("CollectionAPI/GetCreateCollectionStatus?memberCode={0}&amount={1}&officeID={2}&centerID={3}&productID={4}&loggedInUser={5}", memberCode, amount, officeID, centerID, productID, loggedInUser);
            var uri = new Uri(string.Format(url, currentAction));

            try
            {
                var response = await client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<string>(content);
                    return result;
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return status;

        }

        public static async Task<List<long>> PostMobileCollection(CollectionAPIModel apiRequest, string url, string loggedInUser)
        {

            //var status = "Error";
            //var newList = collectionItems.Select(s => new { s.CollectionID, s.CenterID, s.MemberID, s.Amount, s.OfficeID, s.ProductID, loggedInUser = loggedInUser, Token = s.CollectionGUID, Sid = s.SummaryID, TType = s.TrxType, PType = s.ProductType, ApiVersion = OfflineDBConstants.APP_DATABASE_VERSION }).ToList();
            HttpClient client = null;
            client = SecurityHelpers.ConfigHeader(client);

            var json = JsonConvert.SerializeObject(apiRequest);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var syncHelper = new OrganizationUrlOfflineHelper(Android.App.Application.Context);
            // Console.WriteLine("JSON CONTENT:" + json);
            var orgUrl = "";
            if (url.Length > 3)
            {   // for test server with port 8444
                orgUrl = url;
            }
            else if(url.Length <= 3)
            {   // for live server
                var org = syncHelper.Get();
                if (org!= null && org.OrganizationUrl.Contains("8882"))
                {   // api for buro south
                    orgUrl = "http://apiburosouth.gbanker.tech:8885/api/{0}";
                }
                else
                {
                    // api for buro
                    orgUrl = "http://buro.ghrmplus.com/api/{0}";
                }
                
            }
             
            

            client.Timeout = new TimeSpan(0, 5, 0);
            
            var currentAction = "CollectionAPI/PostMobileCollection";
           // Console.WriteLine("JSON CONTENT" + string.Format(_url, currentAction));
            var uri = new Uri(string.Format(orgUrl, currentAction));
            
            try
            {
                var response = await client.PostAsync(uri, content);
                var responseContent = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<List<long>>(responseContent);
                    return result;
                }
                else
                    throw new Exception("API Exception: " + responseContent);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}