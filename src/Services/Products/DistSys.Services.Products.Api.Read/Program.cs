using DistSys.Services.Products.BusinessLogic.DataAccess;
using DistSys.Shared.Setup.API;
using DistSys.Shared.Setup.Databases;

WebApplication app = DefaultDistSysWebApplication.Create(
    args,
    builder =>
    {
        builder
            .Services.AddDistSysMongoDbConnectionProvider(builder.Configuration)
            .AddScoped<IProductsReadStore, ProductsReadStore>();
    }
);

app.MapGet(
    "product/{productId}",
    async (int productId, IProductsReadStore readStore) => await readStore.GetFullProduct(productId)
); //TODO: result struct gives an error on minimal api?

DefaultDistSysWebApplication.Run(app);
