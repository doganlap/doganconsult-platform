namespace DoganConsult.Audit.Workflow;

/// <summary>
/// Types of entities that can be submitted for approval
/// </summary>
public enum ApprovalEntityType
{
    Organization = 1,
    Workspace = 2,
    Document = 3,
    UserProfile = 4,
    AIConfiguration = 5,
    Custom = 99
}
