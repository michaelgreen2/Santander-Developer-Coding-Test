using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Santander.API.HealthCheck
{
    public class HealthChecker : IHealthCheck
    {
        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                return Task.FromResult(HealthCheckResult.Healthy("Santander API is running successfully."));
            }
            catch (Exception)
            {
                return Task.FromResult(new HealthCheckResult(context.Registration.FailureStatus, "The Santander API is down."));
            }
        }
    }
}
