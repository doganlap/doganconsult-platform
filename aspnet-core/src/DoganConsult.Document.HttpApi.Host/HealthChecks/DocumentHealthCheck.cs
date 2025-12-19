using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using DoganConsult.Document.Documents;

namespace DoganConsult.Document.HealthChecks;

public class DocumentHealthCheck : IHealthCheck
{
    private readonly IRepository<Documents.Document, Guid> _documentRepository;

    public DocumentHealthCheck(IRepository<Documents.Document, Guid> documentRepository)
    {
        _documentRepository = documentRepository;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await _documentRepository.GetCountAsync(cancellationToken);
            return HealthCheckResult.Healthy("Document service is healthy");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Document service is unhealthy", ex);
        }
    }
}
