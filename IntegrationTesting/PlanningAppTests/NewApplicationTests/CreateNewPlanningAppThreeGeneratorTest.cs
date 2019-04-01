// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Net;
// using System.Net.Http;
// using System.Net.Http.Headers;
// using System.Threading.Tasks;
// using AutoMapper;
// using ChannelAllocator.Tests.IntegrationTesting.Helpers;
// using Microsoft.AspNetCore;
// using Microsoft.AspNetCore.Hosting;
// using Microsoft.AspNetCore.Mvc.Testing;
// using Microsoft.AspNetCore.TestHost;
// using Microsoft.EntityFrameworkCore;
// using Microsoft.Extensions.DependencyInjection;
// using Microsoft.Extensions.Logging;
// using Newtonsoft.Json;
// using vega.Core;
// using vega.Core.Models;
// using vega.Persistence;
// using vega.Services.Interfaces;
// using vega.test.IntegrationTesting.Helpers;
// using vega.test.IntegrationTesting.TestHelpers;
// using vega.Tests.IntegrationTesting.Helpers;
// using vegaplannerserver.test.IntegrationTesting.TestHelpers;
// using vegaplannerserver.test.TestHelpers;
// using Xunit;

// namespace vega.test.IntegrationTesting.PlanningAppTests
// {
//     public class CreateNewPlanningAppThreeGeneratorTest :  IClassFixture<CreateNewPlanningAppThreeGeneratorTest_WAF<vega.Startup>>
//     {
//         private readonly CreateNewPlanningAppThreeGeneratorTest_WAF<vega.Startup> _factory;
//         public CreateNewPlanningAppThreeGeneratorTest(CreateNewPlanningAppThreeGeneratorTest_WAF<vega.Startup> factory)
//         {
//             _factory = factory;
//         }

//         [Theory]
//         [InlineData(ApiPaths.PlanningApps)] 
//         public async Task Post_CreateNewPlanningAppThreeGeneratorTest(string url)
//         {
//             // Arrange
//             var client = _factory.WithWebHostBuilder(builder =>
//             {
//                 builder.ConfigureTestServices(services =>
//                 {
//                     //Override IService called during POST to return specific test calendar
//                     services.AddScoped<IDateService, DateServiceTest>();
//                 });
//             })
//             .CreateClient();

//             var testWebClient = new TestWebClient(client);
//             testWebClient.Login();

//             Console.WriteLine($"Calling Api Endpoint {url} ");
//             var results = await testWebClient.GetPlanningApps();
//             Assert.True(results.Items.Count() == 0); 

//             var projectGenerators = await testWebClient.GetProjectGenerator();
//             var pg = projectGenerators.Items.FirstOrDefault();
//             var planningAppResource = await testWebClient.CreatePlanningApp(pg.Id);
//             Assert.Equal(planningAppResource.ProjectGeneratorName, TestSettings.ProjectGeneratorName);           
//             Assert.True(planningAppResource.PlanningAppStates.Count() == TestSettings.FifteenStates); 

//             //Check Ordering
//             var stateList = planningAppResource.PlanningAppStates.ToList();

//             //Check Planning App Details
//             Assert.Equal("12-02-2019", planningAppResource.CompletionDate );
//             Assert.Equal("OnTime", planningAppResource.CurrentStateStatus );
//             Assert.Equal("03-01-2019", stateList[0].DueByDate);
//             Assert.Equal("OnTime", stateList[0].StateStatus);

//             //Loop states and check settings
//             Assert.True(testWebClient.checkListOrdering(stateList)); 

//             //Check new generator start flag
//             Assert.False(stateList[3].isLastGeneratorState);
//             Assert.True(stateList[4].isLastGeneratorState);
//             Assert.False(stateList[8].isLastGeneratorState);
//             Assert.True(stateList[9].isLastGeneratorState);
//             Assert.False(stateList[13].isLastGeneratorState);
//             Assert.True(stateList[14].isLastGeneratorState);
//             Assert.Equal("12-02-2019", stateList[14].DueByDate);          
//             Assert.Equal("OnTime", stateList[14].StateStatus);          
//         } 
//     }


//     public class CreateNewPlanningAppThreeGeneratorTest_WAF<TStartup> : WebApplicationFactory<vega.Startup>
//     {
//         protected override void ConfigureWebHost(IWebHostBuilder builder)
//         {
//             new ConfigureTestWebHost(builder);
//         }

//         private class ConfigureTestWebHost : AbstractConfigureWebHost
//         {
//             public ConfigureTestWebHost(IWebHostBuilder builder) : base(builder)
//             {

//             }
//             override public void InitializeVegaPlannerServerDbForTests(VegaDbContext db)
//             {      

//                 //service.InsertGenerator
//                 var testData = new SetupDefaultTestData(db);
//                 testData.CreateCustomer("TestUser1");
//                 testData.CreateProjectGeneratorsStates(noOfGenerators:3, noOfStates:TestSettings.FiveStates);
//                 db.SaveChanges(); 
//             }
//         }
//     }
// }


