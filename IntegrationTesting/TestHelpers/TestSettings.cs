using System;
using vega.Services.Interfaces;

namespace vegaplannerserver.test.IntegrationTesting.TestHelpers
{
    public static class TestSettings
    {
        public const string ProjectGeneratorName = "Test Project Generator";
        public const string GeneratorPrefixName = "TestGen";
 
        public const int OneState = 1;
        public const int FiveStates = 5;
        public const int TenStates = 10;
        public const int FifteenStates = 15;
        public const int TwentyStates = 20;
        public const int FiftyStates = 50;
    }

    public class  DateServiceTest : IDateService
    {
        DateTime testDate = DateTime.Parse("01/01/2019");
        public DateTime GetCurrentDate() {
            return testDate;  //Test Date
        }

        public void SetCurrentDate(DateTime testDate) {
            this.testDate = testDate;  //Test Date
        }
    }
}