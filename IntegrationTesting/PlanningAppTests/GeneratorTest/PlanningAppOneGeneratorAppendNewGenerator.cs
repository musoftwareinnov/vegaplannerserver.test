
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
//     public class PlanningAppOneGeneratorAppendNewGenerator :  IClassFixture<PlanningAppOneGeneratorAppendNewGenerator_WAF<vega.Startup>>
//     {
//         private readonly PlanningAppOneGeneratorAppendNewGenerator_WAF<vega.Startup> _factory;
//         public PlanningAppOneGeneratorAppendNewGenerator(PlanningAppOneGeneratorAppendNewGenerator_WAF<vega.Startup> factory)
//         {
//             _factory = factory;
//         }

//         [Theory]
//         [InlineData(ApiPaths.PlanningApps)] 
//         public async Task Post_PlanningAppOneGeneratorAppendNewGenerator(string url)
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
//             Assert.True(planningAppResource.PlanningAppStates.Count() == TestSettings.OneState); 

//             //Check Ordering
//             var stateList = planningAppResource.PlanningAppStates.ToList();

//             //Check Planning App Details
//             Assert.Equal("03-01-2019", planningAppResource.CompletionDate );
//             Assert.Equal("OnTime", planningAppResource.CurrentStateStatus );
//             Assert.Equal("03-01-2019", stateList[0].DueByDate);

//             //Loop states and check settings
//             Assert.True(testWebClient.checkListOrdering(stateList));

//             //Check new generator start flag
//             Assert.True(stateList[0].isLastGeneratorState);
//             Assert.Equal("OnTime", stateList[0].StateStatus);
//             Assert.True(stateList[0].CurrentState);

//             //Add A new generator to the end and check states
//             var Generators = await testWebClient.GetGenerators();
//             var genToAdd = Generators.Items.Where(g => g.Name == "Append Generator End").SingleOrDefault();

//             await testWebClient.AddGeneratorToPlanningApp(planningAppResource.Id, genToAdd.Id, OrderId:2);

//             var PAR = await testWebClient.GetPlanningApp(planningAppResource.Id);

//             stateList = PAR.PlanningAppStates.ToList();

//             //Assert Completion date updated to reflect new generator
//             Assert.Equal("07-01-2019", PAR.CompletionDate );
//             Assert.Equal("OnTime", PAR.CurrentStateStatus );

//             Assert.True(stateList[0].isLastGeneratorState);
//             Assert.Equal("OnTime", stateList[0].StateStatus);
//             Assert.Equal("03-01-2019", stateList[0].DueByDate);
//             Assert.True(stateList[0].CurrentState);    
//             Assert.True(stateList[1].isLastGeneratorState);
//             Assert.Equal("OnTime", stateList[1].StateStatus);
//             Assert.Equal("07-01-2019", stateList[1].DueByDate);
//             Assert.False(stateList[1].CurrentState);             

//         } 
//     }


//     public class PlanningAppOneGeneratorAppendNewGenerator_WAF<TStartup> : WebApplicationFactory<vega.Startup>
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
//                 testData.CreateProjectGeneratorsStates(noOfGenerators:1, noOfStates:TestSettings.OneState);

//                 //Generator to be inserted
//                 var GN = GeneratorInitialisers.createGenerator("Append Generator End", nOfStates:1, noOfDays:2, alertDays:1);
//                 db.Add(GN);
//                 db.SaveChanges(); 
//             }
//         }
//     }
// }


