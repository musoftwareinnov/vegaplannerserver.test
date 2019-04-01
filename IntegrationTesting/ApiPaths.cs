namespace vega.Tests.IntegrationTesting.Helpers
{
    public static class ApiPaths
    {
        public const string AuthLoginApi = "api/auth/login";
        public const string ProjectGenerators = "api/projectgenerators";
        public const string Generators = "/api/stateinitialisers";
        public const string PlanningApps = "api/planningapps";
        public const string BusinessDates = "api/businessdates";
        public const string AddGenerator = "api/planningapps/insertgenerator";
        public const string RemoveGenerator = "api/planningapps/removegenerator";
        public const string NextState = "api/planningapps/nextstate";
        public const string ResultContentType = "application/json; charset=utf-8";

        public const string AdminUser = "adminuser";
        public const string AdminPassword= "adminpwd";
    }
}