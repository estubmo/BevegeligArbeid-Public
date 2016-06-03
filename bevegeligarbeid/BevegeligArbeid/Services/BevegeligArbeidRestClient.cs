// =====================================================
// AUTHOR: Triona AS
// NOTES:
//======================================================
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using TheWallClient.PCL.Service;

namespace BevegeligArbeid.Services
{
    public class BevegeligArbeidRestClient
    {
        private readonly IAccessTokenProvider accessTokenProvider;

        private readonly string baseUrl;

        public BevegeligArbeidRestClient(IAccessTokenProvider accessTokenProvider, string baseUrl)
        {
            this.accessTokenProvider = accessTokenProvider;
            this.baseUrl = baseUrl;
        }

        public HttpResponseMessage Send(string path, HttpMethod method, string payload)
        {
            var url = this.baseUrl + path;
            using (var handler = new HttpClientHandler())
            using (var httpClient = new HttpClient(handler))
            using (var request = new HttpRequestMessage(method, url))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("bearer", this.accessTokenProvider.AccessToken);
                if (payload != null)
                {
                    request.Content = new StringContent(payload, Encoding.UTF8, "application/json");
                }


                return httpClient.SendAsync(request).Result;
            }
        }
    }
}
