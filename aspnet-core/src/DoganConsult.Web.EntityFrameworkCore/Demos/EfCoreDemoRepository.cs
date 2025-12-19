using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace DoganConsult.Web.Demos;

/// <summary>
/// Entity Framework Core implementation of IDemoRepository
/// </summary>
public class EfCoreDemoRepository : EfCoreRepository<EntityFrameworkCore.WebDbContext, DemoRequest, int>, IDemoRepository
{
    public EfCoreDemoRepository(IDbContextProvider<EntityFrameworkCore.WebDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    public async Task<List<DemoRequest>> GetListAsync(
        string? status = null,
        string? priority = null,
        string? assignedTo = null,
        int skipCount = 0,
        int maxResultCount = 10,
        CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        
        var query = dbSet.AsQueryable();

        // Apply filters
        if (!string.IsNullOrWhiteSpace(status))
        {
            query = query.Where(x => x.CurrentStatus == status);
        }

        if (!string.IsNullOrWhiteSpace(priority))
        {
            query = query.Where(x => x.Priority == priority);
        }

        if (!string.IsNullOrWhiteSpace(assignedTo))
        {
            query = query.Where(x => x.AssignedTo == assignedTo);
        }

        // Order by most recent first
        query = query.OrderByDescending(x => x.CreationTime);

        // Apply pagination
        return await query
            .Skip(skipCount)
            .Take(maxResultCount)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<DemoRequest>> GetRecentAsync(
        int count = 5,
        CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        
        return await dbSet
            .OrderByDescending(x => x.CreationTime)
            .Take(count)
            .ToListAsync(cancellationToken);
    }

    public async Task<Dictionary<string, int>> GetCountByStatusAsync(
        CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        
        var result = await dbSet
            .GroupBy(x => x.CurrentStatus)
            .Select(g => new { Status = g.Key, Count = g.Count() })
            .ToListAsync(cancellationToken);

        return result.ToDictionary(x => x.Status, x => x.Count);
    }
}
