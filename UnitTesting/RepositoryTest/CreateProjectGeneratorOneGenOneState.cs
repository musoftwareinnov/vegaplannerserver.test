// using System.IO;
// using vegaplannerserver.test.TestHelpers;
// using System;
// using System.Configuration;
// using System.Linq;
// using System.Threading.Tasks;
// using AutoMapper;
// using Microsoft.AspNetCore;
// using Microsoft.AspNetCore.Hosting;
// using Microsoft.AspNetCore.Mvc.Testing;
// using Microsoft.EntityFrameworkCore;
// using Microsoft.Extensions.DependencyInjection;
// using Newtonsoft.Json;
// using Microsoft.Extensions.Configuration;
// using Microsoft.Extensions.Logging;
// using vega.Services.Interfaces;
// using vega.Controllers.Resources;
// using Xunit;
// using vega.Core;
// using vega.Persistence;
// using vega.Core.Models;
// using vega.Core.Models.States;

// namespace vegaplannerserver.test.UnitTesting.PlanningAppTests
// {
//     public class PlanningAppCreate
//     {
//         private DependencyResolverHelper _serviceProvider;
//         public PlanningAppCreate()
//         {
//             //var config = InitConfiguration();
//             var webHost = WebHost.CreateDefaultBuilder()
//                 .UseStartup<vega.Startup>()
//                 .UseContentRoot(Directory.GetCurrentDirectory())
//                 .Build();
//             _serviceProvider = new DependencyResolverHelper(webHost);
//         }



//         [Fact]
//         public async void CanAddGeneratorsToProjectGenerator()
//         {
//             await WithTestDatabase.Run(async (VegaDbContext context) => {
//                 var PGR = new ProjectGeneratorRepository(context); 
//                 var GNR = new StateInitialiserRepository(context);

//                 var PG = new ProjectGenerator();
//                 PG.Name = "Project Gen 1";
                
//                 PGR.Add(PG);
//                 context.SaveChanges();

//                 var PG_QR = await PGR.GetProjectGenerators();
//                 Assert.Equal(1, PG_QR.TotalItems);

//                 //Add Generator
//                 var GN = GeneratorInitialisers.createGenerator("GN 1", 5);
//                 PGR.AppendGenerator(PG , GN);
//                 context.SaveChanges();   
//                 Assert.Equal(1, PG.Generators[0].SeqId);

//                 //Add 2 Generator
//                 GN = GeneratorInitialisers.createGenerator("GN 2", 5);
//                 PGR.AppendGenerator(PG , GN);
//                 context.SaveChanges();   
//                 Assert.Equal(2, PG.Generators[1].SeqId);    

//                 //Add 3 Generator
//                 GN = GeneratorInitialisers.createGenerator("GN 3", 5);
//                 PGR.AppendGenerator(PG , GN);
//                 context.SaveChanges();   
//                 Assert.Equal(3, PG.Generators[2].SeqId);                

//                 //Insert Generator
//                 GN = GeneratorInitialisers.createGenerator("INSERT GN 4", 5);               
//                 PGR.InsertGenerator(PG , GN, 2);                

//                 context.SaveChanges();  

//                 var upPG = await PGR.GetProjectGenerator(PG.Id);

//                 Assert.Equal(1, PG.Generators[0].SeqId);                 
//                 Assert.Equal(2, PG.Generators[1].SeqId);                 
//                 Assert.Equal(3, PG.Generators[2].SeqId);                 
//                 Assert.Equal(4, PG.Generators[3].SeqId);    

//                 //Check second generator is new one inserted
//                 Assert.Equal("INSERT GN 4", PG.Generators[1].Generator.Name);                             

//                  //Insert Generator At Beginning
//                 GN = GeneratorInitialisers.createGenerator("INSERT GN 5", 50);               
//                 PGR.InsertGenerator(PG , GN, 1);               
//                 context.SaveChanges();

//                 upPG = await PGR.GetProjectGenerator(PG.Id);
//                 Assert.Equal(1, PG.Generators[0].SeqId);                 
//                 Assert.Equal(2, PG.Generators[1].SeqId);                 
//                 Assert.Equal(3, PG.Generators[2].SeqId);                 
//                 Assert.Equal(4, PG.Generators[3].SeqId);                 
//                 Assert.Equal(5, PG.Generators[4].SeqId);                 

