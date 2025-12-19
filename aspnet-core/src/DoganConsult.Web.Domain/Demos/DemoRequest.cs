using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace DoganConsult.Web.Demos;

/// <summary>
/// Demo Request Entity - Represents a customer demo request with full workflow tracking
/// </summary>
public class DemoRequest : FullAuditedAggregateRoot<int>
{
    // Customer Information
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
    public string? CustomerPhone { get; set; }
    public string? CompanyName { get; set; }
    public string? Industry { get; set; }

    // Demo Details
    public string DemoTitle { get; set; } = string.Empty;
    public string? DemoDescription { get; set; }
    public string DemoType { get; set; } = string.Empty;
    public DateTime RequestedDate { get; set; }
    public DateTime? ScheduledDate { get; set; }
    public int EstimatedDuration { get; set; }
    public string? DemoEnvironment { get; set; }

    // Requirements & Status
    public string? SpecialRequirements { get; set; }
    public string Priority { get; set; } = "Medium";
    public string CurrentStatus { get; set; } = "Pending";
    public string ApprovalStatus { get; set; } = "Pending";

    // Workflow Tracking
    public string? AssignedTo { get; set; }
    public string? ApprovedBy { get; set; }
    public string? RejectedBy { get; set; }
    public string? RejectionReason { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public DateTime? RejectedAt { get; set; }
    public DateTime? CompletedAt { get; set; }

    // Progress Tracking
    public int ProgressPercentage { get; set; }
    public string? InternalNotes { get; set; }

    // Constructor
    protected DemoRequest()
    {
        // For EF Core
    }

    public DemoRequest(
        int id,
        string customerName,
        string customerEmail,
        string demoTitle,
        string demoType,
        DateTime requestedDate,
        int estimatedDuration)
    {
        Id = id;
        CustomerName = customerName;
        CustomerEmail = customerEmail;
        DemoTitle = demoTitle;
        DemoType = demoType;
        RequestedDate = requestedDate;
        EstimatedDuration = estimatedDuration;
        CurrentStatus = "Pending";
        ApprovalStatus = "Pending";
        Priority = "Medium";
        ProgressPercentage = 0;
    }

    // Business Methods
    public void Approve(string approvedBy)
    {
        ApprovalStatus = "Approved";
        ApprovedBy = approvedBy;
        ApprovedAt = DateTime.UtcNow;
        CurrentStatus = "Approved";
        ProgressPercentage = 25;
    }

    public void Reject(string rejectedBy, string reason)
    {
        ApprovalStatus = "Rejected";
        RejectedBy = rejectedBy;
        RejectionReason = reason;
        RejectedAt = DateTime.UtcNow;
        CurrentStatus = "Rejected";
    }

    public void Schedule(DateTime scheduledDate, string assignedTo)
    {
        ScheduledDate = scheduledDate;
        AssignedTo = assignedTo;
        CurrentStatus = "Scheduled";
        ProgressPercentage = 50;
    }

    public void Start()
    {
        CurrentStatus = "InProgress";
        ProgressPercentage = 60;
    }

    public void Complete()
    {
        CurrentStatus = "Completed";
        CompletedAt = DateTime.UtcNow;
        ProgressPercentage = 100;
    }

    public void MoveToAccepted()
    {
        CurrentStatus = "Accepted";
        ProgressPercentage = 75;
    }

    public void MoveToPOC()
    {
        CurrentStatus = "POC";
        ProgressPercentage = 80;
    }

    public void MoveToProduction()
    {
        CurrentStatus = "Production";
        ProgressPercentage = 100;
    }
}
