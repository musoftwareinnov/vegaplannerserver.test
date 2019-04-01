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
using vega.Core.Models.States;
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
    public class PlanningAppNextStateTests :  IClassFixture<PlanningAppNextStateTests_WAF<vega.Startup>>
    {
        private readonly PlanningAppNextStateTests_WAF<vega.Startup> _factory;
        public PlanningAppNextStateTests(PlanningAppNextStateTests_WAF<vega.Startup> factory)
        {
            _factory = factory;
        }

        [Theory]
        [InlineData(ApiPaths.PlanningApps)] 
        public async Task Post_PlanningAppNextStateTests(string url)
        {
            var f = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    //Override IService called during POST to return specific test calendar
                    //services.AddScoped<IDateService, DateServiceTest>(); 
                });
            });

            
            // Arrange
            var client = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    //Override IService called during POST to return specific test calendar
                    //services.AddScoped<IDateService, DateServiceTest>(); 
                });
            })
            .CreateClient();

            var testWebClient = new TestWebClient(client);
            testWebClient.Login();

            Console.WriteLine($"Calling Api Endpoint {url} ");
            var BD = await testWebClient.SetBusinessDate("01-01-2019");

            var projectGenerators = await testWebClient.GetProjectGenerator();
            var pg = projectGenerators.Items.FirstOrDefault();
            await testWebClient.CreatePlanningApp(pg.Id);

            var PAL = await testWebClient.GetPlanningApps();
            Assert.True(PAL.Items.Count() == 1);
            Assert.True(PAL.TotalItems == 1);  //Check Paging Is Working

            var PA = await testWebClient.GetPlanningApp(PAL.Items.SingleOrDefault().Id);

            Assert.Equal(StatusList.OnTime, PA.CurrentStateStatus);
            Assert.Equal(StatusList.AppInProgress, PA.PlanningStatus);
            Assert.Equal("TestGen1:State1", PA.CurrentState);
            Assert.Equal("03-01-2019", PA.ExpectedStateCompletionDate);
            Assert.Equal("15-01-2019", PA.CompletionDate);

            var NEXT_STATE_PA = await testWebClient.NextState(PA.Id);
            Assert.Equal(StatusList.OnTime, NEXT_STATE_PA.CurrentStateStatus);
            Assert.Equal("TestGen1:State2", NEXT_STATE_PA.CurrentState);
            Assert.Equal("TestGen1:State3", NEXT_STATE_PA.NextState);
            Assert.Equal("07-01-2019", NEXT_STATE_PA.ExpectedStateCompletionDate);
            Assert.Equal("15-01-2019", NEXT_STATE_PA.CompletionDate);

            var stateList = NEXT_STATE_PA.PlanningAppStates.ToList();
            Assert.Equal(StatusList.Complete, stateList[0].StateStatus);
            Assert.Equal("TestGen1:State1", stateList[0].StateName);    
            Assert.Equal("01-01-2019", stateList[0].DateCompleted);                 
            Assert.Equal(StatusList.OnTime, stateList[1].StateStatus);
            Assert.Equal("TestGen1:State2", stateList[1].StateName);  

            var COMPLETE_ALL_BUT_LAST_STATE_PA = await testWebClient.NextState(PA.Id);                      
            COMPLETE_ALL_BUT_LAST_STATE_PA = await testWebClient.NextState(PA.Id);                      
            COMPLETE_ALL_BUT_LAST_STATE_PA = await testWebClient.NextState(PA.Id);                      
            Assert.Equal(StatusList.OnTime, COMPLETE_ALL_BUT_LAST_STATE_PA.CurrentStateStatus);
            Assert.Equal("TestGen1:State5", COMPLETE_ALL_BUT_LAST_STATE_PA.CurrentState);
            //Assert All other States are Complete
            var stateListCtr = COMPLETE_ALL_BUT_LAST_STATE_PA.PlanningAppStates.ToList().Where(s => s.StateStatus == StatusList.Complete);
            Assert.Equal(4, stateListCtr.Count());

            Assert.Null(COMPLETE_ALL_BUT_LAST_STATE_PA.NextState);

            //Assert Complete State
            var COMPLETE_PA = await testWebClient.NextState(PA.Id);  
            Assert.Equal(StatusList.Complete, COMPLETE_PA.PlanningStatus);
            Assert.Null(COMPLETE_PA.CurrentState);                        
            Assert.Null(COMPLETE_PA.CurrentStateStatus);                        

            //Roll The Business Date To Make current state 'Due;
            // BD = await testWebClient.SetBusinessDate("02-01-2019");
            // var PA_ROLLED_DUE = await testWebClient.GetPlanningApp(PAL.Items.SingleOrDefault().Id);  
        } 
    }


    public class PlanningAppNextStateTests_WAF<TStartup> : WebApplicationFactory<vega.Startup>
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
                testData.CreateProjectGeneratorsStates(noOfGenerators:1, noOfStates:TestSettings.FiveStates);
                db.SaveChanges(); 
            }
        }
    }
}


