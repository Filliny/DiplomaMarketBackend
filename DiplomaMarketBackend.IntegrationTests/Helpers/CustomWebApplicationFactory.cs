using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace DiplomaMarketBackend.IntegrationTests.Helpers
{
    public class CustomWebApplicationFactory<TProgram>
        : WebApplicationFactory<TProgram> where TProgram : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            // Works for: Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") in Program.cs
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Testing");

            // OR

            // Works for: builder.Environment.EnvironmentName in Program.cs
            builder.UseEnvironment("Testing");
        }
    }
}
