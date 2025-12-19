using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Identity;
using Volo.Abp.Domain.Repositories;

namespace DoganConsult.Identity.HealthChecks;

public class IdentityHealthCheck : IHealthCheck
{
    private readonly IRepository<IdentityUser, Guid> _userRepository;

    public IdentityHealthCheck(IRepository<IdentityUser, Guid> userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await _userRepository.GetCountAsync(cancellationToken);
            return HealthCheckResult.Healthy("Identity service is healthy");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Identity service is unhealthy", ex);
        }
    }
}
