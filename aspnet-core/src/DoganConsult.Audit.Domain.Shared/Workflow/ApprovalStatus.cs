namespace DoganConsult.Audit.Workflow;

/// <summary>
/// Represents the status of an approval request
/// </summary>
public enum ApprovalStatus
{
    /// <summary>
    /// Request is pending review
    /// </summary>
    Pending = 0,
    
    /// <summary>
    /// Request has been approved
    /// </summary>
    Approved = 1,
    
    /// <summary>
    /// Request has been rejected
    /// </summary>
    Rejected = 2,
    
    /// <summary>
    /// Request was cancelled by the requester
    /// </summary>
    Cancelled = 3,
    
    /// <summary>
    /// Request expired without action
    /// </summary>
    Expired = 4
}
