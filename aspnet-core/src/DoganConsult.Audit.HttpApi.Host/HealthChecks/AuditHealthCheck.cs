using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using DoganConsult.Audit.AuditLogs;

namespace DoganConsult.Audit.HealthChecks;

public class AuditHealthCheck : IHealthCheck
{
    private readonly IRepository<AuditLogs.AuditLog, Guid> _auditLogRepository;

    public AuditHealthCheck(IRepository<AuditLogs.AuditLog, Guid> auditLogRepository)
    {
        _auditLogRepository = auditLogRepository;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await _auditLogRepository.GetCountAsync(cancellationToken);
            return HealthCheckResult.Healthy("Audit service is healthy");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Audit service is unhealthy", ex);
        }
    }
}
