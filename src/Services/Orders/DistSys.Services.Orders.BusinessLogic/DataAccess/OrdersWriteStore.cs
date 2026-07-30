using DistSys.Services.Orders.Dtos;
using Microsoft.EntityFrameworkCore;

namespace DistSys.Services.Orders.BusinessLogic.DataAccess;

public interface IOrdersWriteStore
{
    Task<OrderProjectionChanged> CreateOrder(
        CreateOrderRequest request,
        CancellationToken cancellationToken = default
    );

    Task<OrderProjectionChanged?> ChangeStatus(
        Guid orderId,
        OrderStatus newStatus,
        CancellationToken cancellationToken = default
    );
}

public class OrdersWriteStore : DbContext, IOrdersWriteStore
{
    private DbSet<OrderEntity> Orders => Set<OrderEntity>();
    private DbSet<OrderProductEntity> OrderProducts => Set<OrderProductEntity>();
    private DbSet<OrderStatusHistoryEntity> OrderStatusHistory =>
        Set<OrderStatusHistoryEntity>();

    public OrdersWriteStore(DbContextOptions<OrdersWriteStore> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<OrderEntity>(entity =>
        {
            entity.ToTable("Orders");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).HasMaxLength(36);
            entity.Property(x => x.Status).HasMaxLength(30);
            entity.Property(x => x.Street).HasMaxLength(250);
            entity.Property(x => x.City).HasMaxLength(100);
            entity.Property(x => x.Country).HasMaxLength(100);
            entity.Property(x => x.CardNumber).HasMaxLength(30);
            entity.Property(x => x.ExpireDate).HasMaxLength(10);
            entity.Property(x => x.Security).HasMaxLength(10);
            entity.Property(x => x.Version).IsConcurrencyToken();
        });

        modelBuilder.Entity<OrderProductEntity>(entity =>
        {
            entity.ToTable("OrderProducts");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.OrderId).HasMaxLength(36);
            entity.HasIndex(x => x.OrderId);
            entity
                .HasOne<OrderEntity>()
                .WithMany()
                .HasForeignKey(x => x.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<OrderStatusHistoryEntity>(entity =>
        {
            entity.ToTable("OrderStatusHistory");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.OrderId).HasMaxLength(36);
            entity.Property(x => x.Status).HasMaxLength(30);
            entity.HasIndex(x => x.OrderId);
            entity
                .HasOne<OrderEntity>()
                .WithMany()
                .HasForeignKey(x => x.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }

    public async Task<OrderProjectionChanged> CreateOrder(
        CreateOrderRequest request,
        CancellationToken cancellationToken = default
    )
    {
        Guid orderId = Guid.NewGuid();
        string id = orderId.ToString();

        OrderEntity order = new()
        {
            Id = id,
            Status = OrderStatus.Created.ToString(),
            Street = request.DeliveryDetails.Street,
            City = request.DeliveryDetails.City,
            Country = request.DeliveryDetails.Country,
            CardNumber = request.PaymentInformation.CardNumber,
            ExpireDate = request.PaymentInformation.ExpireDate,
            Security = request.PaymentInformation.Security,
            Version = 1,
        };

        await Orders.AddAsync(order, cancellationToken);
        await OrderProducts.AddRangeAsync(
            request.Products.Select(product => new OrderProductEntity
            {
                OrderId = id,
                ProductId = product.ProductId,
                Quantity = product.Quantity,
            }),
            cancellationToken
        );
        await OrderStatusHistory.AddAsync(
            NewStatusHistory(id, OrderStatus.Created, order.Version),
            cancellationToken
        );

        await SaveChangesAsync(cancellationToken);

        return ToProjection(orderId, order, request.Products);
    }

    public async Task<OrderProjectionChanged?> ChangeStatus(
        Guid orderId,
        OrderStatus newStatus,
        CancellationToken cancellationToken = default
    )
    {
        string id = orderId.ToString();
        OrderEntity? order = await Orders.SingleOrDefaultAsync(
            x => x.Id == id,
            cancellationToken
        );

        if (order is null)
            return null;

        OrderStatus currentStatus = Enum.Parse<OrderStatus>(order.Status);
        EnsureValidTransition(currentStatus, newStatus);

        order.Status = newStatus.ToString();
        order.Version++;

        await OrderStatusHistory.AddAsync(
            NewStatusHistory(id, newStatus, order.Version),
            cancellationToken
        );
        await SaveChangesAsync(cancellationToken);

        List<ProductQuantity> products = await OrderProducts
            .Where(x => x.OrderId == id)
            .Select(x => new ProductQuantity(x.ProductId, x.Quantity))
            .ToListAsync(cancellationToken);

        return ToProjection(orderId, order, products);
    }

    private static void EnsureValidTransition(OrderStatus currentStatus, OrderStatus newStatus)
    {
        bool isValid =
            (currentStatus == OrderStatus.Created && newStatus == OrderStatus.Paid)
            || (currentStatus == OrderStatus.Paid && newStatus == OrderStatus.Dispatched)
            || (currentStatus == OrderStatus.Dispatched && newStatus == OrderStatus.Completed)
            || newStatus == OrderStatus.Failed;

        if (!isValid)
            throw new InvalidOperationException(
                $"No se puede cambiar una orden de {currentStatus} a {newStatus}."
            );
    }

    private static OrderStatusHistoryEntity NewStatusHistory(
        string orderId,
        OrderStatus status,
        int version
    ) =>
        new()
        {
            OrderId = orderId,
            Status = status.ToString(),
            Version = version,
            OccurredAtUtc = DateTime.UtcNow,
        };

    private static OrderProjectionChanged ToProjection(
        Guid orderId,
        OrderEntity order,
        List<ProductQuantity> products
    ) =>
        new(
            orderId,
            Enum.Parse<OrderStatus>(order.Status),
            new DeliveryDetails(order.Street, order.City, order.Country),
            new PaymentInformation(order.CardNumber, order.ExpireDate, order.Security),
            products,
            order.Version
        );

    private sealed class OrderEntity
    {
        public string Id { get; set; } = null!;
        public string Status { get; set; } = null!;
        public string Street { get; set; } = null!;
        public string City { get; set; } = null!;
        public string Country { get; set; } = null!;
        public string CardNumber { get; set; } = null!;
        public string ExpireDate { get; set; } = null!;
        public string Security { get; set; } = null!;
        public int Version { get; set; }
    }

    private sealed class OrderProductEntity
    {
        public long Id { get; set; }
        public string OrderId { get; set; } = null!;
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }

    private sealed class OrderStatusHistoryEntity
    {
        public long Id { get; set; }
        public string OrderId { get; set; } = null!;
        public string Status { get; set; } = null!;
        public int Version { get; set; }
        public DateTime OccurredAtUtc { get; set; }
    }
}
