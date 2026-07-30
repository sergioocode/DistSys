using DistSys.Shared.Discovery;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace DistSys.Services.Orders.BusinessLogic.HealthChecks;

public class ProductsHealthCheck : IHealthCheck
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IServiceDiscovery _discovery;

    public ProductsHealthCheck(IHttpClientFactory httpClientFactory, IServiceDiscovery discovery)
    {
        _httpClientFactory = httpClientFactory;
        _discovery = discovery;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = new CancellationToken()
    )
    {
        // TODO: abstraer las llamadas HTTP a otros microservicios de DistSys.
        HttpClient client = _httpClientFactory.CreateClient();

        string productsReadApi = await _discovery.GetFullAddress(
            DiscoveryServices.Microservices.ProductsApi.ApiRead,
            cancellationToken
        );

        client.BaseAddress = new Uri($"https://{productsReadApi}");

        HttpResponseMessage responseMessage = await client.GetAsync("health", cancellationToken);

        return responseMessage.IsSuccessStatusCode
            ? HealthCheckResult.Healthy("Product service is healthy")
            : HealthCheckResult.Degraded("Product service is down");
    }
}
