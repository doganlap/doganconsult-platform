using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using DoganConsult.AI.AIRequests;

namespace DoganConsult.AI.HealthChecks;

public class AIHealthCheck : IHealthCheck
{
    private readonly IRepository<AIRequests.AIRequest, Guid> _aiRequestRepository;

    public AIHealthCheck(IRepository<AIRequests.AIRequest, Guid> aiRequestRepository)
    {
        _aiRequestRepository = aiRequestRepository;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await _aiRequestRepository.GetCountAsync(cancellationToken);
            return HealthCheckResult.Healthy("AI service is healthy");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("AI service is unhealthy", ex);
        }
    }
}
