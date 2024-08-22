using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using minimal_api;
using minimal_api.domain.interfaces;
using Test.Mocks;

namespace Test.Helpers;

public class Setup
{
    public const string Port = "5001";
    public static TestContext TestContext { get; set; } = default!;
    public static WebApplicationFactory<Startup> Http { get; set; } = default!;
    public static HttpClient Client = default!;
    
    public static void ClassInit(TestContext testContext)
    {
        Setup.TestContext = testContext;
        Setup.Http = new WebApplicationFactory<Startup>();

        Setup.Http = Setup.Http.WithWebHostBuilder(builder =>
        {
            builder.UseSetting("https_port", Setup.Port).UseEnvironment("Testing");

            builder.ConfigureServices(services =>
            {
                services.AddScoped<IAdministradorService, AdministradorServiceMock>();
            });
        });
        Setup.Client = Setup.Http.CreateClient();
    }

    public static void ClassCleanup()
    {
        Setup.Http.Dispose();
    }
}