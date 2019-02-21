using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using vega.Core.Models;
using vega.Persistence;
using vega.test.IntegrationTesting.Helpers;
using vega.test.IntegrationTesting.TestHelpers;
using vega.Tests.IntegrationTesting.Helpers;
using Xunit;

namespace vega.test.IntegrationTesting.PlanningAppTests
{
    // public class CreateNewPlanningAppOneGeneratorTest
    // {
        
    // }
    public class CreateNewPlanningAppOneGeneratorTest :  IClassFixture<CreateNewPlanningAppOneGeneratorTest_WAF<vega.Startup>>
    {
        private readonly CreateNewPlanningAppOneGeneratorTest_WAF<vega.Startup> _factory;
        public CreateNewPlanningAppOneGeneratorTest(CreateNewPlanningAppOneGeneratorTest_WAF<vega.Startup> factory)
        {
            _factory = factory;
        }

        [Theory]
        [InlineData(Literals.PlanningApps)] 
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
            // testWebClient.CreatePlanningApp();

            //Assert.True(results.Items.Count() == 0);      
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
                // var testData = new SetupDefaultData(db);

                // testData.Pitched_And_External_Channel_Unit();
                // var externalAllocation = new ChannelAllocation() {
                //                         Status = AllocationStatus.Cancelled,
                //                         ArrivalDate = DateTime.Parse("05/04/2019"),
                //                         DepartureDate = DateTime.Parse("10/04/2019"),
                // };

                // var externalAllocations = new List<ChannelAllocation> {
                //     new ChannelAllocation() {
                //     Status = AllocationStatus.Cancelled,
                //     ArrivalDate = DateTime.Parse("05/04/2019"),
                //     DepartureDate = DateTime.Parse("10/04/2019"),
                //     }
                // };

                // testData.External_Add_Allocation(externalAllocations);

                // db.SaveChanges(); 
            }
        }
    }
}


