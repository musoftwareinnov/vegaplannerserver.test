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
using vega.Services.Interfaces;
using vega.test.IntegrationTesting.Helpers;
using vega.test.IntegrationTesting.TestHelpers;
using vega.Tests.IntegrationTesting.Helpers;
using vegaplannerserver.test.IntegrationTesting.TestHelpers;
using vegaplannerserver.test.TestHelpers;
using Xunit;

namespace vega.test.IntegrationTesting.PlanningAppTests
{
    public class PlanningAppsInProgressTensApps :  IClassFixture<PlanningAppsInProgressTensApps_WAF<vega.Startup>>
    {
        private readonly PlanningAppsInProgressTensApps_WAF<vega.Startup> _factory;
        public PlanningAppsInProgressTensApps(PlanningAppsInProgressTensApps_WAF<vega.Startup> factory)
        {
            _factory = factory;
        }

        [Theory]
        [InlineData(ApiPaths.PlanningApps)] 
        public async Task Post_PlanningAppsInProgressTensApps(string url)
        {
            // Arrange
            var client = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    //Override IService called during POST to return specific test calendar
                    services.AddScoped<IDateService, DateServiceTest>(); 
                });
            })
            .CreateClient();

            var testWebClient = new TestWebClient(client);
            testWebClient.Login();

            Console.WriteLine($"Calling Api Endpoint {url} ");

            var projectGenerators = await testWebClient.GetProjectGenerator();
            var pg = projectGenerators.Items.FirstOrDefault();
            await testWebClient.CreatePlanningApp(pg.Id);
            await testWebClient.CreatePlanningApp(pg.Id);
            await testWebClient.CreatePlanningApp(pg.Id);
            await testWebClient.CreatePlanningApp(pg.Id);
            await testWebClient.CreatePlanningApp(pg.Id);
            await testWebClient.CreatePlanningApp(pg.Id);
            await testWebClient.CreatePlanningApp(pg.Id);
            await testWebClient.CreatePlanningApp(pg.Id);
            await testWebClient.CreatePlanningApp(pg.Id);
            await testWebClient.CreatePlanningApp(pg.Id);
            await testWebClient.CreatePlanningApp(pg.Id);

            var PAL = await testWebClient.GetPlanningApps();
            Assert.True(PAL.Items.Count() == 10);
            Assert.True(PAL.TotalItems == 11);  //Check Paging Is Working

        } 
    }


    public class PlanningAppsInProgressTensApps_WAF<TStartup> : WebApplicationFactory<vega.Startup>
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
                testData.CreateCustomer("TestUser1");
                testData.CreateProjectGeneratorsStates(noOfGenerators:1, noOfStates:TestSettings.FiftyStates);
                db.SaveChanges(); 
            }
        }
    }
}


