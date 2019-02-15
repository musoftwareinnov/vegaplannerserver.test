
using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace vegaplannerserver.test.TestHelpers
{
    public class DependencyResolverHelper
    {
        private readonly IWebHost _webHost;

        /// <inheritdoc />
        public DependencyResolverHelper(IWebHost WebHost) => _webHost = WebHost;

        public T GetService<T>()
        {
            using (var serviceScope = _webHost.Services.CreateScope())
            {
                var services = serviceScope.ServiceProvider;
                try
                {
                    var scopedService = services.GetRequiredService<T>();
                    return scopedService;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            };
        }
    }
}