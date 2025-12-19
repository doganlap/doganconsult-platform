using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using DoganConsult.Organization.Organizations;

namespace DoganConsult.Organization.HealthChecks;

public class OrganizationHealthCheck : IHealthCheck
{
    private readonly IRepository<Organizations.Organization, Guid> _organizationRepository;

    public OrganizationHealthCheck(IRepository<Organizations.Organization, Guid> organizationRepository)
    {
        _organizationRepository = organizationRepository;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Simple check: try to query the database
            await _organizationRepository.GetCountAsync(cancellationToken);
            return HealthCheckResult.Healthy("Organization service is healthy");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Organization service is unhealthy", ex);
        }
    }
}
