using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using vega.Core;
using vega.Core.Models;
using vega.Core.Models.States;
using vega.Persistence;
using vegaplannerserver.test.TestHelpers;

namespace ChannelAllocator.Tests.IntegrationTesting.Helpers
{
    public class SetupDefaultTestData
    {
        private readonly VegaDbContext db;

        public SetupDefaultTestData(VegaDbContext db)
        {
            this.db = db;
        }

        public VegaDbContext CreateProjectWithOneGeneratorFiveStates()
        {
            var PG = GeneratorInitialisers.createProjectGenerator("Test Project Generator");
            var GN = GeneratorInitialisers.createGenerator("Test Gen 1", 3);

            var GS = GeneratorInitialisers.createGeneratorState(6);
            GN.States.Add(GS);
            GS = GeneratorInitialisers.createGeneratorState(4);
            GN.States.Add(GS);
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
            var GN = GeneratorInitialisers.createGenerator("Test Gen 1", 3);

            var GS = GeneratorInitialisers.createGeneratorState(5);
            GN.States.Add(GS);
            GS = GeneratorInitialisers.createGeneratorState(3);
            GN.States.Add(GS);
            GS = GeneratorInitialisers.createGeneratorState(4);
            GN.States.Add(GS);
            var PGNS = new ProjectGeneratorSequence();
            PGNS.SeqId=3;
            PGNS.Generator = GN;
            PG.Generators.Add(PGNS);

            db.Add(PG);

            var GN2 = GeneratorInitialisers.createGenerator("Test Gen 2", 3);
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

            var GN3 = GeneratorInitialisers.createGenerator("Test Gen 3", 3);
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
            var GN = GeneratorInitialisers.createGenerator("Test Gen 1", 3);

            var GS = GeneratorInitialisers.createGeneratorState(6);
            GN.States.Add(GS);
            GS = GeneratorInitialisers.createGeneratorState(4);
            GN.States.Add(GS);
            GS = GeneratorInitialisers.createGeneratorState(5);
            GN.States.Add(GS);
            var PGNS = new ProjectGeneratorSequence();
            PGNS.SeqId=3;
            PGNS.Generator = GN;
            PG.Generators.Add(PGNS);

            db.Add(PG);

            var GN2 = GeneratorInitialisers.createGenerator("Test Gen 2", 3);
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

            var GN3 = GeneratorInitialisers.createGenerator("Test Gen 3", 3);
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