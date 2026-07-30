using DistSys.Services.Products.BusinessLogic.DataAccess;
using DistSys.Services.Products.BusinessLogic.UseCases;
using DistSys.Shared.Setup.API;
using DistSys.Shared.Setup.Databases;
using DistSys.Shared.Setup.Services;

WebApplication app = DefaultDistSysWebApplication.Create(
    args,
    builder =>
    {
        builder
            .Services.AddMySql<ProductsWriteStore>("distribt")
            .AddScoped<IProductsWriteStore, ProductsWriteStore>()
            .AddScoped<IUpdateProductDetails, UpdateProductDetails>()
            .AddScoped<ICreateProductDetails, CreateProductDetails>()
            .AddScoped<IStockApi, ProductsDependencyFakeType>() //testing purposes
            .AddScoped<IWarehouseApi, ProductsDependencyFakeType>() //testing purposes
            .AddServiceBusDomainPublisher(builder.Configuration);
    }
);

DefaultDistSysWebApplication.Run(app);
