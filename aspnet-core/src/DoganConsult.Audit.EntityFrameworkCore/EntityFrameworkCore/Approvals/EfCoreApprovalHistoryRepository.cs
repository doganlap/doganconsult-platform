using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DoganConsult.Audit.Approvals;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace DoganConsult.Audit.EntityFrameworkCore.Approvals;

public class EfCoreApprovalHistoryRepository 
    : EfCoreRepository<AuditDbContext, ApprovalHistory, Guid>, IApprovalHistoryRepository
{
    public EfCoreApprovalHistoryRepository(IDbContextProvider<AuditDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    public async Task<List<ApprovalHistory>> GetListByApprovalRequestAsync(
        Guid approvalRequestId,
        CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet
            .Where(x => x.ApprovalRequestId == approvalRequestId)
            .OrderBy(x => x.CreationTime)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<ApprovalHistory>> GetListByActorAsync(
        Guid actorId,
        int skipCount = 0,
        int maxResultCount = 50,
        CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet
            .Where(x => x.ActorId == actorId)
            .OrderByDescending(x => x.CreationTime)
            .Skip(skipCount)
            .Take(maxResultCount)
            .ToListAsync(cancellationToken);
    }
}
