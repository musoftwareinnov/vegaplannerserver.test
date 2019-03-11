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
//                     //services.AddScoped<IService, Service>(); 
//                 });
//             })
//             .CreateClient();

//             var testWebClient = new TestWebClient(client);
//             testWebClient.Login();

//             var results = testWebClient.GetPlanningApps();
//             Assert.True(results.Items.Count() == 0); 

//             var planningAppResource = testWebClient.CreatePlanningApp();
//             Assert.Equal(planningAppResource.ProjectGeneratorName, TestSettings.ProjectGeneratorName);           
//             Assert.True(planningAppResource.PlanningAppStates.Count() == TestSettings.FifteenStates); 

//             //Check Ordering
//             var stateList = planningAppResource.PlanningAppStates.ToList();

//             //Loop states and check settings
//             Assert.True(testWebClient.checkListOrdering(stateList));  

//             //Check new generator start flag
//             Assert.False(stateList[3].isLastGeneratorState);
//             Assert.True(stateList[4].isLastGeneratorState);
//             Assert.False(stateList[8].isLastGeneratorState);
//             Assert.True(stateList[9].isLastGeneratorState);
//             Assert.False(stateList[13].isLastGeneratorState);
//             Assert.True(stateList[14].isLastGeneratorState);
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


