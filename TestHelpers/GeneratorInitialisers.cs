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
        public static StateInitialiser createGenerator(string name, int nOfStates, int noOfDays, int alertDays) {
            var  GN = new StateInitialiser();
            GN.Name = name;

            for(int i=1; i <= nOfStates; i++)
                GN.States.Add(createGeneratorState(name, i, noOfDays, alertDays));

            return GN;
        }
        public static StateInitialiserState createGeneratorState(string name, int stateNo,int noOfDays, int alertDays) {


            var  GNS = new StateInitialiserState();
            GNS.Name = name + ":State" + stateNo;
            GNS.OrderId = stateNo;
            GNS.CompletionTime = noOfDays;
            GNS.AlertToCompletionTime = alertDays;
            return GNS;
        }
    }   
}