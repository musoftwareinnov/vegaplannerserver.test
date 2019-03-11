using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using vega.Core;
using vega.Core.Models;
using vega.Core.Models.States;
using vega.Persistence;
using vegaplannerserver.test.IntegrationTesting.TestHelpers;
using vegaplannerserver.test.TestHelpers;

namespace ChannelAllocator.Tests.IntegrationTesting.Helpers
{
    public class SetupDefaultTestData
    {
        private readonly VegaDbContext db;
        private readonly int NOOFDAYSDEF = 2;
        private readonly int ALERTNOOFDAYSDEF = 1;
        public SetupDefaultTestData(VegaDbContext db)
        {
            this.db = db;
        }

        public VegaDbContext CreateCustomer(string userName)
        {
            var testCustomer = new Customer() { 
                CustomerAddress = new Address {
                    AddressLine1 = "Test Address Line 1", 
                    Postcode = "TST A01",
                    City = "TST CITY 1" ,
                    County = "TST County 1" },

                CustomerContact = new Contact {
                    FirstName = "TST First Name",
                    LastName = userName,
                    TelephoneMobile = "9999999999999"
                }               
            };

            db.Customers.Add(testCustomer);
            db.SaveChanges();
            return db; 
        }

        public VegaDbContext CreateProjectGeneratorsStates(int noOfGenerators, int noOfStates)
        {

            var PG = GeneratorInitialisers.createProjectGenerator(TestSettings.ProjectGeneratorName);

            for(int gnCtr=1; gnCtr<=noOfGenerators; gnCtr++){
                var GN = GeneratorInitialisers.createGenerator(TestSettings.GeneratorPrefixName + gnCtr, noOfStates, NOOFDAYSDEF, ALERTNOOFDAYSDEF);
                var PGNS = new ProjectGeneratorSequence();
                PGNS.SeqId=gnCtr;
                PGNS.Generator = GN;
                PG.Generators.Add(PGNS);
                db.Add(PG);
                             
            }
            db.SaveChanges();  
            return db;
        }
        public VegaDbContext CreateProjectWithOneGeneratorFiveStates()
        {
            var PG = GeneratorInitialisers.createProjectGenerator("Test Project Generator");
            var GN = GeneratorInitialisers.createGenerator("Test Gen 1", 5, NOOFDAYSDEF, ALERTNOOFDAYSDEF);

            // var GS = GeneratorInitialisers.createGeneratorState(5, NOOFDAYSDEF, ALERTNOOFDAYSDEF);
            // GN.States.Add(GS);
            // GS = GeneratorInitialisers.createGeneratorState(3, NOOFDAYSDEF, ALERTNOOFDAYSDEF);
            // GN.States.Add(GS);
            var PGNS = new ProjectGeneratorSequence();
            PGNS.SeqId=1;
            PGNS.Generator = GN;
            PG.Generators.Add(PGNS);
            db.Add(PG);

            db.SaveChanges();

            return db;
        }
        public VegaDbContext CreateProjectWithOneGeneratorsSorted()
        {
            var PG = GeneratorInitialisers.createProjectGenerator("Test Project Generator");
            var GN = GeneratorInitialisers.createGenerator("Test Gen 1", 3, NOOFDAYSDEF, ALERTNOOFDAYSDEF);

            var GS = GeneratorInitialisers.createGeneratorState(GN.Name, 5, NOOFDAYSDEF, ALERTNOOFDAYSDEF);
            GN.States.Add(GS);
            GS = GeneratorInitialisers.createGeneratorState(GN.Name, 3,NOOFDAYSDEF, ALERTNOOFDAYSDEF);
            GN.States.Add(GS);
            GS = GeneratorInitialisers.createGeneratorState(GN.Name, 4, NOOFDAYSDEF, ALERTNOOFDAYSDEF);
            GN.States.Add(GS);
            var PGNS = new ProjectGeneratorSequence();
            PGNS.SeqId=3;
            PGNS.Generator = GN;
            PG.Generators.Add(PGNS);

            db.Add(PG);

            var GN2 = GeneratorInitialisers.createGenerator("Test Gen 2", 3, NOOFDAYSDEF, ALERTNOOFDAYSDEF);
            var PGNS2 = new ProjectGeneratorSequence();
            PGNS2.SeqId=2;
            PGNS2.Generator = GN2;
            PG.Generators.Add(PGNS2);

            var PGSLIST = new List<ProjectGeneratorSequence>();
            PGSLIST.Add(PGNS);
            PGSLIST.Add(PGNS2);
            PG.Generators = PGSLIST;
            db.Add(PG);


            db.SaveChanges();
            var projectGenerator = db.ProjectGenerators.Include(pg => pg.Generators).ToList().SingleOrDefault();

            var GN3 = GeneratorInitialisers.createGenerator("Test Gen 3", 3, NOOFDAYSDEF, ALERTNOOFDAYSDEF);
            var PGNS3 = new ProjectGeneratorSequence();
            PGNS3.SeqId=1;
            PGNS3.Generator = GN3;
            //PG.Generators.Add(PGNS2);

            projectGenerator.Generators.Add(PGNS3);
            db.SaveChanges();
            return db;
        }
        public VegaDbContext CreateProjectWithThreeGeneratorsSorted()
        {
            var PG = GeneratorInitialisers.createProjectGenerator("Test Project Generator");
            var GN = GeneratorInitialisers.createGenerator("Test Gen 1", 3, NOOFDAYSDEF, ALERTNOOFDAYSDEF);

            var GS = GeneratorInitialisers.createGeneratorState(GN.Name, 6, NOOFDAYSDEF, ALERTNOOFDAYSDEF);
            GN.States.Add(GS);
            GS = GeneratorInitialisers.createGeneratorState(GN.Name, 4, NOOFDAYSDEF, ALERTNOOFDAYSDEF);
            GN.States.Add(GS);
            GS = GeneratorInitialisers.createGeneratorState(GN.Name, 5, NOOFDAYSDEF, ALERTNOOFDAYSDEF);
            GN.States.Add(GS);
            var PGNS = new ProjectGeneratorSequence();
            PGNS.SeqId=3;
            PGNS.Generator = GN;
            PG.Generators.Add(PGNS);

            db.Add(PG);

            var GN2 = GeneratorInitialisers.createGenerator("Test Gen 2", 3, NOOFDAYSDEF, ALERTNOOFDAYSDEF);
            var PGNS2 = new ProjectGeneratorSequence();
            PGNS2.SeqId=2;
            PGNS2.Generator = GN2;
            PG.Generators.Add(PGNS2);

            var PGSLIST = new List<ProjectGeneratorSequence>();
            PGSLIST.Add(PGNS);
            PGSLIST.Add(PGNS2);
            PG.Generators = PGSLIST;
            db.Add(PG);


            db.SaveChanges();
            var projectGenerator = db.ProjectGenerators.Include(pg => pg.Generators).ToList().SingleOrDefault();

            var GN3 = GeneratorInitialisers.createGenerator("Test Gen 3", 3, NOOFDAYSDEF, ALERTNOOFDAYSDEF);
            var PGNS3 = new ProjectGeneratorSequence();
            PGNS3.SeqId=1;
            PGNS3.Generator = GN3;
            //PG.Generators.Add(PGNS2);

            projectGenerator.Generators.Add(PGNS3);
            db.SaveChanges();
            return db;
        }



    }
}