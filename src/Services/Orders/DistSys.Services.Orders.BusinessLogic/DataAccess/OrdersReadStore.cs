using DistSys.Services.Orders.Dtos;
using DistSys.Shared.Databases.MongoDb;
using Microsoft.Extensions.Options;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace DistSys.Services.Orders.BusinessLogic.DataAccess;

public interface IOrdersReadStore
{
    Task<OrderResponse?> GetOrder(
        Guid orderId,
        CancellationToken cancellationToken = default
    );

    Task UpsertOrder(
        OrderResponse order,
        int version,
        CancellationToken cancellationToken = default
    );

    Task UpdateProductName(
        int productId,
        string name,
        CancellationToken cancellationToken = default
    );
}

public class OrdersReadStore : IOrdersReadStore
{
    private const string CollectionName = "Orders";
    private readonly IMongoDatabase _database;

    public OrdersReadStore(
        IMongoDbConnectionProvider connectionProvider,
        IOptions<DatabaseConfiguration> databaseConfiguration
    )
    {
        MongoClient client = new(connectionProvider.GetMongoUrl());
        _database = client.GetDatabase(databaseConfiguration.Value.DatabaseName);
    }

    public async Task<OrderResponse?> GetOrder(
        Guid orderId,
        CancellationToken cancellationToken = default
    )
    {
        IMongoCollection<OrderReadEntity> collection =
            _database.GetCollection<OrderReadEntity>(CollectionName);

        OrderReadEntity? entity = await collection
            .Find(x => x.OrderId == orderId)
            .SingleOrDefaultAsync(cancellationToken);

        return entity?.ToResponse();
    }

    public async Task UpsertOrder(
        OrderResponse order,
        int version,
        CancellationToken cancellationToken = default
    )
    {
        IMongoCollection<OrderReadEntity> collection =
            _database.GetCollection<OrderReadEntity>(CollectionName);

        OrderReadEntity? current = await collection
            .Find(x => x.OrderId == order.OrderId)
            .SingleOrDefaultAsync(cancellationToken);

        if (current is not null && current.Version > version)
            return;

        OrderReadEntity entity = OrderReadEntity.From(order, version);
        await collection.ReplaceOneAsync(
            x => x.OrderId == order.OrderId,
            entity,
            new ReplaceOptions { IsUpsert = true },
            cancellationToken
        );
    }

    public async Task UpdateProductName(
        int productId,
        string name,
        CancellationToken cancellationToken = default
    )
    {
        IMongoCollection<OrderReadEntity> collection =
            _database.GetCollection<OrderReadEntity>(CollectionName);

        List<OrderReadEntity> orders = await collection
            .Find(x => x.Products.Any(product => product.ProductId == productId))
            .ToListAsync(cancellationToken);

        foreach (OrderReadEntity order in orders)
        {
            foreach (
                ProductReadEntity product in order.Products.Where(
                    product => product.ProductId == productId
                )
            )
                product.Name = name;

            await collection.ReplaceOneAsync(
                x => x.OrderId == order.OrderId,
                order,
                cancellationToken: cancellationToken
            );
        }
    }

    private sealed class OrderReadEntity
    {
        [BsonId]
        public Guid OrderId { get; set; }
        public string OrderStatus { get; set; } = null!;
        public DeliveryDetails DeliveryDetails { get; set; } = null!;
        public PaymentInformation PaymentInformation { get; set; } = null!;
        public List<ProductReadEntity> Products { get; set; } = [];
        public int Version { get; set; }

        public static OrderReadEntity From(OrderResponse order, int version) =>
            new()
            {
                OrderId = order.OrderId,
                OrderStatus = order.OrderStatus,
                DeliveryDetails = order.DeliveryDetails,
                PaymentInformation = order.PaymentInformation,
                Products = order.Products
                    .Select(product => new ProductReadEntity
                    {
                        ProductId = product.ProductId,
                        Quantity = product.Quantity,
                        Name = product.Name,
                    })
                    .ToList(),
                Version = version,
            };

        public OrderResponse ToResponse() =>
            new(
                OrderId,
                OrderStatus,
                DeliveryDetails,
                PaymentInformation,
                Products
                    .Select(product => new ProductQuantityName(
                        product.ProductId,
                        product.Quantity,
                        product.Name
                    ))
                    .ToList()
            );
    }

    private sealed class ProductReadEntity
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public string Name { get; set; } = null!;
    }
}
