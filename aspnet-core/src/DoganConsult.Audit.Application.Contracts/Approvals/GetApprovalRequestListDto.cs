using DoganConsult.Audit.Workflow;
using Volo.Abp.Application.Dtos;

namespace DoganConsult.Audit.Approvals;

public class GetApprovalRequestListDto : PagedAndSortedResultRequestDto
{
    public ApprovalStatus? Status { get; set; }
    public ApprovalEntityType? EntityType { get; set; }
    public ApprovalPriority? Priority { get; set; }
    public string? RequestNumber { get; set; }
    public string? RequesterName { get; set; }
    public bool? PendingOnly { get; set; }
    public bool? MyRequestsOnly { get; set; }
    public bool? AssignedToMe { get; set; }
}
