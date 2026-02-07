using System.ComponentModel.DataAnnotations;

namespace PolicyManagement.DTOs;

public class EnrollmentApprovalDto
{
    [StringLength(500, ErrorMessage = "Admin remarks cannot exceed 500 characters.")]
    public string? AdminRemarks { get; set; }
}
