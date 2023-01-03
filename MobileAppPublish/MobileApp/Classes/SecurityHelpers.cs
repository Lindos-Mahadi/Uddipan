using System.Net.Http;

namespace PMS.Droid.Helpers
{
    public class SecurityHelpers
    {
        public static HttpClient ConfigHeader(HttpClient client)
        {
           // var authData = string.Format("{0}:{1}:{2}", SecurityConstants.Username, SecurityConstants.Password, AuthToken);
            //var authHeaderValue = Convert.ToBase64String(Encoding.UTF8.GetBytes(authData));

            client = new HttpClient();
            client.MaxResponseContentBufferSize = int.MaxValue;
           // client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", AuthToken);
            //client.DefaultRequestHeaders.Authorization.Scheme = "";

           // client.DefaultRequestHeaders.Add("Auth", AuthToken);
            //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authHeaderValue);
            return client;
        }
    }
}