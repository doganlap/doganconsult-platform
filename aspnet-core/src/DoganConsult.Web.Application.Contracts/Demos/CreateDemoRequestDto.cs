using System;
using System.ComponentModel.DataAnnotations;

namespace DoganConsult.Web.Demos
{
    public class CreateDemoRequestDto
    {
        // Customer Information
        [Required]
        [StringLength(256)]
        public string? CustomerName { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(256)]
        public string? CustomerEmail { get; set; }

        [StringLength(50)]
        public string? CustomerPhone { get; set; }

        [StringLength(256)]
        public string? CompanyName { get; set; }

        [StringLength(128)]
        public string? Industry { get; set; }

        // Demo Details
        [Required]
        [StringLength(512)]
        public string? DemoTitle { get; set; }

        [StringLength(2000)]
        public string? DemoDescription { get; set; }

        [StringLength(128)]
        public string? DemoType { get; set; }

        public DateTime? RequestedDate { get; set; }

        public int? EstimatedDuration { get; set; }

        [StringLength(256)]
        public string? DemoEnvironment { get; set; }

        // Requirements
        [StringLength(2000)]
        public string? SpecialRequirements { get; set; }

        [StringLength(50)]
        public string? Priority { get; set; }

        // For backward compatibility (not mapped to entity)
        public string RequestSource { get; set; } = "Internal";
    }
}
