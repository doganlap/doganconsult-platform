using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace DoganConsult.Web.Demos;

/// <summary>
/// Repository interface for DemoRequest entity
/// </summary>
public interface IDemoRepository : IRepository<DemoRequest, int>
{
    /// <summary>
    /// Get demo requests with optional filtering
    /// </summary>
    Task<List<DemoRequest>> GetListAsync(
        string? status = null,
        string? priority = null,
        string? assignedTo = null,
        int skipCount = 0,
        int maxResultCount = 10,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get recent demo requests
    /// </summary>
    Task<List<DemoRequest>> GetRecentAsync(
        int count = 5,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get demo request count by status
    /// </summary>
    Task<Dictionary<string, int>> GetCountByStatusAsync(
        CancellationToken cancellationToken = default);
}
