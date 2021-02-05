using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace microservice_crm
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var response = CrmRequest(
                HttpMethod.Get,
                "https://curateurdevfr.crm3.dynamics.com/api/data/v9.2/WhoAmI")
                .Result.Content.ReadAsStringAsync();
                System.Console.WriteLine("response: "+response+", response.Result: "+response.Result);                
        }

        public static async Task<string> AccessTokenGenerator()
        {
            string clientId = "3df92374-9cc8-4a76-bc1f-5ed0896fe77e";
            string clientSecret = "~y-whJzRiRQx.qzRs9k._.MHE7R68I1t2S";
            string authority = "https://login.microsoftonline.com/f05b8ed3-3505-4c81-b0cd-e5817052c9bc";
            string resourceUrl = "https://curateurdevfr.crm3.dynamics.com";

            var credentials = new ClientCredential(clientId, clientSecret);
            var authContext = new Microsoft.IdentityModel.Clients.ActiveDirectory.AuthenticationContext(authority);
            var result = await authContext.AcquireTokenAsync(resourceUrl, credentials);

            System.Console.WriteLine("result: "+result+ ", result.AccessToken: [" + result.AccessToken + "]");
            return result.AccessToken;
        }

        public static async Task<HttpResponseMessage> CrmRequest(HttpMethod httpMethod, string requestUri, string body = null)
        {
            // Acquiring Access Token
            var accessToken = await AccessTokenGenerator();
            System.Console.WriteLine("accessToken: "+accessToken);

            var client = new HttpClient();
            var message = new HttpRequestMessage(httpMethod, requestUri);

            // OData related headers
            message.Headers.Add("OData-MaxVersion", "4.0");
            message.Headers.Add("OData-Version", "4.0");
            message.Headers.Add("Prefer", "odata.include-annotations=\"*\"");

            // Passing AccessToken in Authentication header
            message.Headers.Add("Authorization", $"Bearer {accessToken}");

            // Adding body content in HTTP request
            if (body != null)
                message.Content = new StringContent(body, UnicodeEncoding.UTF8, "application/json");

            return await client.SendAsync(message);

        }
    }
}
