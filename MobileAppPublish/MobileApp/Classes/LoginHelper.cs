using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Http;
using PMS.Droid.Helpers;
using Newtonsoft.Json;
using System.Threading.Tasks;
using GBPMS.Droid.Classes;
using PMS.Droid.Classes.OffLineHelpers;

namespace PMS.Droid.Classes
{
    public class LoginHelper
    {
        public static async Task<bool> IsUserAuthenticated(string userName, string password, string url)
        {
            HttpClient client = null;
            client = SecurityHelpers.ConfigHeader(client);
            client.Timeout = new TimeSpan(0, 5, 0);
            var currentAction = string.Format("LoanProposalAPI/GetValidateLogin?userName={0}&password={1}", userName, password);
            var uri = new Uri(string.Format(url, currentAction));
            var allowed = false;
            try
            {
                var response = await client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    allowed = JsonConvert.DeserializeObject<bool>(content);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return allowed;
        }
        public static async Task<string> GetAPIVersion(string url, int appVersion, string user)
        {
            HttpClient client = null;
            //int appVersion, string user
            client = SecurityHelpers.ConfigHeader(client);
            client.Timeout = new TimeSpan(0, 5, 0);
            var currentAction = string.Format("lookupapi/getinfoapi?appVersion={0}&user={1}", appVersion, user);// "lookupapi/GetAPIVersion";
            var uri = new Uri(string.Format(url, currentAction));
            var apiInfo = "";
            try
            {
                var response = await client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    apiInfo = JsonConvert.DeserializeObject<string>(content);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return apiInfo;
        }
        public static async Task<List<LookupItem>> GetEmployeeOfficeListListAsync(string employeeCode, string url)
        {
            HttpClient client = null;
            client = SecurityHelpers.ConfigHeader(client);

            var currentAction = string.Format("LoanProposalAPI/GetLoggedInuserOfficeList?employeeCode={0}", employeeCode);
            var uri = new Uri(string.Format(url, currentAction));
            //Console.WriteLine("OFFICE_URL: "+string.Format(url, currentAction));
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

        public static async Task PostErrorLog(List<SystemErrorModel> errorList, string url)
        {

            HttpClient client = null;
            client = SecurityHelpers.ConfigHeader(client);

            var json = JsonConvert.SerializeObject(errorList);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            client.Timeout = new TimeSpan(0, 5, 0);
            var currentAction = "CollectionAPI/PostErrorLog";
            var uri = new Uri(string.Format(url, currentAction));

            try
            {
                var response = await client.PostAsync(uri, content);
                var responseContent = await response.Content.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode)
                    throw new Exception("API Exception: " + responseContent);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}