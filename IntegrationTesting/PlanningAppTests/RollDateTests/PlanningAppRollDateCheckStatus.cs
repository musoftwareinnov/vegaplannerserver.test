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
    public class PlanningAppRollDateCheckStatus :  IClassFixture<PlanningAppRollDateCheckStatus_WAF<vega.Startup>>
    {
        private readonly PlanningAppRollDateCheckStatus_WAF<vega.Startup> _factory;
        public PlanningAppRollDateCheckStatus(PlanningAppRollDateCheckStatus_WAF<vega.Startup> factory)
        {
            _factory = factory;
        }

        [Theory]
        [InlineData(ApiPaths.PlanningApps)] 
        public async Task Post_PlanningAppRollDateCheckStatus(string url)
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
            Assert.Equal("TestGen1:State1", PA.CurrentState);
            Assert.Equal("03-01-2019", PA.ExpectedStateCompletionDate);
            Assert.Equal("15-01-2019", PA.CompletionDate);

            //Roll The Business Date To Make current state 'Due;
            BD = await testWebClient.SetBusinessDate("02-01-2019");
            var PA_ROLLED_DUE = await testWebClient.GetPlanningApp(PAL.Items.SingleOrDefault().Id);  

            //Assert Application Status is 'Due'
            Assert.Equal(StatusList.Due, PA_ROLLED_DUE.CurrentStateStatus);
            Assert.Equal("TestGen1:State1", PA_ROLLED_DUE.CurrentState);  
            //Assert First State is 'Due;
            var stateList = PA_ROLLED_DUE.PlanningAppStates.ToList();
            Assert.Equal(StatusList.Due, stateList[0].StateStatus);
            Assert.Equal("TestGen1:State1", stateList[0].StateName);  

            Assert.Equal(StatusList.OnTime, stateList[1].StateStatus);
            Assert.Equal("TestGen1:State2", stateList[1].StateName); 

            //Assert Roll dates so first state 'Overdue' and second state 'Due'
            await testWebClient.SetBusinessDate("04-01-2019");
            var PA_ROLLED_OVERDUE_DUE = await testWebClient.GetPlanningApp(PAL.Items.SingleOrDefault().Id);  
            stateList = PA_ROLLED_OVERDUE_DUE.PlanningAppStates.ToList();
            //Assert Application Status is 'Due'
            Assert.Equal(StatusList.Overdue, PA_ROLLED_OVERDUE_DUE.CurrentStateStatus);
            Assert.Equal("TestGen1:State1", PA_ROLLED_OVERDUE_DUE.CurrentState);

            Assert.Equal(StatusList.Overdue, stateList[0].StateStatus);
            Assert.Equal("TestGen1:State1", stateList[0].StateName);  

            Assert.Equal(StatusList.Due, stateList[1].StateStatus);
            Assert.Equal("TestGen1:State2", stateList[1].StateName);

            Assert.Equal(StatusList.OnTime, stateList[2].StateStatus);
            Assert.Equal("TestGen1:State3", stateList[2].StateName); 

            //Assert All States overdue except last state 'Due'
            await testWebClient.SetBusinessDate("15-01-2019");
            var PA_ROLLED_ALL_OVERDUE_EX_LAST = await testWebClient.GetPlanningApp(PAL.Items.SingleOrDefault().Id);  
            stateList = PA_ROLLED_ALL_OVERDUE_EX_LAST.PlanningAppStates.ToList();
            Assert.Equal(StatusList.Overdue, PA_ROLLED_ALL_OVERDUE_EX_LAST.CurrentStateStatus);
            Assert.Equal("TestGen1:State1", PA_ROLLED_ALL_OVERDUE_EX_LAST.CurrentState);           

            var TOT_OVERDUE = stateList.Where(s => s.StateStatus == StatusList.Overdue).Count();;
            Assert.Equal(4, TOT_OVERDUE);
            Assert.Equal(StatusList.Due, stateList[4].StateStatus);
            Assert.Equal("TestGen1:State5", stateList[4].StateName); 


            //Assert All States overdue
            await testWebClient.SetBusinessDate("16-01-2019");
            var PA_ROLLED_ALL_OVERDUE = await testWebClient.GetPlanningApp(PAL.Items.SingleOrDefault().Id);  
            stateList = PA_ROLLED_ALL_OVERDUE.PlanningAppStates.ToList();
            Assert.Equal(StatusList.Overdue, PA_ROLLED_ALL_OVERDUE.CurrentStateStatus);
            Assert.Equal("TestGen1:State1", PA_ROLLED_ALL_OVERDUE.CurrentState);           

            Assert.True(stateList.All(s => s.StateStatus == StatusList.Overdue));

        } 
    }


    public class PlanningAppRollDateCheckStatus_WAF<TStartup> : WebApplicationFactory<vega.Startup>
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


