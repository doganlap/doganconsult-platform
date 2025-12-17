using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace DoganConsult.Audit.Approvals;

public interface IApprovalHistoryRepository : IRepository<ApprovalHistory, Guid>
{
    Task<List<ApprovalHistory>> GetListByApprovalRequestAsync(
        Guid approvalRequestId,
        CancellationToken cancellationToken = default);

    Task<List<ApprovalHistory>> GetListByActorAsync(
        Guid actorId,
        int skipCount = 0,
        int maxResultCount = 50,
        CancellationToken cancellationToken = default);
}
