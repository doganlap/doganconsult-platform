using System;
using System.ComponentModel.DataAnnotations;

namespace DoganConsult.AI.AIRequests;

public class CreateAIRequestDto
{
    [Required]
    [StringLength(200)]
    public string RequestType { get; set; } = string.Empty;

    [Required]
    [StringLength(5000)]
    public string InputText { get; set; } = string.Empty;

    public Guid? OrganizationId { get; set; }
}
