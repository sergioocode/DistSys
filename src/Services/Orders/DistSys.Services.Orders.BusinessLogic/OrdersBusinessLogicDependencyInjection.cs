using DistSys.Services.Orders.BusinessLogic.Data.External;
using DistSys.Services.Orders.BusinessLogic.Services.External;
using DistSys.Shared.Setup.Databases;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DistSys.Services.Orders.BusinessLogic;

public static class OrdersBusinessLogicDependencyInjection
{
    public static void AddProductService(
        this IServiceCollection serviceCollection,
        IConfiguration configuration
    )
    {
        serviceCollection.AddDistSysMongoDbConnectionProvider(configuration, "productStore");
        serviceCollection.AddScoped<IProductRepository, ProductRepository>();
        serviceCollection.AddScoped<IProductNameService, ProductNameService>();
        serviceCollection.AddHttpClient();
        //For now we do not need redis, as is only for local, in prod I recommend redis.
        serviceCollection.AddDistributedMemoryCache();
    }
}
