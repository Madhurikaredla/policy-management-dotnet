using System.ComponentModel.DataAnnotations;

namespace PolicyManagement.DTOs;

public class UpdatePolicyDto
{
    [StringLength(100, MinimumLength = 3, ErrorMessage = "Policy name must be between 3 and 100 characters.")]
    public string? PolicyName { get; set; }

    [StringLength(500, MinimumLength = 3, ErrorMessage = "Description must be between 3 and 500 characters.")]
    public string? Description { get; set; }

    [Range(0.01, 100000.0, ErrorMessage = "Premium amount must be between 0.01 and 100000.00")]
    public decimal? PremiumAmount { get; set; }

    public bool? IsActive { get; set; }
}