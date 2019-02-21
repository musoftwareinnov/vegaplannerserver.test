
using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using vega.Persistence;

namespace vega.test.IntegrationTesting.Helpers
{
    public abstract class AbstractConfigureWebHost {

        public AbstractConfigureWebHost(IWebHostBuilder builder) {
            var b = builder.ConfigureServices(services =>
            {
                // Create a new service provider.
                var serviceChannelAllocatorProvider = new ServiceCollection()
                    .AddEntityFrameworkInMemoryDatabase()
                    .BuildServiceProvider();

                // Add a database context (ApplicationDbContext) using an in-memory 
                // database for testing.
                services.AddDbContext<VegaDbContext>(options => 
                {
                    options.UseInMemoryDatabase("InMemoryChannelAllocatorDbForTesting");
                    options.UseInternalServiceProvider(serviceChannelAllocatorProvider);
                });

                // Build the service provider.
                var sp = services.BuildServiceProvider();

                // Create a scope to obtain a reference to the database
                // context (ApplicationDbContext).
                using (var scope = sp.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;
                    var vegaDbContext = scopedServices.GetRequiredService<VegaDbContext>();

                    // Ensure the database is created.
                    vegaDbContext.Database.EnsureCreated();
                    InitializeVegaPlannerServerDbForTests(vegaDbContext); 
                }
            });
        }
        public abstract void InitializeVegaPlannerServerDbForTests(VegaDbContext db);

    }
}