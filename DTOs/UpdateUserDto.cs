using System.ComponentModel.DataAnnotations;

namespace PolicyManagement.DTOs;

public class UpdateUserDto
{
    [StringLength(100, MinimumLength = 3, ErrorMessage = "Name must be between 3 and 100 characters.")]
    public string? Name { get; set; } = string.Empty;

    [EmailAddress(ErrorMessage = "Invalid email format.")]
    [StringLength(100, MinimumLength = 5, ErrorMessage = "Email must be between 5 and 100 characters.")]
    public string? Email { get; set; } = string.Empty;

    public string? Password { get; set; } = string.Empty;
    
    [RegularExpression("ROLE_USER|ROLE_ADMIN", ErrorMessage = "Role must be ROLE_USER or ROLE_ADMIN.")]
    public string? Role { get; set; } = string.Empty;

    public string? CountryCode { get; set; }

    public string? PhoneNumber { get; set; }

    public bool? Active { get; set; }
}