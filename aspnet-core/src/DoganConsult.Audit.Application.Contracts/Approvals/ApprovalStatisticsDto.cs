namespace DoganConsult.Audit.Approvals;

public class ApprovalStatisticsDto
{
    public int TotalPending { get; set; }
    public int TotalApproved { get; set; }
    public int TotalRejected { get; set; }
    public int TotalCancelled { get; set; }
    public int TotalExpired { get; set; }
    
    public int MyPendingApprovals { get; set; }
    public int MySubmittedRequests { get; set; }
    public int MyApprovedToday { get; set; }
    public int MyRejectedToday { get; set; }

    public int PendingOrganizations { get; set; }
    public int PendingWorkspaces { get; set; }
    public int PendingDocuments { get; set; }
    public int PendingUserProfiles { get; set; }

    public double AverageApprovalTimeHours { get; set; }
}
