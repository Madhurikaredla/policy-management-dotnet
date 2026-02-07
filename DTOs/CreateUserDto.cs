using System.ComponentModel.DataAnnotations;

namespace PolicyManagement.DTOs;

public class CreateUserDto
{
    [Required(ErrorMessage = "Name is required.")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "Name must be between 3 and 100 characters.")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email format.")]
    [StringLength(100, MinimumLength = 5, ErrorMessage = "Email must be between 5 and 100 characters.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required.")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Role is required.")]
    [RegularExpression("ROLE_USER|ROLE_ADMIN", ErrorMessage = "Role must be ROLE_USER or ROLE_ADMIN.")]
    public string Role { get; set; } = "ROLE_USER";

    public string? CountryCode { get; set; }

    public string? PhoneNumber { get; set; }

    public bool Active { get; set; }
}