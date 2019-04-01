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
//     public class PlanningAppCreateThreeGensRemoveMiddleGen :  IClassFixture<PlanningAppCreateThreeGensRemoveMiddleGen_WAF<vega.Startup>>
//     {
//         private readonly PlanningAppCreateThreeGensRemoveMiddleGen_WAF<vega.Startup> _factory;
//         public PlanningAppCreateThreeGensRemoveMiddleGen(PlanningAppCreateThreeGensRemoveMiddleGen_WAF<vega.Startup> factory)
//         {
//             _factory = factory;
//         }

//         [Theory]
//         [InlineData(ApiPaths.PlanningApps)] 
//         public async Task Post_PlanningAppCreateThreeGensRemoveMiddleGen(string url)
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


//             //Loop states and check settings
//             Assert.True(testWebClient.checkListOrdering(stateList));

//             //Remove Generator
//             var Generators = await testWebClient.GetGenerators();
//             var genToRemove = Generators.Items.Where(g => g.Name == "TestGen2").SingleOrDefault();

//             await testWebClient.RemoveGeneratorFromPlanningApp(planningAppResource.Id, genToRemove.Id, OrderId:2);

//             var PAR = await testWebClient.GetPlanningApp(planningAppResource.Id);

//             Assert.True(PAR.PlanningAppStates.Count() == TestSettings.TenStates);
//             stateList = PAR.PlanningAppStates.ToList();

//             //Assert Completion date
//             Assert.Equal("29-01-2019", PAR.CompletionDate );
//             Assert.Equal("OnTime", PAR.CurrentStateStatus );
//         } 
//     }


//     public class PlanningAppCreateThreeGensRemoveMiddleGen_WAF<TStartup> : WebApplicationFactory<vega.Startup>
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


