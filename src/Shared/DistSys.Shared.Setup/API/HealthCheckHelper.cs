using Microsoft.Extensions.Configuration;

namespace DistSys.Shared.Setup.API;

public static class HealthCheckHelper
{
    public static IConfiguration BuildBasicHealthCheck()
    {
        var myConfiguration = new Dictionary<string, string?>
        {
            { "HealthChecksUI:HealthChecks:0:Name", "self" },
            { "HealthChecksUI:HealthChecks:0:Uri", "/health" },
        };

        return new ConfigurationBuilder().AddInMemoryCollection(myConfiguration).Build();
    }
}
