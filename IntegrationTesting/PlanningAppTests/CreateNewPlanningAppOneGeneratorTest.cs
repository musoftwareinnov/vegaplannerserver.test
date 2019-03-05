using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using AutoMapper;
using ChannelAllocator.Tests.IntegrationTesting.Helpers;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using vega.Core;
using vega.Core.Models;
using vega.Persistence;
using vega.test.IntegrationTesting.Helpers;
using vega.test.IntegrationTesting.TestHelpers;
using vega.Tests.IntegrationTesting.Helpers;
using vegaplannerserver.test.TestHelpers;
using Xunit;

namespace vega.test.IntegrationTesting.PlanningAppTests
{
    public class CreateNewPlanningAppOneGeneratorTest :  IClassFixture<CreateNewPlanningAppOneGeneratorTest_WAF<vega.Startup>>
    {
        private readonly CreateNewPlanningAppOneGeneratorTest_WAF<vega.Startup> _factory;

        public CreateNewPlanningAppOneGeneratorTest(CreateNewPlanningAppOneGeneratorTest_WAF<vega.Startup> factory)
        {
            _factory = factory;
        }

        [Theory]
        [InlineData(ApiPaths.PlanningApps)] 
        public async Task Post_CreateNewPlanningAppOneGeneratorTest(string url)
        {
            // Arrange
            var client = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    //Override IService called during POST to return specific test calendar
                    //services.AddScoped<IService, Service>(); 
                });
            })
            .CreateClient();

            var testWebClient = new TestWebClient(client);
            testWebClient.Login();

            var results = testWebClient.GetPlanningApps();
            Assert.True(results.Items.Count() == 0); 

            var planningApp = testWebClient.CreatePlanningApp();
            Assert.Equal(planningApp.ProjectGeneratorName, "Test Project Generator");           
            Assert.True(planningApp.PlanningAppStates.Count() == 5); 

            //Check Ordering

            var stateList = planningApp.PlanningAppStates.ToList();
            Assert.True(stateList[0].StateName == "State:0");     
            Assert.True(stateList[4].StateName == "State:6");         
        } 
    }


    public class CreateNewPlanningAppOneGeneratorTest_WAF<TStartup> : WebApplicationFactory<vega.Startup>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            new ConfigureTestWebHost(builder);
        }

        private class ConfigureTestWebHost : AbstractConfigureWebHost
        {
            public ConfigureTestWebHost(IWebHostBuilder builder) : base(builder)
            {

            }
            override public void InitializeVegaPlannerServerDbForTests(VegaDbContext db)
            {      

                //service.InsertGenerator
                var testData = new SetupDefaultTestData(db);
                testData.CreateProjectWithOneGeneratorFiveStates();
                db.SaveChanges(); 
            }
        }
    }
}


