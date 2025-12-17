namespace DoganConsult.Audit.Workflow;

/// <summary>
/// Represents the lifecycle status of an entity that supports approval workflow
/// </summary>
public enum EntityStatus
{
    /// <summary>
    /// Entity is in draft state, not yet submitted for approval
    /// </summary>
    Draft = 0,
    
    /// <summary>
    /// Entity is pending approval
    /// </summary>
    PendingApproval = 1,
    
    /// <summary>
    /// Entity has been approved and is active
    /// </summary>
    Approved = 2,
    
    /// <summary>
    /// Entity approval was rejected
    /// </summary>
    Rejected = 3,
    
    /// <summary>
    /// Entity is active (post-approval or no approval required)
    /// </summary>
    Active = 4,
    
    /// <summary>
    /// Entity is suspended/inactive
    /// </summary>
    Suspended = 5,
    
    /// <summary>
    /// Entity is archived
    /// </summary>
    Archived = 6
}
