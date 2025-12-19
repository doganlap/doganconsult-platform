using System;

namespace DoganConsult.Web.Demos
{
    public class DemoRequestDto
    {
        public int Id { get; set; }
        
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

        // Audit fields
        public DateTime CreationTime { get; set; }
    }
}
