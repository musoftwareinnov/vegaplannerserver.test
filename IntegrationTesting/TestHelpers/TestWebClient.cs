using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using vega.Controllers.Resources;
using vega.Controllers.Resources.StateInitialser;
using vega.Core.Models;
using vega.Tests.IntegrationTesting.Helpers;
using vegaplannerserver.Controllers.Resources;
using vegaplannerserver.test.IntegrationTesting.TestHelpers;

namespace vega.test.IntegrationTesting.TestHelpers
{
    public class TestWebClient
    {
        HttpClient _client;
        AuthToken Token;

        public readonly string ProjectGeneratorName = "Test Project Generator";

        public TestWebClient(HttpClient httpClient) {
            _client = httpClient;
            _client.DefaultRequestHeaders
            .Accept
            .Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }   

        public void Login() {
            var postParams = new Dictionary<string, string> { { "userName", "adminuser@gmail.com" }, { "password", "admpassword" } };
            Token = MakeRequest<AuthToken>("POST", _client, ApiPaths.AuthLoginApi, postParams).Result;
            //Authorise all calls to the server
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", Token.authToken); 
        }

        private void MakeRequest<T>(string v, object client, string authLoginApi, Dictionary<string, string> postParams)
        {
            throw new NotImplementedException();
        }

        public async Task<BusinessDateResource> SetBusinessDate(string date) {   
            var apiPath = ApiPaths.BusinessDates + '/' + date; 
            var res = await MakeRequest<BusinessDateResource>("PUT", _client, apiPath, new Dictionary<string, string>());
            return res;
        }

        public async Task<QueryResultResource<PlanningAppSummaryResource>> GetPlanningApps() {    
            var res = await MakeRequest<QueryResultResource<PlanningAppSummaryResource>>("GET", _client, ApiPaths.PlanningApps, new Dictionary<string, string>());
            return res;
        }
        public async Task<PlanningAppResource> GetPlanningApp(int id) {   
            var apiPath = ApiPaths.PlanningApps + '/' + id.ToString(); 
            var res = await MakeRequest<PlanningAppResource>("GET", _client, apiPath, new Dictionary<string, string>());
            return res;
        }
        
        public async Task<PlanningAppResource> NextState(int id) {   
            var apiPath = ApiPaths.NextState +'/' + id.ToString(); 
            var res = await MakeRequest<PlanningAppResource>("PUT", _client, apiPath, new Dictionary<string, string>());
            return res;
        }
        public async Task<QueryResultResource<ProjectGeneratorResource>> GetProjectGenerator() {    
            var res = await MakeRequest<QueryResultResource<ProjectGeneratorResource>>("GET", _client, ApiPaths.ProjectGenerators, new Dictionary<string, string>());
            return res;
        }
        public async Task<QueryResultResource<StateInitialiserResource>> GetGenerators() {    
            var res = await MakeRequest<QueryResultResource<StateInitialiserResource>>("GET", _client, ApiPaths.Generators, new Dictionary<string, string>());
            return res;
        }


        public async Task<PlanningAppResource> AddGeneratorToPlanningApp(int AppId, int GenId, int OrderId) {  

            var body = new Dictionary<string, string>();
            body.Add("OrderId", OrderId.ToString());
            body.Add("GeneratorId", GenId.ToString());

            var apiPath = ApiPaths.AddGenerator + '/' + AppId.ToString();
            var res = await MakeRequest<PlanningAppResource>("PUT", _client, apiPath, body);
            return res;
        }
        public async Task<PlanningAppResource> RemoveGeneratorFromPlanningApp(int AppId, int GenId, int OrderId) {  

            var body = new Dictionary<string, string>();
            body.Add("OrderId", OrderId.ToString());
            body.Add("GeneratorId", GenId.ToString());

            var apiPath = ApiPaths.RemoveGenerator + '/' + AppId.ToString();
            var res = await MakeRequest<PlanningAppResource>("PUT", _client, apiPath, body);
            return res;
        }
        public async Task<PlanningAppResource> CreatePlanningApp(int pgId) {  

            var body = new Dictionary<string, string>();
            body.Add("CustomerId", "1");
            body.Add("ProjectGeneratorId", pgId.ToString());
            var res = await MakeRequest<PlanningAppResource>("POST", _client, ApiPaths.PlanningApps, body);
            return res;
        }

        public bool checkListOrdering(List<PlanningAppStateResource> stateList)
        {
            int genOrder = stateList.FirstOrDefault().GeneratorOrder;
            int genNumber = 1, stateNo = 1;
            foreach(var state in stateList) {
                if(genOrder != state.GeneratorOrder) {
                    genNumber++; stateNo = 1;
                    genOrder = state.GeneratorOrder;
                }
                var stateName = TestSettings.GeneratorPrefixName + genNumber + ":" + "State" + stateNo;
                if (!state.StateName.Equals(stateName)) {
                    return false;
                }
                stateNo++;
            }   
            return true;
        }

        private static async Task<T> MakeRequest<T>(string httpMethod, HttpClient client, string route, Dictionary<string, string> postParams = null)
        {
            HttpRequestMessage requestMessage = new HttpRequestMessage(new HttpMethod(httpMethod), $"{route}");
            
            var bodyJson = JsonConvert.SerializeObject(postParams);
            if (postParams != null) {
                requestMessage.Content = new StringContent(bodyJson,  Encoding.UTF8, "application/json");   // This is where your content gets added to the request body
            }
            HttpResponseMessage response = await client.SendAsync(requestMessage);

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
            throw new Exception($"An error ocurred while calling the API. It responded with the following message: {response.StatusCode} {response.ReasonPhrase} {ex.Message}");
            }
        }
    }
}