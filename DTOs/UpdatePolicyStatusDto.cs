using System.ComponentModel.DataAnnotations;
namespace PolicyManagement.DTOs;
public class UpdatePolicyStatusDto
{
    [Required(ErrorMessage = "IsActive status is required.")]
    public bool IsActive { get; set; }
}