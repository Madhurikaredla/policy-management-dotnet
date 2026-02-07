using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PolicyManagement.Constants;

namespace PolicyManagement.Models;

[Table("users")]
public class User
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Required(ErrorMessage = "Name is required.")]
    [StringLength(100,MinimumLength =3, ErrorMessage = "Name must be between 3 and 100 characters.")]
    [Column("name")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email format.")]
    [StringLength(100, MinimumLength = 5, ErrorMessage = "Email must be between 5 and 100 characters.")]
    [Column("email")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password hash is required.")]
    [Column("password_hash")]
    public string PasswordHash { get; set; } = string.Empty;

    [Required(ErrorMessage = "Role is required.")]
    [Column("role")]
    public string Role { get; set; } = Roles.ROLE_USER;

    [Column("country_code")]
    public string? CountryCode { get; set; }

    [Column("phone_number")]
    public string? PhoneNumber { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; }

    [Column("active")]
    public bool Active { get; set; }

    // Navigation property for one-to-many relationship with PolicyEnrollments
    public ICollection<PolicyEnrollment> PolicyEnrollments { get; set; } = new List<PolicyEnrollment>();
}   