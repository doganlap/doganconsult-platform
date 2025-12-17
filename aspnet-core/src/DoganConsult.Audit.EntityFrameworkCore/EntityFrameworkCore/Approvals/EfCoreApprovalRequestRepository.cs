using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DoganConsult.Audit.Approvals;
using DoganConsult.Audit.Workflow;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace DoganConsult.Audit.EntityFrameworkCore.Approvals;

public class EfCoreApprovalRequestRepository 
    : EfCoreRepository<AuditDbContext, ApprovalRequest, Guid>, IApprovalRequestRepository
{
    public EfCoreApprovalRequestRepository(IDbContextProvider<AuditDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    public async Task<ApprovalRequest?> FindByRequestNumberAsync(
        string requestNumber,
        CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet
            .FirstOrDefaultAsync(x => x.RequestNumber == requestNumber, cancellationToken);
    }

    public async Task<List<ApprovalRequest>> GetListByEntityAsync(
        ApprovalEntityType entityType,
        Guid entityId,
        CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet
            .Where(x => x.EntityType == entityType && x.EntityId == entityId)
            .OrderByDescending(x => x.CreationTime)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<ApprovalRequest>> GetPendingRequestsAsync(
        Guid? assignedApproverId = null,
        ApprovalEntityType? entityType = null,
        int skipCount = 0,
        int maxResultCount = 10,
        CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        var query = dbSet.Where(x => x.Status == ApprovalStatus.Pending);

        if (assignedApproverId.HasValue)
        {
            query = query.Where(x => x.AssignedApproverId == null || x.AssignedApproverId == assignedApproverId);
        }

        if (entityType.HasValue)
        {
            query = query.Where(x => x.EntityType == entityType);
        }

        return await query
            .OrderByDescending(x => x.Priority)
            .ThenBy(x => x.CreationTime)
            .Skip(skipCount)
            .Take(maxResultCount)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<ApprovalRequest>> GetListByRequesterAsync(
        Guid requesterId,
        ApprovalStatus? status = null,
        int skipCount = 0,
        int maxResultCount = 10,
        CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        var query = dbSet.Where(x => x.RequesterId == requesterId);

        if (status.HasValue)
        {
            query = query.Where(x => x.Status == status);
        }

        return await query
            .OrderByDescending(x => x.CreationTime)
            .Skip(skipCount)
            .Take(maxResultCount)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<ApprovalRequest>> GetListByApproverAsync(
        Guid approverId,
        ApprovalStatus? status = null,
        int skipCount = 0,
        int maxResultCount = 10,
        CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        var query = dbSet.Where(x => x.ApprovedById == approverId);

        if (status.HasValue)
        {
            query = query.Where(x => x.Status == status);
        }

        return await query
            .OrderByDescending(x => x.ApprovedAt)
            .Skip(skipCount)
            .Take(maxResultCount)
            .ToListAsync(cancellationToken);
    }

    public async Task<long> GetPendingCountAsync(
        Guid? assignedApproverId = null,
        ApprovalEntityType? entityType = null,
        CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        var query = dbSet.Where(x => x.Status == ApprovalStatus.Pending);

        if (assignedApproverId.HasValue)
        {
            query = query.Where(x => x.AssignedApproverId == null || x.AssignedApproverId == assignedApproverId);
        }

        if (entityType.HasValue)
        {
            query = query.Where(x => x.EntityType == entityType);
        }

        return await query.LongCountAsync(cancellationToken);
    }

    public async Task<List<ApprovalRequest>> GetExpiredRequestsAsync(
        CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        var now = DateTime.UtcNow;

        return await dbSet
            .Where(x => x.Status == ApprovalStatus.Pending && x.ExpiresAt.HasValue && x.ExpiresAt < now)
            .ToListAsync(cancellationToken);
    }
}
