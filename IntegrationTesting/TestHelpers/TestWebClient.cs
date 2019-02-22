using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using vega.Controllers.Resources;
using vega.Core.Models;
using vega.Tests.IntegrationTesting.Helpers;

namespace vega.test.IntegrationTesting.TestHelpers
{
    public class TestWebClient
    {
        HttpClient _client;
        AuthToken Token;

        public TestWebClient(HttpClient httpClient) {
            _client = httpClient;
            _client.DefaultRequestHeaders
            .Accept
            .Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }   

        public void Login() {
            var postParams = new Dictionary<string, string> { { "userName", "adminuser@gmail.com" }, { "password", "admpassword" } };
            Token = MakeRequest<AuthToken>("POST", _client, ApiPaths.AuthLoginApi, postParams);
            //Authorise all calls to the server
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", Token.authToken); 
        }

        private void MakeRequest<T>(string v, object client, string authLoginApi, Dictionary<string, string> postParams)
        {
            throw new NotImplementedException();
        }

        public QueryResultResource<PlanningAppSummaryResource> GetPlanningApps() {    
            var res = MakeRequest<QueryResultResource<PlanningAppSummaryResource>>("GET", _client, ApiPaths.PlanningApps, new Dictionary<string, string>());
            return res;
        }
        public PlanningAppResource CreatePlanningApp() {  

            var body = new Dictionary<string, string>();
            body.Add("CustomerId", "1");
            body.Add("ProjectGeneratorId", "1");
            var res = MakeRequest<PlanningAppResource>("POST", _client, ApiPaths.PlanningApps, body);
            return res;
        }


        private static T MakeRequest<T>(string httpMethod, HttpClient client, string route, Dictionary<string, string> postParams = null)
        {
            HttpRequestMessage requestMessage = new HttpRequestMessage(new HttpMethod(httpMethod), $"{route}");
            
            var bodyJson = JsonConvert.SerializeObject(postParams);
            if (postParams != null) {
                requestMessage.Content = new StringContent(bodyJson,  Encoding.UTF8, "application/json");   // This is where your content gets added to the request body
            }
            HttpResponseMessage response = client.SendAsync(requestMessage).Result;

            string apiResponse = response.Content.ReadAsStringAsync().Result;
            try
            {
                // Attempt to deserialise the reponse to the desired type, otherwise throw an expetion with the response from the api.
                if (apiResponse != "") {
                    var t = JsonConvert.DeserializeObject<T>(apiResponse);
                    return JsonConvert.DeserializeObject<T>(apiResponse);
                }
                else
                    throw new Exception();
            }
            catch (Exception ex)
            {
                throw new Exception($"An error ocurred while calling the API. It responded with the following message: {response.StatusCode} {response.ReasonPhrase}");
            }
        }
    }
}