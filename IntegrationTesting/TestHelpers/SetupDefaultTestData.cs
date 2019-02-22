using System;
using System.Collections.Generic;
using System.Linq;
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

        public VegaDbContext CreateProjectWithOneGenerator()
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

    }
}