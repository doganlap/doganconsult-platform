using System;
using System.ComponentModel.DataAnnotations;

namespace DoganConsult.Web.Demos
{
    public class UpdateDemoRequestDto
    {
        [StringLength(200)]
        public string? CustomerName { get; set; }
        
        [EmailAddress]
        [StringLength(200)]
        public string? CustomerEmail { get; set; }
        
        [StringLength(200)]
        public string? CompanyName { get; set; }
        
        [StringLength(2000)]
        public string? SpecialRequirements { get; set; }
        
        public DateTime? ScheduledDate { get; set; }
    }
}
