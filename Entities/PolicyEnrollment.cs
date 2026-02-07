using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PolicyManagement.Models;

[Table("policy_enrollments")]
public class PolicyEnrollment
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Required(ErrorMessage = "User ID is required.")]
    [Column("user_id")]
    public int UserId { get; set; }

    [Required(ErrorMessage = "Policy ID is required.")]
    [Column("policy_id")]
    public int PolicyId { get; set; }

    [Required(ErrorMessage = "Status is required.")]
    [StringLength(20, ErrorMessage = "Status cannot exceed 20 characters.")]
    [Column("status")]
    public string Status { get; set; } = "Pending";

    [Column("requested_at")]
    public DateTime RequestedAt { get; set; } = DateTime.UtcNow;

    [Column("approved_at")]
    public DateTime? ApprovedAt { get; set; }

    [Column("rejected_at")]
    public DateTime? RejectedAt { get; set; }

    [StringLength(500, ErrorMessage = "Admin remarks cannot exceed 500 characters.")]
    [Column("admin_remarks")]
    public string? AdminRemarks { get; set; }

    // Navigation properties
    [ForeignKey("UserId")]
    public User? User { get; set; }

    [ForeignKey("PolicyId")]
    public Policy? Policy { get; set; }
}
