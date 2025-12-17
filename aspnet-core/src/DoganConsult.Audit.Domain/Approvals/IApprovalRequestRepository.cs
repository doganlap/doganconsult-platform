using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DoganConsult.Audit.Workflow;
using Volo.Abp.Domain.Repositories;

namespace DoganConsult.Audit.Approvals;

public interface IApprovalRequestRepository : IRepository<ApprovalRequest, Guid>
{
    Task<ApprovalRequest?> FindByRequestNumberAsync(
        string requestNumber,
        CancellationToken cancellationToken = default);

    Task<List<ApprovalRequest>> GetListByEntityAsync(
        ApprovalEntityType entityType,
        Guid entityId,
        CancellationToken cancellationToken = default);

    Task<List<ApprovalRequest>> GetPendingRequestsAsync(
        Guid? assignedApproverId = null,
        ApprovalEntityType? entityType = null,
        int skipCount = 0,
        int maxResultCount = 10,
        CancellationToken cancellationToken = default);

    Task<List<ApprovalRequest>> GetListByRequesterAsync(
        Guid requesterId,
        ApprovalStatus? status = null,
        int skipCount = 0,
        int maxResultCount = 10,
        CancellationToken cancellationToken = default);

    Task<List<ApprovalRequest>> GetListByApproverAsync(
        Guid approverId,
        ApprovalStatus? status = null,
        int skipCount = 0,
        int maxResultCount = 10,
        CancellationToken cancellationToken = default);

    Task<long> GetPendingCountAsync(
        Guid? assignedApproverId = null,
        ApprovalEntityType? entityType = null,
        CancellationToken cancellationToken = default);

    Task<List<ApprovalRequest>> GetExpiredRequestsAsync(
        CancellationToken cancellationToken = default);
}
