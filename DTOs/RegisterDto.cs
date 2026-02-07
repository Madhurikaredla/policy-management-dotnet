using System.ComponentModel.DataAnnotations;
using PolicyManagement.Constants;

namespace PolicyManagement.DTOs;

public class RegisterDto
{
    [Required(ErrorMessage = "Name is required.")]
    [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email format.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required.")]
    [MinLength(8, ErrorMessage = "Password must be at least 8 characters long.")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Role is required.")]
    [RegularExpression("ROLE_USER|ROLE_ADMIN", ErrorMessage = "Role must be ROLE_USER or ROLE_ADMIN.")]
    public string Role { get; set; } = Roles.ROLE_USER;

    public string? CountryCode { get; set; }
    
    public string? PhoneNumber { get; set; }
    
    public bool Active { get; set; } = true;
}
