namespace PolicyManagement.DTOs;

using System.ComponentModel.DataAnnotations;

public class CreatePolicyDto
{
    [Required(ErrorMessage = "Policy code is required.")]
    [StringLength(20, MinimumLength = 2, ErrorMessage = "Policy code must be between 2 and 20 characters.")]
    public string PolicyCode { get; set; } = string.Empty;

    [Required(ErrorMessage = "Policy name is required.")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "Policy name must be between 3 and 100 characters.")]
    public string PolicyName { get; set; } = string.Empty;

    [StringLength(500, MinimumLength = 3, ErrorMessage = "Description must be between 3 and 500 characters.")]
    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "Premium amount is required.")]
    [Range(0.01, 100000.0, ErrorMessage = "Premium amount must be between 0.01 and 100000.00")]
    public decimal PremiumAmount { get; set; }

    public bool IsActive { get; set; }
}