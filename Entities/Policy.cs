using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PolicyManagement.Models;

[Table("policies")]
public class Policy
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Required(ErrorMessage = "Policy code is required.")]
    [StringLength(20,MinimumLength = 2, ErrorMessage = "Policy code must be between 2 and 20 characters.")]
    [Column("policy_code")]
    public string PolicyCode { get; set; } = string.Empty;

    [Required(ErrorMessage = "Policy name is required.")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "Policy name must be between 3 and 100 characters.")]
    [Column("policy_name")]
    public string PolicyName { get; set; } = string.Empty;

    [StringLength(500,MinimumLength =3, ErrorMessage = "Description must be between 3 and 500 characters.")]
    [Column("description")]
    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "Premium amount is required.")]
    [Range(0.01, 100000.0, ErrorMessage = "Premium amount must be between 0.01 and 100000.00")]
    [Column("premium_amount")]
    public decimal PremiumAmount { get; set; }

    [Column("is_active")]
    public bool IsActive { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; }

    [Column("deleted_at")]
    public DateTime? DeletedAt { get; set; }

    // Navigation property for one-to-many relationship with PolicyEnrollments
    public ICollection<PolicyEnrollment> PolicyEnrollments { get; set; } = new List<PolicyEnrollment>();
}