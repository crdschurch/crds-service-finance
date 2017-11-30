using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Pushpay
{
    public class Client
    {
        public static async Task<string> GetOAuthToken()
        {
            Console.WriteLine("GetOAuthToken");
            var clientId = Environment.GetEnvironmentVariable("PUSHPAY_CLIENT_ID");
            var clientSecret = Environment.GetEnvironmentVariable("PUSHPAY_CLIENT_SECRET");
            var authUri = new Uri(Environment.GetEnvironmentVariable("PUSHPAY_AUTH_ENDPOINT"));

            var httpClient = new HttpClient();

            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes(clientId + ":" + clientSecret)));
            var tokenRequestMessage = new HttpRequestMessage(HttpMethod.Post, authUri);

            var body = new Dictionary<string, string> {
                {"grant_type", "client_credentials"},
                {"scope", "list_my_merchants merchant:manage_community_members merchant:view_community_members merchant:view_payments merchant:view_recurring_payments organization:manage_funds read"}
            };

            tokenRequestMessage.Content = new FormUrlEncodedContent(body);

            var tokenresponse = await httpClient.SendAsync(tokenRequestMessage);
            Console.WriteLine(tokenresponse);

            if (tokenresponse.StatusCode == HttpStatusCode.OK)
            {
                Console.WriteLine("*&%&%&#%$&$ successful! *&%&%&#%$&$");
                var tokenJson = await tokenresponse.Content.ReadAsStringAsync();
                var tokens = JsonConvert.DeserializeObject<Pushpay.OAuth2TokenResponse>(tokenJson);
                Console.WriteLine(tokens);
                return tokens.access_token;
            }
            Console.WriteLine("not successful");
            throw new Exception("Authentication was not successful");
        }
    }

    public class OAuth2TokenResponse
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public string expires_in { get; set; }
        public string refresh_token { get; set; }
    }
}
