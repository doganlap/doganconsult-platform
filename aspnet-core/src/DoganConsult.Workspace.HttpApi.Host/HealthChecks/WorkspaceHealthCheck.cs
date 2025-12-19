using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using DoganConsult.Workspace.Workspaces;

namespace DoganConsult.Workspace.HealthChecks;

public class WorkspaceHealthCheck : IHealthCheck
{
    private readonly IRepository<Workspaces.Workspace, Guid> _workspaceRepository;

    public WorkspaceHealthCheck(IRepository<Workspaces.Workspace, Guid> workspaceRepository)
    {
        _workspaceRepository = workspaceRepository;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await _workspaceRepository.GetCountAsync(cancellationToken);
            return HealthCheckResult.Healthy("Workspace service is healthy");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Workspace service is unhealthy", ex);
        }
    }
}
