using vega.Core.Models;
using vega.Core.Models.States;

namespace vegaplannerserver.test.TestHelpers
{
    public static class GeneratorInitialisers
    {
        public static ProjectGenerator createProjectGenerator(string name) {
            var  PG = new ProjectGenerator();
            PG.Name = name;
            return PG;
        }
        public static StateInitialiser createGenerator(string name, int nOfStates) {
            var  GN = new StateInitialiser();
            GN.Name = name;

            for(int i=0; i < nOfStates; i++)
                GN.States.Add(createGeneratorState(i));

            return GN;
        }
        public static StateInitialiserState createGeneratorState(int stateNo) {


            var  GNS = new StateInitialiserState();
            GNS.Name = "State:" + stateNo;
            GNS.OrderId = stateNo;
            return GNS;
        }
    }   
}