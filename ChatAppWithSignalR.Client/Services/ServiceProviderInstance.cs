using ChatAppWithSignalR.Client.Helpers;
using ChatAppWithSignalR.Shared;
using Newtonsoft.Json;
using System.Text;

namespace ChatAppWithSignalR.Client.Services
{
    public class ServiceProviderInstance
    {
        private static ServiceProviderInstance _instance;

        public string _accessToken = "";
       
        private ServiceProviderInstance() { }

        public static ServiceProviderInstance GetInstance()
        {
            if (_instance == null)
                _instance = new ServiceProviderInstance();

            return _instance;
        }

        public async Task<AuthenticateResponse> Authenticate(AuthenticateRequest request)
        {
            var devSslHelper = new DevHttpsConnectionHelper(sslPort: 7259);

            using (HttpClient client = devSslHelper.HttpClient)
            {
                var httpRequestMessage = new HttpRequestMessage();
                httpRequestMessage.Method = HttpMethod.Post;
                httpRequestMessage.RequestUri = new Uri(devSslHelper.DevServerRootUrl + "/Authenticate/Authenticate");

                if (request != null)
                {
                    string jsonContent = JsonConvert.SerializeObject(request);
                    var httpContent = new StringContent(jsonContent, encoding: Encoding.UTF8, "application/json"); ;
                    httpRequestMessage.Content = httpContent;
                }
                try
                {
                    var response = await client.SendAsync(httpRequestMessage);
                    var responseContent = await response.Content.ReadAsStringAsync();

                    var result = JsonConvert.DeserializeObject<AuthenticateResponse>(responseContent);
                    result.StatusCode = (int)response.StatusCode;

                    if (result.StatusCode == 200)
                    {
                        _accessToken = result.Token;
                    }
                    return result;
                }
                catch (Exception ex)
                {
                    var result = new AuthenticateResponse
                    {
                        StatusCode = 500,
                        StatusMessage = ex.ToString(),
                    };
                    return result;
                }
            }
        }
    }
}