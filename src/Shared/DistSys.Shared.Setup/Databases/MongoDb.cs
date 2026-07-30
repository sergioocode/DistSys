using DistSys.Shared.Databases.MongoDb;
using Microsoft.Extensions.Configuration;

namespace DistSys.Shared.Setup.Databases;

public static class MongoDb
{
    public static IServiceCollection AddDistSysMongoDbConnectionProvider(
        this IServiceCollection serviceCollection,
        IConfiguration configuration,
        string name = "mongodb"
    )
    {
        return serviceCollection
            .AddMongoDbConnectionProvider()
            .AddMongoDbDatabaseConfiguration(configuration)
            .AddMongoHealthCheck(name);
    }
}
