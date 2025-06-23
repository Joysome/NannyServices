using Microsoft.Extensions.Diagnostics.HealthChecks;
using Nanny.Admin.Infrastructure.Db;

namespace Nanny.Admin.Api.HealthChecks;

public class DatabaseHealthCheck(AppDbContext context) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context1,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var canConnect = await context.Database.CanConnectAsync(cancellationToken);

            return !canConnect
                ? HealthCheckResult.Unhealthy("Database connection failed")
                : HealthCheckResult.Healthy("Database connection is healthy");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Health check failed", ex);
        }
    }
}
