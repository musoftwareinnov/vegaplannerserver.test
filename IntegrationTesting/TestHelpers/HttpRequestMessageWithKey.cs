using System.Net.Http;

namespace vega.test.IntegrationTesting.Helpers
{
    public static class HttpRequestMessageWithKey
    {
         public static HttpRequestMessage GenRequest(HttpMethod HttpMethod, string url) {
            var request = new HttpRequestMessage(HttpMethod, url);
            request.Headers.Add("ApiAllocationKey", "PITCHED_API_KEY");
            request.Headers.Add("ApiSecurityKey", "PITCHED_API_SEC_KEY");
            
            return request;
        }   
    }
}