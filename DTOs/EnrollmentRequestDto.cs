using System.ComponentModel.DataAnnotations;

namespace PolicyManagement.DTOs;

public class EnrollmentRequestDto
{
    [Required(ErrorMessage = "Policy ID is required.")]
    [Range(1, int.MaxValue, ErrorMessage = "Policy ID must be greater than 0.")]
    public int PolicyId { get; set; }
}