//                 Assert.Equal("INSERT GN 5", PG.Generators[0].Generator.Name); 
//                 Assert.Equal("INSERT GN 4", PG.Generators[2].Generator.Name); 

//                 //Apend 6 Generator
//                 GN = GeneratorInitialisers.createGenerator("APPEND GN 6", 5);
//                 PGR.AppendGenerator(PG , GN);
//                 context.SaveChanges(); 

//                 upPG = await PGR.GetProjectGenerator(PG.Id);
//                 Assert.Equal(6, PG.Generators[5].SeqId);                

//                 //Get List Of Project Generators
//                 var PGL = await PGR.GetProjectGenerators();
//                 Assert.Equal(1, PGL.TotalItems);

//                 var PG2 = new ProjectGenerator();
//                 PG2.Name = "Project Gen 1";
                
//                 PGR.Add(PG2);
//                 context.SaveChanges();
//                 PGL = await PGR.GetProjectGenerators();
//                 Assert.Equal(2, PGL.TotalItems);

//                 //Test Remove Generator
//                 upPG = await PGR.GetProjectGenerator(PG.Id);
//                 var rmGN = PG.Generators[0].Generator;                 

//                 PGR.RemoveGenerator(PG, rmGN);
//                 Assert.Equal(1, PG.Generators[0].SeqId); 
//                 Assert.Equal(5, PG.Generators.ToList().Count);

//                 Assert.Null(PG.Generators.Where(g => g.Generator.Name == "INSERT GN 5").SingleOrDefault());
//                 context.SaveChanges();
//             });
//         }
//     }

//     public class WithTestDatabase
//     {
//         public static async Task Run(Func<VegaDbContext,Task> testFunc)
//         {
//             var options = new DbContextOptionsBuilder<VegaDbContext>()
//                 .UseInMemoryDatabase("IN_MEMORY_DATABASE")
//                 .Options;

//             using (var context = new VegaDbContext(options)) {
//                 try {
//                     await context.Database.EnsureCreatedAsync();
//                     //PrepareTestDatabase(context);
//                     await testFunc(context);
//                 } 
//                 catch (Exception e){
//                     throw e; 
//                 }
//                 finally {
//                     //CleanupTestDatabase(context);
//                 }
//             }
//         }
//         public static void CleanupTestDatabase(VegaDbContext context)
//         {
//             if (context.Database.IsInMemory())
//             {
//                 context.Database.EnsureDeleted();
//             }
//             else
//             {
//                 if (context.Database.IsSqlServer())
//                 {
//                     context.Database.ExecuteSqlCommand("DELETE FROM ProjectGenerators");
//                 }
//             }
//         }

//         static void PrepareTestDatabase(VegaDbContext context) {

//                 // var stateInitialiserRepository = new StateInitialiserRepository(context);
//                 // var stateInitialiser1 = new StateInitialiser();
//                 // stateInitialiser1.Name = "Test Generator 1 - One State";

//                 // var state1 = new StateInitialiserState();
//                 // state1.Name = "State One";
//                 // state1.OrderId = 0;
//                 // state1.AlertToCompletionTime = 2;

//                 // stateInitialiser1.States.Add(state1);

          
//                 // stateInitialiserRepository.Add(stateInitialiser1);
//                 // context.SaveChangesAsync();

//                 // var si = stateInitialiserRepository.GetStateInitialiser(stateInitialiser1.Id).Result;

//                 // var projectGeneratorRepository = new ProjectGeneratorRepository(context);                
//                 // var projectGenerator = new ProjectGenerator();

//                 // projectGenerator.Name = "Project Gen 1";
//                 // projectGenerator.Generators.Add(si);

//                 // projectGeneratorRepository.Add(projectGenerator);
//                 // context.SaveChangesAsync();

            
//         }
//     }

// }

//         // public static IConfiguration InitConfiguration()
//         // {
//         //     var config = new ConfigurationBuilder()
//         //         .AddJsonFile("appsettings.test.json")
//         //         .Build();
//         //         return config;
//         // }